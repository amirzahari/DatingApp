

namespace API.Data;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public MessageRepository(DataContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
    }

    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
        return await _context.Connections.FindAsync(connectionId);
    }

    public async Task<Group> GetGroupForConnection(string connectionId)
    {
        return await _context.Groups
            .Include(c => c.Connections)
            .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages
            .Include(u => u.Sender)
            .Include(u => u.Recipient)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Group> GetMessageGroup(string groupName)
    {
        return await _context.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = _context.Messages
            .OrderByDescending(m => m.MessageSent)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(u =>
                u.RecipientUsername == messageParams.Username &&
                u.RecipientDeleted == false),
            "Outbox" => query.Where(u =>
                u.SenderUsername == messageParams.Username &&
                u.SenderDeleted == false),
            _ => query.Where(u =>
                u.RecipientUsername == messageParams.Username &&
                u.DateRead == null &&
                u.RecipientDeleted == false
                )
        };

        return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);

    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(
        string currentUsername, string recipientUsername)
    {
        // *** use prjection mapper. can remove Include.
        var messages = await _context.Messages
            .Where(m =>
                (
                    m.Recipient.UserName == currentUsername &&
                    m.Sender.UserName == recipientUsername &&
                    m.RecipientDeleted == false
                ) || (
                    m.Recipient.UserName == recipientUsername &&
                    m.Sender.UserName == currentUsername &&
                    m.SenderDeleted == false
                )
            )
            .OrderBy(m => m.MessageSent)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        // var messages = await _context.Messages
        // .Include(u => u.Sender).ThenInclude(p => p.Photos)
        // .Include(u => u.Recipient).ThenInclude(p => p.Photos)
        // .Where(m => 
        //     (
        //         m.Recipient.UserName == currentUsername && 
        //         m.Sender.UserName == recipientUsername &&
        //         m.RecipientDeleted == false 
        //     ) || (
        //         m.Recipient.UserName == recipientUsername && 
        //         m.Sender.UserName == currentUsername &&
        //         m.SenderDeleted == false
        //     )
        // )
        // .OrderBy(m => m.MessageSent)
        // .ToListAsync();

        var unreadMessages = messages.Where
            (um =>
                um.DateRead == null &&
                um.RecipientUsername == currentUsername
            ).ToList();

        if (unreadMessages.Any())
        {
            foreach (var unreadMessage in unreadMessages)
            {
                unreadMessage.DateRead = DateTime.UtcNow;
            }
            // *** save context from caller. hub or controller.
        }

        //return _mapper.Map<IEnumerable<MessageDto>>(messages);
        return messages;
    }

    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }
}
