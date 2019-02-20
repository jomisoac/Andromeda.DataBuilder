using System;
using System.Collections.Generic;
using System.Text;
using Andromeda.Binary;
using Andromeda.Data.Parsing.D2O.Storage;
using Andromeda.IoC;
using Andromeda.Logging;
using Andromeda.Serialization;

namespace Andromeda.DataBuilder
{
    internal static class Startup
    {
        public static IContainer Configure(IContainerBuilder builder, AppConstants constants)
        {
            var loggerBuilder = new Logger.Builder()
                .AddStrategy(new ConsoleOutputStrategy())
                .AddStrategy(new FileStrategy($"log-{DateTime.Now:dd-MM-yyyy-hh_mm_ss}.txt"));

            var binaryBuilder = new BinaryBuilder(Endianness.BigEndian)
                .Register(new Utf8BinaryStorage());

            var serdesBuilder = new SerDesBuilder()
                .Register(new ClassDefDeserializerStorage())
                .Register(new FieldDefDeserializerStorage());

            builder.Register(constants);
            builder
                .Register(binaryBuilder)
                .Register((args, ctr) => ctr.GetInstance<IBinaryBuilder>().Build((byte[])args[0]));

            builder
                .Register(loggerBuilder.Build())
                .Register(serdesBuilder.Build());

            var container = builder.Build();
            ServiceLocator.Container = container;
            return container;
        }
    }
}
