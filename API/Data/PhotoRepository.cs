namespace API.Data;

public class PhotoRepository : IPhotoRepository
{
    private readonly DataContext _context;

    public PhotoRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Photo> GetPhotoById(int id)
    {
        return await _context.Photos
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
    {
        return await _context.Photos
            .IgnoreQueryFilters()
            .Where(photo => photo.IsApproved == false)
            .Select(photoDetails => new PhotoForApprovalDto
            {
                Id = photoDetails.Id,
                Url = photoDetails.Url,
                Username = photoDetails.AppUser.UserName,
                IsApproved = photoDetails.IsApproved
            }).ToListAsync();
    }

    public void RemovePhoto(Photo photo)
    {
        _context.Photos.Remove(photo);
    }
}
