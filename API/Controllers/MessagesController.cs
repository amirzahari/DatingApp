namespace API.Controllers;

[Authorize]
public class MessagesController : BaseAPIController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser(
        [FromQuery] MessageParams messageParam)
    {
        messageParam.Username = User.GetUsername();

        var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParam);

        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize,
            messages.TotalCount, messages.TotalPages);

        return messages;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await _unitOfWork.MessageRepository.GetMessage(id);

        if (message.Sender.UserName != username && message.Recipient.UserName != username)
            return Unauthorized();

        if (message.Sender.UserName == username)
            message.SenderDeleted = true;

        if (message.Recipient.UserName == username)
            message.RecipientDeleted = true;

        if (message.SenderDeleted && message.RecipientDeleted)
            _unitOfWork.MessageRepository.DeleteMessage(message);

        if (await _unitOfWork.Complete())
            return Ok();

        return BadRequest("Problem deleting the message");
    }
}
