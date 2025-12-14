namespace Allen.API.Controllers;

[Route("reactions")]
[Authorize]
public class ReactionsController(IReactionService _service) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet("{objectId}")]
    public async Task<IEnumerable<Reaction>> GetReactions(Guid objectId)
        => await _service.GetReactionsAsync(objectId);


    [HttpGet("{objectId}/user/{userId}")]
    public async Task<Reaction?> GetReactionByUser(Guid objectId, Guid userId)
        => await _service.GetReactionByUserAsync(userId, objectId);

    // GET /reactions/{objectId}/users?reactionType=Like
    [HttpGet("{objectId}/users")]
    public async Task<IEnumerable<ReactionUserModel>> GetUsersByReaction(Guid objectId, [FromQuery] string reactionType)
        => await _service.GetUsersByReactionAsync(objectId, reactionType);

    [AllowAnonymous]
    [HttpGet("{objectId}/summary")]
    public async Task<IEnumerable<ReactionSummaryModel>> GetSummaryReaction(Guid objectId)
    => await _service.GetSummaryReactionAsync(objectId);


    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> CreateOrUpdate([FromBody] CreateOrUpdateReactionModel model)
    {
        model.UserId = HttpContext.GetCurrentUserId();
        model.UserName = HttpContext.GetCurrentUserName();
        return await _service.CreateOrUpdateReactionAsync(model);
    }
}