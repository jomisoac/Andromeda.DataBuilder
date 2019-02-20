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
    public class ClassDefDeserializerStorage : IDeserializerStorage<D2OClassDefinition>
    {
        public Action<IDeserializer, IReader, D2OClassDefinition> Deserialize => (des, reader, def) =>
        {
            def.ClassIdentifier = reader.ReadValue<int>();
            def.Name = reader.ReadValue<string>();
            def.PackageName = reader.ReadValue<string>();

            var fieldsCount = reader.ReadValue<int>();
            def.Fields = new List<D2OFieldDefinition>(fieldsCount);
            for (var i = 0; i < fieldsCount; i++)
                def.Fields.Add(
                    des.Deserialize<D2OFieldDefinition>(reader));
        };
    }
}
