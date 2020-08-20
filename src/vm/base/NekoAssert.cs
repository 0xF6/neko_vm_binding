﻿namespace Neko.Base
{
    using Base;
    using NativeRing;

    internal static unsafe class NekoAssert
    {
        public static void IsFunction(NekoValue* value, string name)
        {
            if (!NekoType.is_function(value))
                throw new IsNotAFunctionNekoException(name);
        }
    }
}