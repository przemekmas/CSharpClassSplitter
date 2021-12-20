using System.Text.RegularExpressions;

namespace CSharpClassSplitter.Operations
{
    public abstract class ProcessorBase
    {
        protected int CurlyBraceLeftCount { get; set; }

        protected int CurlyBraceRightCount { get; set; }

        protected bool IsComment { get; set; }

        protected int GetLeftCurlyBraceCount(string line)
        {
            var regexMatch = Regex.Matches(line, @".*({).*");
            return regexMatch.Count;
        }

        protected int GetRightCurlyBraceCount(string line)
        {
            var regexMatch = Regex.Matches(line, @".*(}).*");
            return regexMatch.Count;
        }
    }
}
