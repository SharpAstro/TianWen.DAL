using System;

namespace TianWen.DAL
{
    [Flags]
    public enum BayerPattern
    {
        Monochrome = 0,
        HasBayerMatrix = 1,
        RGGB = 0b0000_0010 | HasBayerMatrix,
        BGGR = 0b0000_0100 | HasBayerMatrix,
        GBRG = 0b0000_1000 | HasBayerMatrix,
        GRBG = 0b0001_0000 | HasBayerMatrix,
    }

    public static class BayerPatternEx
    {
        public static (int X, int Y) GetOffsets(this BayerPattern @this)
        {
            switch (@this)
            {
                case BayerPattern.RGGB: return (0, 0);
                case BayerPattern.BGGR: return (1, 0);
                case BayerPattern.GBRG: return (0, 1);
                case BayerPattern.GRBG: return (1, 1);
                default:
                    throw new NotSupportedException($"Unknown Bayer pattern {@this}");
            }
        }
    }
}