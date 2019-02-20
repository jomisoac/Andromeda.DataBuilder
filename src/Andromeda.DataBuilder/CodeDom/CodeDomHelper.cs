using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Andromeda.Data.Parsing.D2O.Enums;
using Andromeda.Data.Parsing.Helpers;
using Andromeda.IoC;

namespace Andromeda.DataBuilder.CodeDom
{
    public static class CodeDomHelper
    {
        public static CodeTypeReference GetPrimitivePropertyType(this D2OFieldValueType type) 
            => new CodeTypeReference(type.GetPrimitiveFieldType());
        public static CodeTypeReference GetPrimitiveVectorPropertyType(this D2OFieldValueType type) // avoid full Namespace
            => new CodeTypeReference($"List<{type.GetPrimitiveFieldString()}>");
        public static CodeTypeReference GetPrimitiveVectorOfVectorPropertyType(this D2OFieldValueType type) 
            => new CodeTypeReference($"List<List<{type.GetPrimitiveFieldString()}>>");
        public static CodeTypeReference GetPrimitiveVectorPropertyType(this string relativeType)
            => new CodeTypeReference($"List<{relativeType}>");
        public static CodeTypeReference GetPrimitiveVectorOfVectorPropertyType(this string relativeType) 
            => new CodeTypeReference($"List<List<{relativeType}>>");

        public static CodeMemberField CreateProperty(string propertyName)
            => new CodeMemberField
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = propertyName + " { get; set; }//" 
                // comment the semicolon of the member field, which avoid empty code block writing for getter/setter if you use CodeMemberProperty
            };
        public static CodeMemberField CreateProperty(string propertyName, string propertyType)
            => new CodeMemberField
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = propertyName + " { get; set; }//" ,
                // comment the semicolon of the member field, which avoid empty code block writing for getter/setter if you use CodeMemberProperty
                Type = new CodeTypeReference(propertyType)
            };
    }
}
