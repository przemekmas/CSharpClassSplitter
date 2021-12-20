using CSharpClassSplitter.Entities;
using CSharpClassSplitter.Events;
using System.Text;

namespace CSharpClassSplitter.Operations
{
    public class ClassProcessor : ProcessorBase
    {
        public StringBuilder ClassBuilder { get; set; }

        public string ClassName { get; set; }

        public ClassCompleteDelegate Complete { get; set; }

        public ClassProcessor()
        {
            ClassBuilder = new StringBuilder();
        }

        // Process every within the class
        public void Process(string line)
        {
            SetClassName(line);

            CurlyBraceLeftCount += GetLeftCurlyBraceCount(line);
            CurlyBraceRightCount += GetRightCurlyBraceCount(line);
            ClassBuilder.AppendLine(line);

            ClassEnd();
        }

        // Handle class end
        private void ClassEnd()
        {
            if (CurlyBraceLeftCount != 0
                && CurlyBraceRightCount != 0
                && CurlyBraceLeftCount == CurlyBraceRightCount)
            {
                var classContent = ClassBuilder.ToString();
                var lastNewLineIndex = classContent.LastIndexOf("\r\n");
                classContent = classContent.Remove(lastNewLineIndex, 2);

                var currentClass = new Class
                {
                    Name = ClassName,
                    Content = classContent
                };

                Complete(currentClass);
            }
        }

        // Set class name if not set already
        private void SetClassName(string line)
        {
            var classIndex = line.IndexOf("class ");

            if (classIndex > -1 && string.IsNullOrWhiteSpace(ClassName))
            {
                ClassName = line.Substring(classIndex + 6);
            }
        }
    }
}
