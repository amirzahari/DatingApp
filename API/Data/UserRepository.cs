using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            // *** [aznote] For performance purpose. We binding DTO at level of query
            // *** [aznote] the query will select only selected variable as we need in DTO.
            // *** [aznote] not the whole of data.

            // *** [aznote] using auto mapping
            // *** [aznote] PhotoUrl and Age, configure in AutoMapperProfiles.
            // *** [aznote] no need .Include(p => p.Photos)
            // *** [aznote] automatically done by EF

            return await _context.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();

            // *** [aznote] without auto mapping (manual)

            // return await _context.Users
            // .Where(x => x.UserName == username)
            // .Select(user => new MemberDto{
            //     Id = user.Id,
            //     Username = user.UserName,
            //     PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
            //     Age = user.DateOfBirth.CalculateAge(),
            //     KnownAs = user.KnownAs,
            //     Created = user.Created,
            //     LastActive = user.LastActive,
            //     Gender = user.Gender,
            //     Introduction = user.Introduction,
            //     LookingFor = user.LookingFor,
            //     Interests = user.Interests,
            //     City = user.City,
            //     Country = user.Country,
            //     Photos = user.Photos.Select(photo => new PhotoDto{
            //         Id = photo.Id,
            //         Url = photo.Url,
            //         IsMain = photo.IsMain,
            //     }).ToArray()
            // }).SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            // var query =  _context.Users
            //     .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            //     .AsNoTracking()
            //     .AsQueryable();

            //query = query.Where(u => u.Username != userParams.CurrentUsername);
            
            
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };
            
            return await PagedList<MemberDto>.CreateAsync(
                query.ProjectTo<MemberDto>(
                    _mapper.ConfigurationProvider).AsNoTracking(), 
                    userParams.PageNumber, userParams.PageSize);

            // var query = _context.Users
            //     .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            //     .AsNoTracking();

            //var query = _context.Users.AsQueryable();

            // query = query.Where(u => u.UserName != userParams.CurrentUsername);

            // que
            
            // return await PagedList<MemberDto>.CreateAsync(
            //     query, userParams.PageNumber, userParams.PageSize);
        }

        // public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        // {
        //     return await _context.Users
        //     .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        //     .ToListAsync();
        // }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(p => p.Photos)
            .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}