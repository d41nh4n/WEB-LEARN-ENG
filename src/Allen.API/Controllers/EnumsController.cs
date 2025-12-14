namespace Allen.API.Controllers;

[Route("enums")]
public class EnumsController : BaseApiController
{
    [HttpGet("role-types")]
    public IActionResult GetRoleTypes()
    {
        var result = EnumHelper.GetEnumList<RoleType>();
        return Ok(result);
    }
    [HttpGet("privacy-types")]
    public IActionResult GetPrivacyTypes()
    {
        var result = EnumHelper.GetEnumList<PrivacyType>();
        return Ok(result);
    }
    [HttpGet("activity-types")]
    public IActionResult GetActivityTypes()
    {
        var result = EnumHelper.GetEnumList<ActivityType>();
        return Ok(result);
    }
    [HttpGet("chart-types")]
    public IActionResult GetChartTypes()
    {
        var result = EnumHelper.GetEnumList<ChartType>();
        return Ok(result);
    }
    [HttpGet("content-types")]
    public IActionResult GetContentTypes()
    {
        var result = EnumHelper.GetEnumList<ContentType>();
        return Ok(result);
    }
    [HttpGet("learning-module-types")]
    public IActionResult GetLearningModuleTypes()
    {
        var result = EnumHelper.GetEnumList<LearningModuleType>();
        return Ok(result);
    }
    [HttpGet("level-types")]
    public IActionResult GetLevelTypes()
    {
        var result = EnumHelper.GetEnumList<LevelType>();
        return Ok(result);
    }
    [HttpGet("object-types")]
    public IActionResult GetObjectTypes()
    {
        var result = EnumHelper.GetEnumList<ObjectType>();
        return Ok(result);
    }
    [HttpGet("question-types")]
    public IActionResult GetQuestionTypes()
    {
        var result = EnumHelper.GetEnumList<QuestionType>();
        return Ok(result);
    }
    [HttpGet("question-reading-types")]
    public IActionResult GetQuestionForReadingTypes()
    {
        var result = EnumHelper.GetEnumList<QuestionForReadingType>();
        return Ok(result);
    }
	[HttpGet("question-listening-types")]
	public IActionResult GetQuestionForListeningTypes()
	{
		var result = EnumHelper.GetEnumList<QuestionForListeningType>();
		return Ok(result);
	}
	[HttpGet("reaction-types")]
    public IActionResult GetReactionTypes()
    {
        var result = EnumHelper.GetEnumList<ReactionType>();
        return Ok(result);
    }
    [HttpGet("skill-types")]
    public IActionResult GetSkillTypes()
    {
        var result = EnumHelper.GetEnumList<SkillType>();
        return Ok(result);
    }
    [HttpGet("test-types")]
    public IActionResult GetTestTypes()
    {
        var result = EnumHelper.GetEnumList<TestType>();
        return Ok(result);
    }
    [HttpGet("user-status-types")]
    public IActionResult GetUserStatusTypes()
    {
        var result = EnumHelper.GetEnumList<UserStatusType>();
        return Ok(result);
    }
    [HttpGet("media-types")]
    public IActionResult GetMedisTypes()
    {
        var result = EnumHelper.GetEnumList<MediaType>();
        return Ok(result);
    }
	[HttpGet("learning-unit-status")]
	public IActionResult GetLearningUnitStatusTypes()
	{
		var result = EnumHelper.GetEnumList<LearningUnitStatusType>();
		return Ok(result);
	}
}
