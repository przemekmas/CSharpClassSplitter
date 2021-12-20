using CSharpClassSplitter.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharpClassSplitter
{
    public class ProjectOperations
    {
        public void Process(string csprojFile, List<Class> classes)
        {
            if (File.Exists(csprojFile))
            {
                var csprojContent = string.Empty;
                var tempPath = Path.GetTempPath();
                var tempProjectPath = Path.Combine(tempPath, Path.GetFileName(csprojFile));

                if (File.Exists(tempProjectPath))
                {
                    File.Delete(tempProjectPath);
                }

                File.Copy(csprojFile, tempProjectPath);

                using (var streamReader = new StreamReader(tempProjectPath))
                {
                    var compileIncludeBuilder = new StringBuilder();
                    csprojContent = streamReader.ReadToEnd();
                    compileIncludeBuilder.AppendLine("  <ItemGroup>");

                    foreach (var currentClass in classes.Select(x => x.Name).Distinct())
                    {
                        if (!IsClassIncluded(csprojContent, currentClass))
                        {
                            compileIncludeBuilder
                                .Append("    <Compile Include=\"")
                                .Append(currentClass)
                                .AppendLine(".cs\" />");
                        }
                    }

                    compileIncludeBuilder.AppendLine("  </ItemGroup>");
                    compileIncludeBuilder.AppendLine("</Project>");

                    var indexEndOfProject = csprojContent.IndexOf("</Project>");
                    csprojContent = csprojContent.Remove(indexEndOfProject, 10);
                    csprojContent = csprojContent.Insert(indexEndOfProject, compileIncludeBuilder.ToString());
                }

                using (var streamWriter = new StreamWriter(tempProjectPath))
                {
                    streamWriter.Write(csprojContent);
                }

                File.Copy(tempProjectPath, csprojFile, true);
            }
            else
            {
                throw new Exception($"The file path \"{csprojFile}\" does not exist.");
            }
        }

        private bool IsClassIncluded(string csprojContent, string className)
        {
            var regexMatch = Regex.Match(csprojContent, $@".*(\bCompile Include\b).*({className}.cs)", RegexOptions.Singleline);
            return regexMatch.Success;
        }
    }
}
