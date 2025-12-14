using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allen.Domain
{
	public class ValidationResult
	{
		public bool IsValid { get; set; }
		public List<string> Errors { get; set; } = new();
		public GeneratedQuestion? CorrectedQuestion { get; set; }
		public string? ValidationDetails { get; set; }
	}

}
