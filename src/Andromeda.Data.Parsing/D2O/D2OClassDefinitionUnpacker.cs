using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Andromeda.Binary.Reader;
using Andromeda.Core.Extensions;
using Andromeda.Data.Parsing.D2O.Models;
using Andromeda.Data.Parsing.Files;
using Andromeda.Data.Parsing.Helpers;
using Andromeda.IoC;
using Andromeda.Serialization;

namespace Andromeda.Data.Parsing.D2O
{
    public class D2OClassDefinitionUnpacker : IFileUnpacker<D2OClassDefinition>
    {
        private ISerDes _serdes = ServiceLocator.Container.GetInstance<ISerDes>();
        public IReadOnlyCollection<D2OClassDefinition> Unpacked { get; private set; }

        public void Unpack(IReader reader)
        {
            if(!D2OHelper.TryParseHeader(reader)) throw new InvalidOperationException("d2o file is corrupted");

            _skipValuePositions(reader);

            var classDefCount = reader.ReadValue<int>();
            var unpacked = new List<D2OClassDefinition>(classDefCount);
            for (var i = 0; i < classDefCount; i++)
                unpacked.Add(
                    _serdes.Deserialize<D2OClassDefinition>(reader));

            Unpacked = unpacked;
        }

        private void _skipValuePositions(IReader reader)
        {
            var tablePointerPosition = reader.ReadValue<int>();
            reader.Seek(tablePointerPosition);

            var objectPointersLen = reader.ReadValue<int>();
            for (var i = 0; i < objectPointersLen; i += 8)
                reader.Skip(8); // skip key + valuePosition (int + int)
        }
    }
}
