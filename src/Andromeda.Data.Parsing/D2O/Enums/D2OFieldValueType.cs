using System;

namespace Andromeda.Data.Parsing.D2O.Enums
{
    [Flags]
    public enum D2OFieldValueType
    {
        Int = -1,
        Bool = -2,
        String = -3,
        Double = -4,
        I18N = -5,
        UInt = -6,
        Vector = -99,
        VectorOfVector = -100
    }
}
