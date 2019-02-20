using Andromeda.Binary.Reader;

namespace Andromeda.Data.Parsing.Helpers
{
    public static class ReaderExtensions
    {
        public static void SkipString(this IReader reader) => reader.Skip(reader.ReadValue<short>());
    }
}
