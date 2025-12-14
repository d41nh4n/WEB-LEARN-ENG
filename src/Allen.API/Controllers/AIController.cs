using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Allen.API;

[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly OpenAIOptions _options;
    private readonly HttpClient _httpClient;
    private readonly IGeminiService _geminiService;
    private readonly IChatHistoryService _chatHistory;

    public AIController(
    IGeminiService geminiService,
    IChatHistoryService chatHistoryService,
    IOptions<OpenAIOptions> options,
    IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _httpClient = httpClientFactory.CreateClient("OpenAIClient");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        _geminiService = geminiService;
        _chatHistory = chatHistoryService;
    }

    #region OpenAI
    //[HttpPost("chat")]
    //public async Task<IActionResult> Chat([FromBody] OpenAIRequest request)
    //{
    //    var chatRequest = new ChatRequest
    //    {
    //        Model = _options.OpenAIModel,
    //        Messages = new List<ChatMessage>
    //    {
    //        new ChatMessage { Role = "user", Content = request.Prompt }
    //    }
    //    };

    //    var content = new StringContent(
    //        JsonSerializer.Serialize(chatRequest),
    //        Encoding.UTF8,
    //        "application/json"
    //    );

    //    var response = await _httpClient.PostAsync(_options.OpenAIUrl, content);

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        var error = await response.Content.ReadAsStringAsync();
    //        return StatusCode((int)response.StatusCode, error);
    //    }

    //    var resultJson = await response.Content.ReadAsStringAsync();
    //    var result = JsonSerializer.Deserialize<ChatResponse>(resultJson);

    //    var reply = result?.Choices?.FirstOrDefault()?.Message?.Content;

    //    return Ok(new { reply });
    //}
    //[HttpPost("test-model")]
    //public async Task<IActionResult> TestModel([FromBody] string model)
    //{
    //    var body = new
    //    {
    //        model = model, // ví dụ: "gpt-4o"
    //        messages = new[]
    //        {
    //        new { role = "user", content = "level là tu thuoc band nao" }
    //    }
    //    };

    //    var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
    //    _httpClient.DefaultRequestHeaders.Authorization =
    //        new AuthenticationHeaderValue("Bearer", _options.ApiKey);

    //    var response = await _httpClient.PostAsync("https://api.chatanywhere.org/v1/chat/completions", content);

    //    var result = await response.Content.ReadAsStringAsync();
    //    return StatusCode((int)response.StatusCode, result);
    //}
    #endregion

    #region Gemini
    [HttpPost("gemini/speaking")]
    [Authorize]
    [ValidateModel]
    public async Task SpeakingWithGemini([FromBody] GeminiRequest model)
    {
        var userId = HttpContext.GetCurrentUserId().ToString();
        var sessionId = string.IsNullOrWhiteSpace(model.SessionId)
            ? Guid.NewGuid().ToString()
            : model.SessionId;

        Response.Headers.Append("X-Session-Id", sessionId);
        Response.Headers.Append("Content-Type", "text/plain; charset=utf-8");
        Response.Headers.Append("Access-Control-Expose-Headers", "X-Session-Id");

        await Response.StartAsync();

        await foreach (var chunk in _geminiService.AIAgentSpeakingAsync(userId, sessionId, model))
        {
            await Response.WriteAsync(chunk);
            await Response.Body.FlushAsync();
        }
    }

    [HttpPost("gemini/chat")]
    [Authorize]
    [ValidateModel]
    public async Task GeminiChat([FromBody] GeminiRequest model)
    {
        var userId = HttpContext.GetCurrentUserId().ToString();
        var sessionId = string.IsNullOrWhiteSpace(model.SessionId)
            ? Guid.NewGuid().ToString()
            : model.SessionId;

        Response.Headers.Append("X-Session-Id", sessionId);
        Response.Headers.Append("Content-Type", "text/plain; charset=utf-8");
        Response.Headers.Append("Access-Control-Expose-Headers", "X-Session-Id");

        await Response.StartAsync();

        await foreach (var chunk in _geminiService.ChatGeminiAsync(userId, sessionId, model))
        {
            await Response.WriteAsync(chunk);
            await Response.Body.FlushAsync();
        }
    }

    [HttpPost("translate")]
    [Authorize]
    public async Task<OperationResult> TranslateAsync([FromBody] TranslateRequest model)
    {
        return await _geminiService.TranslateAsync(model);
    }

    #region session management
    [HttpGet("gemini/sessions")]
    [Authorize]
    public async Task<IActionResult> GetSessions()
    {
        var userId = HttpContext.GetCurrentUserId().ToString();
        var sessions = await _chatHistory.GetAllSessionsAsync(userId);

        return Ok(new { sessions });
    }

    [HttpGet("gemini/{sessionId}/sessionId")]
    [Authorize]
    public async Task<IActionResult> GetSessionMessages(string sessionId)
    {
        var userId = HttpContext.GetCurrentUserId().ToString();

        var messages = await _chatHistory.GetHistoryAsync(userId, sessionId, 50);
        return Ok(new { sessionId, messages });
    }

    [HttpDelete("gemini/sessions")]
    [Authorize]
    public async Task<IActionResult> ClearAllSessions()
    {
        var userId = HttpContext.GetCurrentUserId().ToString();
        await _chatHistory.ClearAllSessionsForUserAsync(userId);
        return Ok("All sessions cleared.");
    }
    #endregion

    #endregion
}
