using System.Collections.Generic;

namespace Andromeda.Data.Parsing.D2O.Models
{
    public class D2OClassDefinition
    {
        public int ClassIdentifier { get; set; }
        public string Name { get; set; }
        public string PackageName { get; set; }
        public List<D2OFieldDefinition> Fields { get; set; }

        public D2OClassDefinition() { }
        public D2OClassDefinition(int classIdentifier, string name, string packageName, List<D2OFieldDefinition> fields)
        {
            ClassIdentifier = classIdentifier;
            Name = name;
            PackageName = packageName;
            Fields = fields;
        }
    }
}
