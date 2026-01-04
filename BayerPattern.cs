using System;

namespace TianWen.DAL
{
    public enum BayerPattern : ulong
    {
        // TYPE
        Monochrome = 0b00,
        R = 0b001,
        G = 0b010,
        B = 0b011,
        L = 0b100, // Luminance (Kodak Truesense)
        E = 0b101, // Emerald

        RGGB = R << 9 | G << 6 | G << 3 | B,
        BGGR = B << 9 | G << 6 | G << 3 | R,
        GBRG = G << 9 | B << 6 | R << 3 | G,
        GRBG = G << 9 | R << 6 | B << 3 | G,

        /// <summary>
        /// Native BIN1 pattern of Sony IMX294 sensors (QHY294C-PRO).
        /// </summary>
        RRGG_RRGG_GGBB_GGBB =
            R << 45 | R << 42 | G << 39 | G << 36 |
            R << 33 | R << 30 | G << 27 | G << 24 |
            G << 21 | G << 18 | B << 15 | B << 12 |
            G <<  9 | G <<  6 | B <<  3 | B
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