using System.Net.Http.Json;
using System.Text;

namespace Allen.Application;

[RegisterService(typeof(IGeminiService))]
public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly IChatHistoryService _chatHistory;
    private readonly string _apiKey;
    private readonly string _endpoint;
    private const string OutOfScopeMessage =
        "Xin lỗi, mình không thể cung cấp thông tin này. Bạn có thể tìm trên web hoặc ứng dụng khác để biết thông tin chính xác nhé!";

    public GeminiService(HttpClient httpClient, IConfiguration config, IChatHistoryService chatHistory)
    {
        _httpClient = httpClient;
        _chatHistory = chatHistory;
        _apiKey = config["Gemini:ApiKey"]
                  ?? throw new ArgumentNullException("Gemini:ApiKey");
        _endpoint = config["Gemini:Endpoint"]
                    ?? throw new ArgumentNullException("Gemini:Endpoint");
    }

    public async IAsyncEnumerable<string> AIAgentSpeakingAsync(string userId, string sessionId, GeminiRequest model)
    {
        var url = $"{_endpoint}?key={_apiKey}";

        // lấy history
        var history = await _chatHistory.GetHistoryAsync(userId, sessionId);

        // nếu session chưa có -> lưu SessionInfo
        if (!history.Any())
        {
            var promptText = model.Prompt ?? "";
            var title = promptText.Length > 20
                ? promptText.Substring(0, 20) + "..."
                : promptText;

            var info = new SessionInfo
            {
                SessionId = sessionId,
                Title = title,
            };

            await _chatHistory.SaveSessionInfoAsync(userId, info);
        }

        // build prompt
        var builtPrompt = BuildSpeakingPrompt(model);

        // Lưu user message ngay lập tức
        await _chatHistory.AddMessageAsync(userId, sessionId,
            new GeminiChatMessage { Role = "user", Content = builtPrompt });

        // build contents gửi Gemini
        var contents = history.Select(h => new
        {
            role = h.Role,
            parts = new[] { new { text = h.Content } }
        }).ToList();

        contents.Add(new
        {
            role = "user",
            parts = new[] { new { text = builtPrompt } }
        });

        var request = new { contents };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(request)
        };

        using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        var collected = new StringBuilder();
        StringBuilder? objBuffer = null;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line) || line is "[" or "]" or ",")
                continue;

            if (line.TrimStart().StartsWith("{"))
                objBuffer = new StringBuilder();

            if (objBuffer != null)
                objBuffer.AppendLine(line);

            if (line.TrimEnd().EndsWith("}") && objBuffer != null)
            {
                var jsonObject = objBuffer.ToString().Trim();
                objBuffer = null;

                string? parsedText = null;
                try
                {
                    using var doc = JsonDocument.Parse(jsonObject);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("text", out var textProperty) && textProperty.ValueKind == JsonValueKind.String)
                    {
                        parsedText = textProperty.GetString();
                    }
                }
                catch (JsonException)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(parsedText))
                {
                    collected.Append(parsedText);
                    yield return parsedText; // stream ra FE theo chunk
                }
            }
        }

        // Sau khi stream xong -> lưu message model
        await _chatHistory.AddMessageAsync(userId, sessionId,
            new GeminiChatMessage { Role = "model", Content = collected.ToString() });
    }
    public async Task<SpeakingEvaluationResult?> AIAgentSubmitSpeakingAsync(GeminiRequest model)
    {
        var url = $"{_endpoint.Replace(":streamGenerateContent", ":generateContent")}?key={_apiKey}";

        var prompt = BuildSpeakingIeltsExamerPrompt(model);
        var request = new
        {
            contents = new[]
            {
            new
            {
                role = "user",
                parts = new[] { new { text = prompt } }
            }
        }
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(request)
        };

        using var response = await _httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;

            if (root.TryGetProperty("candidates", out var candidates)
                && candidates.ValueKind == JsonValueKind.Array
                && candidates.GetArrayLength() > 0)
            {
                var text = candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

                if (!string.IsNullOrWhiteSpace(text))
                {
                    // Làm sạch nếu Gemini bọc ```json ... ```
                    var clean = text.Trim().Trim('`', '\n', '\r', ' ');
                    if (clean.StartsWith("json", StringComparison.OrdinalIgnoreCase))
                        clean = clean[4..].Trim();

                    var result = JsonSerializer.Deserialize<SpeakingEvaluationResult>(clean);
                    return result;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AI evaluation parse failed: {ex.Message}");
        }

        return null;
    }

    public async IAsyncEnumerable<string> AIAgentWritingAsync(GeminiRequest model)
    {
        var url = $"{_endpoint}?key={_apiKey}";
        var builtPrompt = BuildWritingPrompt(model);

        var request = new
        {
            contents = new object[]
            {
            new { role = "user", parts = new[] { new { text = builtPrompt } } }
            }
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(request)
        };

        using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        StringBuilder? objBuffer = null;
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line) || line is "[" or "]" or ",")
                continue;

            if (line.TrimStart().StartsWith("{"))
                objBuffer = new StringBuilder();

            if (objBuffer != null)
                objBuffer.AppendLine(line);

            if (line.TrimEnd().EndsWith("}") && objBuffer != null)
            {
                var jsonObject = objBuffer.ToString().Trim();
                objBuffer = null;

                string? parsedText = null;
                try
                {
                    using var doc = JsonDocument.Parse(jsonObject);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("text", out var textProperty) && textProperty.ValueKind == JsonValueKind.String)
                    {
                        parsedText = textProperty.GetString();
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (JsonException)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(parsedText))
                {
                    //Console.WriteLine("Parsed chunk: " + parsedText);
                    yield return parsedText;
                }
            }
        }
    }
    public async Task<string> AIAgentIeltsWritingAsync(GeminiRequest model)
    {
        var url = $"{_endpoint.Replace(":streamGenerateContent", ":generateContent")}?key={_apiKey}";
        object request;

        if (model.SessionId == "Task1")
        {
            var builtPrompt = BuildIeltsWriting1Prompt(model);
            request = new
            {
                contents = new object[]
                {
                new { role = "user", parts = new[] { new { text = builtPrompt } } }
                }
            };
        }
        else if (model.SessionId == "Task2")
        {
            var builtPrompt = BuildIeltsWriting2Prompt(model);
            request = new
            {
                contents = new object[]
                {
                new { role = "user", parts = new[] { new { text = builtPrompt } } }
                }
            };
        }
        else
        {
            throw new ArgumentException("Invalid SessionId. Expected 'Task1' or 'Task2'.");
        }

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(request)
        };

        using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;

            if (root.TryGetProperty("candidates", out var candidates)
                && candidates.ValueKind == JsonValueKind.Array && candidates.GetArrayLength() > 0)
            {
                var first = candidates[0];

                if (first.TryGetProperty("content", out var content)
                    && content.TryGetProperty("parts", out var parts)
                    && parts.ValueKind == JsonValueKind.Array && parts.GetArrayLength() > 0)
                {
                    var text = parts[0].GetProperty("text").GetString();
                    return text ?? string.Empty;
                }
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON parsing error: {ex.Message}");
        }

        return string.Empty;
    }

    public async IAsyncEnumerable<string> ChatGeminiAsync(string userId, string sessionId, GeminiRequest model)
    {
        var url = $"{_endpoint}?key={_apiKey}";

        // lấy history
        var history = await _chatHistory.GetHistoryAsync(userId, sessionId);

        // nếu session chưa có -> lưu SessionInfo
        if (!history.Any())
        {
            var promptText = model.Prompt ?? "";
            var title = promptText.Length > 20
                ? promptText.Substring(0, 20) + "..."
                : promptText;

            var info = new SessionInfo
            {
                SessionId = sessionId,
                Title = title,
            };

            await _chatHistory.SaveSessionInfoAsync(userId, info);
        }

        // build prompt
        var builtPrompt = BuildPrompt(model);

        // Lưu user message ngay lập tức
        await _chatHistory.AddMessageAsync(userId, sessionId,
            new GeminiChatMessage { Role = "user", Content = builtPrompt });

        // build contents gửi Gemini
        var contents = history.Select(h => new
        {
            role = h.Role,
            parts = new[] { new { text = h.Content } }
        }).ToList();

        contents.Add(new
        {
            role = "user",
            parts = new[] { new { text = builtPrompt } }
        });

        var request = new { contents };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(request)
        };

        using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        var collected = new StringBuilder();
        StringBuilder? objBuffer = null;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line) || line is "[" or "]" or ",")
                continue;

            if (line.TrimStart().StartsWith("{"))
                objBuffer = new StringBuilder();

            if (objBuffer != null)
                objBuffer.AppendLine(line);

            if (line.TrimEnd().EndsWith("}") && objBuffer != null)
            {
                var jsonObject = objBuffer.ToString().Trim();
                objBuffer = null;

                string? parsedText = null;
                try
                {
                    using var doc = JsonDocument.Parse(jsonObject);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("text", out var textProperty) && textProperty.ValueKind == JsonValueKind.String)
                    {
                        parsedText = textProperty.GetString();
                    }
                }
                catch (JsonException)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(parsedText))
                {
                    collected.Append(parsedText);
                    yield return parsedText; // stream ra FE theo chunk
                }
            }
        }

        // Sau khi stream xong -> lưu message model
        await _chatHistory.AddMessageAsync(userId, sessionId,
            new GeminiChatMessage { Role = "model", Content = collected.ToString() });
    }
    public async Task<string> AIAgentTranslateAsync(GeminiRequest model)
    {
        var url = $"{_endpoint.Replace(":streamGenerateContent", ":generateContent")}?key={_apiKey}";
        var builtPrompt = BuildTranslatePrompt(model);

        var request = new
        {
            contents = new object[]
            {
            new { role = "user", parts = new[] { new { text = builtPrompt } } }
            }
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(request)
        };

        using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;

            if (root.TryGetProperty("candidates", out var candidates)
                && candidates.ValueKind == JsonValueKind.Array && candidates.GetArrayLength() > 0)
            {
                var first = candidates[0];

                if (first.TryGetProperty("content", out var content) && content.TryGetProperty("parts", out var parts)
                    && parts.ValueKind == JsonValueKind.Array && parts.GetArrayLength() > 0)
                {
                    var text = parts[0].GetProperty("text").GetString();
                    //Console.WriteLine(text);
                    return text ?? string.Empty;
                }
            }
        }
        catch (JsonException) { }
        return string.Empty;
    }

    public async Task<string> BuildVocabularyAsync(List<string> listVocabulary)
    {
        if (listVocabulary == null || listVocabulary.Count == 0)
            return string.Empty;

        var url = $"{_endpoint.Replace(":streamGenerateContent", ":generateContent")}?key={_apiKey}";

        // 1. Chuẩn hóa word list (giảm lỗi AI)
        var wordListText = string.Join(", ",
            listVocabulary
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Select(w => w.Trim().ToLower())
                .Distinct()
        );

        // 2. Build prompt vocabulary
        var builtPrompt = BuildVocabularyPrompt(new GeminiRequest
        {
            Prompt = wordListText
        });

        // 3. Request body
        var request = new
        {
            contents = new object[]
            {
            new
            {
                role = "user",
                parts = new[] { new { text = builtPrompt } }
            }
            }
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(request)
        };

        using var response = await _httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead
        );

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        // 4. Parse Gemini response
        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;

            if (root.TryGetProperty("candidates", out var candidates)
                && candidates.ValueKind == JsonValueKind.Array
                && candidates.GetArrayLength() > 0)
            {
                var parts = candidates[0]
                    .GetProperty("content")
                    .GetProperty("parts");

                if (parts.ValueKind == JsonValueKind.Array && parts.GetArrayLength() > 0)
                {
                    return parts[0].GetProperty("text").GetString() ?? string.Empty;
                }
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Gemini JSON parse error: {ex.Message}");
        }

        return string.Empty;
    }

    //public async Task<List<GeneratedQuestion>> GenerateQuestionsAsync(QuestionGenerationRequest request)
    //{
    //	var prompt = BuildGeneratorPrompt(request);

    //	var requestBody = new
    //	{
    //		contents = new[]
    //		{
    //			new
    //			{
    //				parts = new[]
    //				{
    //					new { text = prompt }
    //				}
    //			}
    //		},
    //		generationConfig = new
    //		{
    //			temperature = 0.7,
    //			maxOutputTokens = 4096,
    //			responseMimeType = "application/json"
    //		}
    //	};

    //	var response = await _httpClient.PostAsJsonAsync($"{API_URL}?key={_apiKey}", requestBody);
    //	response.EnsureSuccessStatusCode();

    //	var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
    //	var jsonText = result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

    //	if (string.IsNullOrEmpty(jsonText))
    //	{
    //		throw new Exception("Gemini returned empty response");
    //	}

    //	var questions = JsonSerializer.Deserialize<QuestionsList>(jsonText, new JsonSerializerOptions
    //	{
    //		PropertyNameCaseInsensitive = true
    //	});

    //	return questions?.Questions ?? new List<GeneratedQuestion>();
    //}

    private string BuildWritingPrompt(GeminiRequest request)
    {
        var writingRules = LoadTemplate("writing.txt");

        return $@"
        {writingRules}

        =======================================
        [WRITING COMPARISON TASK]

        [CANDIDATE INPUT (Prompt)]:
        {request.Prompt?.Trim()}

        [REFERENCE SENTENCE (Description)]:
        {request.Description?.Trim()} 
        =======================================
        ";
    }
    private string BuildIeltsWriting1Prompt(GeminiRequest request)
    {
        var writingRules = LoadTemplate("writing_task1_ielts.txt");
        return $@"
        {writingRules}

        =========================================
        [INPUT DATA]

        [IELTS TASK 1 QUESTION / CHART DESCRIPTION]:
        {request.Description?.Trim()}

        [CANDIDATE'S REPORT]:
        {request.Prompt?.Trim()}
        =========================================
        ";
    }
    private string BuildIeltsWriting2Prompt(GeminiRequest request)
    {
        var writingRules = LoadTemplate("writing_task2_ielts.txt");
        return $@"
        {writingRules}

        =========================================
        [INPUT DATA]

        [IELTS TASK 1 QUESTION / CHART DESCRIPTION]:
        {request.Description?.Trim()}

        [CANDIDATE'S REPORT]:
        {request.Prompt?.Trim()}
        =========================================
        ";
    }

    private string BuildSpeakingPrompt(GeminiRequest request)
    {
        var speakingRules = LoadTemplate("speaking.txt");
        var userUtterance = request.Prompt?.Trim() ?? "Hello, can we start?";

        return $@"
        {speakingRules}

        ===========================================
        [CURRENT CONVERSATION INPUT]

        USER MESSAGE:
        {userUtterance}

        [END OF INPUT]
        ===========================================
        ";
    }
    private string BuildSpeakingIeltsExamerPrompt(GeminiRequest request)
    {
        var speakingRule = LoadTemplate("speaking_ielts_examer.txt");

        return $@"
        {speakingRule}

        ===========================================
        [IELTS GRADING DATA]

        [EXAM QUESTION]:
        {request.Description?.Trim()}

        [CANDIDATE RESPONSE]:
        {request.Prompt?.Trim()}
        ===========================================
        ";
    }

    private string BuildPrompt(GeminiRequest request)
    {
        var rules = LoadTemplate("rules.txt");

        return $@"
            [SYSTEM INSTRUCTIONS]
            {rules}

            =======================================
            [USER TASK & CONTEXT]

            USER CONTENT (Prompt):
            {request.Prompt?.Trim()}

            ADDITIONAL GUIDELINES (Description):
            {request.Description?.Trim()}

            =======================================
            ";
    }
    private string BuildTranslatePrompt(GeminiRequest request)
    {
        var rules = LoadTemplate("translate.txt");

        return $@"
        {rules}

        =======================================
        [TRANSLATION INPUT]

        TEXT TO TRANSLATE (Prompt):
        {request.Prompt?.Trim()}

        SPECIFIC INSTRUCTIONS (Description):
        {request.Description?.Trim()} 
        =======================================
        ";
    }

    private string BuildVocabularyPrompt(GeminiRequest request)
    {
        var rules = LoadTemplate("vocabulary_agent.txt");

        return $@"
        {rules}

        ========================================
        [INPUT WORD LIST]
        {request.Prompt?.Trim()}
        ========================================
        ";
    }

    private string LoadTemplate(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Prompts", fileName);
        return File.ReadAllText(path);
    }
    public async Task<OperationResult> TranslateAsync(TranslateRequest request)
    {
        try
        {
            var result = await AIAgentTranslateAsync(new GeminiRequest
            {
                Prompt = request.Prompt,
                Description = "Translate accurately and naturally."
            });

            if (string.IsNullOrWhiteSpace(result))
                return OperationResult.Failure("Translation failed or AI returned empty response.");

            return OperationResult.SuccessResult("Translated successfully", new { translated = result });
        }
        catch (Exception ex)
        {
            return OperationResult.Failure($"Translation error: {ex.Message}");
        }
    }
}