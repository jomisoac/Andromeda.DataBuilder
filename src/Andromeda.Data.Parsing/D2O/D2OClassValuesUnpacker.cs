using System;
using System.Collections.Generic;
using System.Text;
using Andromeda.Binary.Reader;
using Andromeda.Data.Parsing.D2O.Models;
using Andromeda.Data.Parsing.Files;

namespace Andromeda.Data.Parsing.D2O
{
    public class D2OClassValuesUnpacker : IFileUnpacker<D2OClass>
    {
        private IEnumerable<D2OClassDefinition> _localClassDefinitions;
        public IReadOnlyCollection<D2OClass> Unpacked { get; }

        public D2OClassValuesUnpacker(IEnumerable<D2OClassDefinition> localClassDefinitions)
        {
            _localClassDefinitions = localClassDefinitions;
        }

        public void Unpack(IReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
