using System.Collections.Generic;

namespace CSharpClassSplitter.Entities
{
    public class Namespace
    {
        public string Name { get; set; }
        public List<Class> Classes { get; set; }
    }
}