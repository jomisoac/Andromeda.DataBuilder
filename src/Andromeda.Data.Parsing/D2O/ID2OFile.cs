using System;
using System.Collections.Generic;
using System.Text;
using Andromeda.Data.Parsing.D2O.Models;
using Andromeda.Data.Parsing.Files;

namespace Andromeda.Data.Parsing.D2O
{
    public interface ID2OFile
    {
        IFileAccessor File { get; }
        IFileUnpacker<D2OClass> ClassValues { get; }
        IFileUnpacker<D2OClassDefinition> LocalClassDefinitions { get; }
    }
}
