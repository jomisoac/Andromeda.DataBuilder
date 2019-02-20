using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Andromeda.Binary.Reader;
using Andromeda.Binary.Storage;
using Andromeda.Binary.Writer;

namespace Andromeda.Data.Parsing.D2O.Storage
{
    public class Utf8BinaryStorage : IBinaryStorage<string>
    {
        public void WriteValue(IWriter writer, string value, out int writtenBytes)
        {
            writtenBytes = 0;
            var encodedStr = Encoding.UTF8.GetBytes(value);
            if(writer.BytesAvailable < encodedStr.Length + sizeof(short))
                throw new InternalBufferOverflowException("Not enough byte to write in the buffer");

            writer.WriteValue((short)value.Length);
            writer.WriteValues(encodedStr);
        }

        public string ReadValue(IReader reader, out int bytesRead)
        {
            bytesRead = 0;
            var strLen = reader.ReadValue<short>();
            if(reader.BytesAvailable < strLen)
                throw new InternalBufferOverflowException("Not enough byte to read in the buffer");

            var encodedStr = reader.ReadValues<byte>(strLen);
            return Encoding.UTF8.GetString(encodedStr);
        }
    }
}
