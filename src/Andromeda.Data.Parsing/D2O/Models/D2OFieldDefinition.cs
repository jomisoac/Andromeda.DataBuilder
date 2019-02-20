using Andromeda.Data.Parsing.D2O.Enums;

namespace Andromeda.Data.Parsing.D2O.Models
{
    public class D2OFieldDefinition
    {
        public string Name { get; set; }
        public int TypeValue { get; set; }
        public int InnerTypeValue { get; set; }

        public D2OFieldDefinition() { }
        public D2OFieldDefinition(string name, int typeValue, int innerTypeValue)
        {
            Name = name;
            TypeValue = typeValue;
            InnerTypeValue = innerTypeValue;
        }
    }
}
