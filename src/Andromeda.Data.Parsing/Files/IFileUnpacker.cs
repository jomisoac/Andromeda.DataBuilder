using System.Collections.Generic;
using Andromeda.Binary.Reader;

namespace Andromeda.Data.Parsing.Files
{
    public interface IFileUnpacker<out T>
    {
        IReadOnlyCollection<T> Unpacked { get; }
        void Unpack(IReader reader);
    }
}
