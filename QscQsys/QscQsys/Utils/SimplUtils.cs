using System;

namespace QscQsys.Utils
{
    public static class SimplUtils
    {
        public static ushort BoolToSplus(this bool extends)
        {
            return (ushort)(extends ? 1 : 0);
        }

        public static bool BoolFromSplus(this ushort extends)
        {
            return extends > 0;
        }

        /// <summary>
        /// Scales the given level from input of 0 - 1
        /// to ouptput of  0 - 65535
        /// for use going to SimplWindows.
        /// </summary>
        /// <param name="level"></param>
        /// <exception cref="ArgumentOutOfRangeException">If level is below 0 or above 1</exception>
        /// <returns></returns>
        public static ushort ScaleToUshort(double level)
        {
            if (level > 1 || level < 0)
                throw new ArgumentOutOfRangeException("level", "level must be beween 0 and 1");
            
            return (ushort)(level * ushort.MaxValue);
        }

        /// <summary>
        /// Scales the given level from input of 0 - 65535
        /// to ouptput of  0 - 1
        /// for use coming from SimplWindows.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static double ScaleToDouble(ushort level)
        {
            return ((double)level) / ushort.MaxValue;
        }

        /// <summary>
        /// Scales the given level from input of 0 - 65535
        /// to ouptput of  0 - 1
        /// for use coming from SimplWindows.
        /// </summary>
        /// <param name="level"></param>
        /// <exception cref="ArgumentOutOfRangeException">If level is below 0 or above 65535</exception>
        /// <returns></returns>
        public static double ScaleToDouble(int level)
        {
            if (level < ushort.MinValue || level > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("level", "level must be between ushort.MinValue and ushort.MaxValue");

            return ScaleToDouble((ushort)level);
        }
    }
}