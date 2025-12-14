using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allen.Domain
{
	public class QuestionGenerationRequest
	{
		public ReadingPassage Passage { get; set; } = new ReadingPassage();
		public QuestionType QuestionType { get; set; }
		public int NumberOfQuestions { get; set; }
	}

}
