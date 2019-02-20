using System;
using System.Collections.Generic;
using System.Text;
using Andromeda.Binary.Reader;
using Andromeda.Data.Parsing.D2O.Enums;
using Andromeda.Data.Parsing.D2O.Models;
using Andromeda.Data.Parsing.Helpers;
using Andromeda.Serialization.Deserialization;
using Andromeda.Serialization.Storage;

namespace Andromeda.Data.Parsing.D2O.Storage
{
    public class FieldDefDeserializerStorage : IDeserializerStorage<D2OFieldDefinition>
    {
        public Action<IDeserializer, IReader, D2OFieldDefinition> Deserialize => (des, reader, def) =>
        {
            def.Name = reader.ReadValue<string>();
            def.TypeValue = reader.ReadValue<int>();
            var valueType = (D2OFieldValueType) def.TypeValue;

            if (valueType.IsPrimitiveField()) return;
            if (def.TypeValue > 0) return; // field is another class

            reader.SkipString(); // skip inner field name
            def.InnerTypeValue = reader.ReadValue<int>();
            var innerValueType = (D2OFieldValueType) def.InnerTypeValue;

            if (innerValueType.IsPrimitiveField()) return;
            if (def.InnerTypeValue > 0) return; // inner field is another class

            def.TypeValue = (int) D2OFieldValueType.VectorOfVector; // field is a vector 2d 
            reader.SkipString(); // skip inner inner field name
            def.InnerTypeValue = reader.ReadValue<int>();
        };
    }
}
