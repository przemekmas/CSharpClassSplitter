using CSharpClassSplitter.Entities;
using CSharpClassSplitter.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharpClassSplitter
{
    public class ClassSplitterOperations
    {
        public bool NamespaceFound { get; set; }

        public List<Namespace> Namespaces { get; set; }

        public NamespaceProcessor NamespaceProcessor { get; set; }

        public StringBuilder UsingBuilder { get; set; }

        public ClassSplitterOperations()
        {
            Namespaces = new List<Namespace>();
            UsingBuilder = new StringBuilder();
        }

        // Process a class file and get all namespaces and their classes
        public bool TryProcess(string filePath, out List<Namespace> namespaces, out string usings)
        {
            var success = false;
            namespaces = null;

            if (File.Exists(filePath))
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    var currentClassName = string.Empty;
                    var line = string.Empty;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (IsUsing(line))
                        {
                            UsingBuilder.AppendLine(line);
                        }

                        if (IsNamespace(line))
                        {
                            NamespaceFound = true;
                        }

                        if (NamespaceFound)
                        {
                            if (NamespaceProcessor == null)
                            {
                                NamespaceProcessor = new NamespaceProcessor();
                                NamespaceProcessor.Complete += NamespaceProcessingComplete;
                            }
                            NamespaceProcessor.Process(line);
                        }
                    }
                    namespaces = Namespaces;
                    usings = UsingBuilder.ToString();
                    success = namespaces.Any();
                }
            }
            else
            {
                throw new Exception($"The file path \"{filePath}\" does not exist.");
            }

            return success;
        }

        // Handle end of namespace processing
        private void NamespaceProcessingComplete(Namespace currentNamespace)
        {
            NamespaceProcessor = null;
            NamespaceFound = false;
            Namespaces.Add(currentNamespace);
        }

        // Is current string of text a namespace
        private bool IsNamespace(string line)
        {
            var regexMatch = Regex.Match(line, @".*(\bnamespace \b).*", RegexOptions.Singleline);
            return regexMatch.Success;
        }

        // Is current string a using
        private bool IsUsing(string line)
        {
            var regexMatch = Regex.Match(line, @".*(\busing\b).*[;]", RegexOptions.Singleline);
            return regexMatch.Success;
        }
    }
}
