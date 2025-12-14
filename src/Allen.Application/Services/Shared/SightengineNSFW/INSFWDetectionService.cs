namespace Allen.Application;

public interface INSFWDetectionService
{
	Task<bool> IsExplicitImageAsync(Stream stream);
}
