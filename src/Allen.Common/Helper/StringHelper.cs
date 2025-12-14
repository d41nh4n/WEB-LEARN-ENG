namespace Allen.Common;

public static class StringHelper
{
    public static List<string> ParseStringToList(string? csv)
    {
        return string.IsNullOrWhiteSpace(csv)
            ? new List<string>()
            : csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                 .Where(s => s.Length > 0)
                 .Distinct(StringComparer.OrdinalIgnoreCase)
                 .ToList();
    }
    public static double RoundToNearestHalf(double band)
	{
		var floor = Math.Floor(band);
		var fraction = band - floor;

		if (fraction < 0.25)
			return floor;
		else if (fraction < 0.75)
			return floor + 0.5;
		else
			return floor + 1.0;
	}
}
