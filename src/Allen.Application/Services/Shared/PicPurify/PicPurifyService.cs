//using System.Net.Http.Headers;

//namespace Allen.Application;


//public class PicPurifyService : IPicPurifyService
//{
//	private readonly HttpClient _httpClient;
//	private readonly string _apiKey;

//	public PicPurifyService(HttpClient httpClient, IConfiguration config)
//	{
//		_httpClient = httpClient;
//		_apiKey = config["PicPurify:ApiKey"];
//	}

//	public async Task<bool> IsExplicitImageAsync(Stream imageStream)
//	{
//		using var content = new MultipartFormDataContent();

//		// API key
//		content.Add(new StringContent(_apiKey), "key");

//		// Image content
//		var imageContent = new StreamContent(imageStream);
//		imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
//		content.Add(imageContent, "file", "upload.jpg");

//		// Call PicPurify endpoint
//		var response = await _httpClient.PostAsync("https://api.picpurify.com/v1/moderate", content);
//		response.EnsureSuccessStatusCode();

//		var json = await response.Content.ReadAsStringAsync();
//		using var doc = JsonDocument.Parse(json);
//		var root = doc.RootElement;

//		// PicPurify response format example:
//		// "final_nudity": "safe" or "unsafe"
//		// "final_porn": "safe" or "unsafe"

//		var nudity = root.TryGetProperty("final_nudity", out var pNudity)
//					 && pNudity.GetString() == "unsafe";

//		var porn = root.TryGetProperty("final_porn", out var pPorn)
//				   && pPorn.GetString() == "unsafe";

//		// If either nudity or porn is unsafe -> explicit
//		return nudity || porn;
//	}
//}
