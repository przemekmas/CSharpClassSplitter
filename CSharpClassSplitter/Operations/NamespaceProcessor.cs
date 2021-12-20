using CSharpClassSplitter.Entities;
using CSharpClassSplitter.Events;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSharpClassSplitter.Operations
{
    public class NamespaceProcessor : ProcessorBase
    {
        public bool ClassFound { get; set; }

        public string NamespaceName { get; set; }

        public ClassProcessor ClassProcessor { get; set; }

        public NamespaceCompleteDelegate Complete { get; set; }

        public List<Class> Classes { get; set; }

        public NamespaceProcessor()
        {
            Classes = new List<Class>();
        }

        // Process every line within the namespace
        public void Process(string line)
        {
            SetNamespaceName(line);
            
            CurlyBraceLeftCount += GetLeftCurlyBraceCount(line);
            CurlyBraceRightCount += GetRightCurlyBraceCount(line);

            if (!ClassFound && IsClass(line))
            {
                ClassFound = true;
            }

            if (ClassFound)
            {
                if (ClassProcessor == null)
                {
                    ClassProcessor = new ClassProcessor();
                    ClassProcessor.Complete += ClassProcessingComplete;
                }
                
                ClassProcessor.Process(line);
            }

            NamespaceEnd();
        }

        // Set the namespace name if not set already
        private void SetNamespaceName(string line)
        {
            var namespaceIndex = line.IndexOf("namespace ");

            if (namespaceIndex > -1 && string.IsNullOrWhiteSpace(NamespaceName))
            {
                NamespaceName = line.Substring(namespaceIndex + 10);
            }
        }

        // Handle the end of the namespace
        private void NamespaceEnd()
        {
            if (CurlyBraceLeftCount != 0
                && CurlyBraceRightCount != 0
                && CurlyBraceLeftCount == CurlyBraceRightCount)
            {
                var currentNamespace = new Namespace()
                {
                    Name = NamespaceName,
                    Classes = Classes
                };

                Complete(currentNamespace);
            }
        }

        // Handle end of namespace processing
        private void ClassProcessingComplete(Class currentClass)
        {
            ClassProcessor = null;
            ClassFound = false;
            Classes.Add(currentClass);
        }

        // Is current string of text a class
        private bool IsClass(string line)
        {
            var regexMatch = Regex.Match(line, @".*(\bclass \b).*", RegexOptions.Singleline);
            return regexMatch.Success;
        }
    }
}
