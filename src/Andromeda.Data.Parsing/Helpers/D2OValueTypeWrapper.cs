using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;
using Andromeda.Binary.Reader;
using Andromeda.Data.Parsing.D2O;
using Andromeda.Data.Parsing.D2O.Enums;

namespace Andromeda.Data.Parsing.Helpers
{
    public static class D2OValueTypeWrapper
    {
        private static readonly Dictionary<D2OFieldValueType, Type> PrimitivesTypes =
            new Dictionary<D2OFieldValueType, Type>(6)
            {
                {D2OFieldValueType.Bool, typeof(bool)},
                {D2OFieldValueType.Int, typeof(int)},
                {D2OFieldValueType.UInt, typeof(uint)},
                {D2OFieldValueType.Double, typeof(double)},
                {D2OFieldValueType.I18N, typeof(int)},
                {D2OFieldValueType.String, typeof(string)}
            };

        private static readonly Dictionary<D2OFieldValueType, string> PrimitivesString =
            new Dictionary<D2OFieldValueType, string>(6)
            {
                {D2OFieldValueType.Bool, "bool"},
                {D2OFieldValueType.Int, "int"},
                {D2OFieldValueType.UInt, "uint"},
                {D2OFieldValueType.Double, "double"},
                {D2OFieldValueType.I18N, "int"},
                {D2OFieldValueType.String, "string"}
            };

        private static readonly Dictionary<D2OFieldValueType, Func<IReader, object>> PrimitivesReadMethods =
            new Dictionary<D2OFieldValueType, Func<IReader, object>>(6)
            {
                {D2OFieldValueType.Bool, r => r.ReadValue<bool>()},
                {D2OFieldValueType.Int, r => r.ReadValue<int>()},
                {D2OFieldValueType.UInt, r => r.ReadValue<uint>()},
                {D2OFieldValueType.Double, r => r.ReadValue<double>()},
                {D2OFieldValueType.I18N, r => r.ReadValue<int>()},
                {D2OFieldValueType.String, r => r.ReadValue<string>()}
            };


        public static bool IsPrimitiveField(this D2OFieldValueType type)
            => PrimitivesTypes.ContainsKey(type);
        public static Type GetPrimitiveFieldType(this D2OFieldValueType type) => PrimitivesTypes[type];
        public static string GetPrimitiveFieldString(this D2OFieldValueType type) => PrimitivesString[type];
        public static object ReadPrimitive(IReader reader, D2OFieldValueType primitiveType) =>
            PrimitivesReadMethods[primitiveType](reader);
    }
}
