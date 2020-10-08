using System.Collections.Generic;

namespace SnapTest.Middleware
{
    public class JsonSerializerOptions
    {
        public bool WriteIndented = true;
        public bool SerializeStrings = false;
        public string SelectPath = null;
        public List<string> ExcludedPaths { get; } = new List<string>();
    }
}
