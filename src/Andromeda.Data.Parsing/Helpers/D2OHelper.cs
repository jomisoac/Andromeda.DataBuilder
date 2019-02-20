using System;
using System.Collections.Generic;
using System.Text;
using Andromeda.Binary.Reader;
using Andromeda.Data.Parsing.D2O;
using Andromeda.Data.Parsing.D2O.Enums;
using Andromeda.Data.Parsing.D2O.Models;

namespace Andromeda.Data.Parsing.Helpers
{
    public static class D2OHelper
    {
        private const string FileHeader = "D2O";
        public static bool TryParseHeader(IReader reader)
            => Encoding.ASCII.GetString(reader.ReadValues<byte>(3)) == FileHeader;
    }
}
