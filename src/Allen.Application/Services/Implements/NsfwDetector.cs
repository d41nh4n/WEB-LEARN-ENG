using NsfwSpyNS;

public class NsfwDetector : INsfwDetector
{
	private static readonly Lazy<NsfwSpy> _lazy = new(() => new NsfwSpy());
	private NsfwSpy Detector => _lazy.Value;

	public bool IsExplicit(Stream imageStream)
	{
		using var ms = new MemoryStream();
		imageStream.CopyTo(ms);

		var result = Detector.ClassifyImage(ms.ToArray());

		return result.PredictedLabel.Equals("Pornography", StringComparison.OrdinalIgnoreCase)
			|| result.PredictedLabel.Equals("Sexy", StringComparison.OrdinalIgnoreCase)
			|| result.PredictedLabel.Equals("Hentai", StringComparison.OrdinalIgnoreCase);
	}
}


public interface INsfwDetector
{
    bool IsExplicit(Stream imageStream);
}