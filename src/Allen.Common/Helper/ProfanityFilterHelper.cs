using System.Text.Json;
using System.Text.RegularExpressions;

namespace Allen.Common
{
	public class ProfanityFilterHelper
	{
		private static HashSet<string> _prohibitedWords = new(StringComparer.OrdinalIgnoreCase);
		private static readonly string JsonPath = Path.Combine(AppContext.BaseDirectory, "Prompts", "badwords.json");

		static ProfanityFilterHelper()
		{
			LoadWords();
		}

		public static bool ContainsProhibitedWords(string content)
		{
			if (string.IsNullOrWhiteSpace(content))
				return false;

			// Normalize input
			string cleanedInput = content.Trim();

			foreach (var bad in _prohibitedWords)
			{
				if (string.IsNullOrWhiteSpace(bad))
					continue;

				// Escape bad word for regex
				string escaped = Regex.Escape(bad.Trim());

				// Word boundary cho Unicode: dùng (\b|[^a-zA-Z0-9])
				string pattern = $@"(?<!\w){escaped}(?!\w)";

				if (Regex.IsMatch(cleanedInput, pattern, RegexOptions.IgnoreCase))
					return true;
			}

			return false;
		}

		private static void LoadWords()
		{
			try
			{
				if (!File.Exists(JsonPath))
				{
					_prohibitedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					return;
				}

				var json = File.ReadAllText(JsonPath);
				var words = JsonSerializer.Deserialize<List<string>>(json);

				_prohibitedWords = words != null
					? new HashSet<string>(words
						.Where(w => !string.IsNullOrWhiteSpace(w))
						.Select(w => w.Trim()),
						StringComparer.OrdinalIgnoreCase)
					: new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			}
			catch
			{
				_prohibitedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			}
		}

		public static void Reload()
		{
			LoadWords();
		}
	}
}
