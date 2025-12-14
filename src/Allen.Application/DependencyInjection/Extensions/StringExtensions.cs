namespace Allen.Application;

public static class StringExtensions
{
    public static string ConvertToCase(this string input, StringCaseType caseType)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Chuẩn hóa khoảng trắng và tách từ
        var words = input
            .Trim()
            .Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLowerInvariant())
            .ToList();

        switch (caseType)
        {
            case StringCaseType.Lower:
                return string.Join(" ", words);

            case StringCaseType.Upper:
                return string.Join(" ", words).ToUpperInvariant();

            case StringCaseType.Title:
                return string.Join(" ", words.Select(Capitalize));

            case StringCaseType.Pascal:
                return string.Concat(words.Select(Capitalize));

            case StringCaseType.Camel:
                return words.First() + string.Concat(words.Skip(1).Select(Capitalize));

            default:
                return input;
        }
    }

    private static string Capitalize(string word)
    {
        if (string.IsNullOrEmpty(word)) return word;
        return char.ToUpperInvariant(word[0]) + word.Substring(1);
    }
}
