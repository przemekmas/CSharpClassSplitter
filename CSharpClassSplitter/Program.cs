using System;
using System.IO;
using System.Collections.Generic;
using CSharpClassSplitter.Entities;
using System.Linq;

namespace CSharpClassSplitter
{
    public class Program
    {
        static ProjectOperations ProjectOperations => new ProjectOperations();

        static void Main(string[] args)
        {
            // Use dummy parameters for testing/debugging
#if DEBUG
            var currentDirPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            args = new string[3];
            args[0] = Path.Combine(currentDirPath, @"..\..\..\DummyProject\LargeDummyClass.cs");
            args[1] = Path.Combine(currentDirPath, @"..\..\..\DummyProject");
            args[2] = Path.Combine(currentDirPath, @"..\..\..\DummyProject\DummyProject.csproj");
#endif

            var sourceFile = args[0];
            var targetDirectory = args[1];
            var csprojFile = args[2];

            var classSplitterOperations = new ClassSplitterOperations();

            if (classSplitterOperations.TryProcess(sourceFile, out List<Namespace> namespaces, out string usings))
            {
                // Go through each class and create a file for it
                foreach (var currentNamespace in namespaces)
                {
                    foreach (var currentClass in currentNamespace.Classes)
                    {
                        WriteClassToFile(targetDirectory, currentClass, currentNamespace.Name, usings);
                    }
                }

                // Add the classes to the target csproj file
                var allClasses = namespaces.SelectMany(x => x.Classes);
                ProjectOperations.Process(csprojFile, allClasses.ToList());
            }
        }

        private static void WriteClassToFile(string targetDirectory, Class currentClass, string namespaceName, string usings)
        {
            using (var streamWriter = new StreamWriter(Path.Combine(targetDirectory, $"{currentClass.Name}.cs")))
            {
                streamWriter.WriteLine(usings);
                streamWriter.WriteLine($"namespace {namespaceName}");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine(currentClass.Content);
                streamWriter.WriteLine("}");
            }
        }
    }
}
