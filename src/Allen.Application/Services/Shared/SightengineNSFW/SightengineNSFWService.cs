using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Allen.Application;

public class SightengineNSFWService : INSFWDetectionService
{
	private readonly HttpClient _http;
	private readonly SightengineOptions _options;

	public SightengineNSFWService(
		HttpClient http,
		IOptions<SightengineOptions> options)
	{
		_http = http;
		_options = options.Value;
	}

	//public async Task<bool> IsExplicitImageAsync(Stream stream)
	//{
	//	var form = new MultipartFormDataContent();

	//	// Bật tất cả các model bạn cần
	//	form.Add(new StringContent("nudity,violence,gore,offensive,wad"), "models");
	//	form.Add(new StringContent(_options.ApiKey), "api_user");
	//	form.Add(new StringContent(_options.ApiSecret), "api_secret");

	//	var img = new StreamContent(stream);
	//	img.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
	//	form.Add(img, "media", "image.jpg");

	//	var response = await _http.PostAsync("https://api.sightengine.com/1.0/check.json", form);
	//	response.EnsureSuccessStatusCode();

	//	var json = await response.Content.ReadAsStringAsync();
	//	using var doc = JsonDocument.Parse(json);
	//	var root = doc.RootElement;

	//	// ⚠ Kiểm tra nudity
	//	double nudityScore = root.GetProperty("nudity").GetProperty("safe").GetDouble();
	//	bool isNudityUnsafe = nudityScore < 0.5;

	//	// ⚠ Kiểm tra violence
	//	//bool isViolence = root.TryGetProperty("violence", out var violence)
	//	//				  && violence.GetProperty("violence").GetDouble() > 0.3;

	//	// ⚠ Kiểm tra gore
	//	bool isGore = root.TryGetProperty("gore", out var gore)
	//				  && gore.GetProperty("prob").GetDouble() > 0.3;

	//	// ⚠ Kiểm tra offensive
	//	bool isOffensive = root.TryGetProperty("offensive", out var off)
	//					   && off.GetProperty("prob").GetDouble() > 0.3;

	//	// ⚠ Kiểm tra weapon/alcohol/drugs
	//	//bool isWAD = root.TryGetProperty("weapon", out var weapon)
	//	//			 && weapon.GetProperty("prob").GetDouble() > 0.3;

	//	// Return true nếu ảnh KHÔNG AN TOÀN (explicit)
	//	return isNudityUnsafe || isGore || isOffensive;
	//}
	public async Task<bool> IsExplicitImageAsync(Stream stream)
	{
		var form = new MultipartFormDataContent();
		form.Add(new StringContent("nudity"), "models");
		form.Add(new StringContent(_options.ApiKey), "api_user");
		form.Add(new StringContent(_options.ApiSecret), "api_secret");

		var img = new StreamContent(stream);
		img.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
		form.Add(img, "media", "image.jpg");

		var res = await _http.PostAsync("https://api.sightengine.com/1.0/check.json", form);
		res.EnsureSuccessStatusCode();

		var json = await res.Content.ReadAsStringAsync();
		using var doc = JsonDocument.Parse(json);

		var nudityScore = doc.RootElement
			.GetProperty("nudity")
			.GetProperty("safe")
			.GetDouble();

		return nudityScore < 0.5;
	}
}