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
}