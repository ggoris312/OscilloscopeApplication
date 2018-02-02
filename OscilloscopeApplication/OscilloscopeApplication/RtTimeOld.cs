#region

using System.Runtime.InteropServices;

#endregion

namespace OscilloscopeConnection
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct RtTimeOld
    {
        public double seconds;
        public char minutes;
        public char hours;
        public char days;
        public char months;
        public short year;
        public short dummy;
    }
}