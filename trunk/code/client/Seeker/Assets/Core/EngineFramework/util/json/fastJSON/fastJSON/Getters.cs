namespace fastJSON
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Getters
    {
        public string Name;
        public Reflection.GenericGetter Getter;
    }
}

