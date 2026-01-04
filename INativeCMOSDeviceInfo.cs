using System.Collections.Generic;

namespace TianWen.DAL
{
    public interface INativeCMOSDeviceInfo : INativeDeviceInfo
    {
        /// <summary>
        /// Max height of the camera
        /// </summary>
        int MaxHeight { get; }

        /// <summary>
        /// Max width of the camera
        /// </summary>
        int MaxWidth { get; }

        /// <summary>
        /// Maximum bit depth in bits.
        /// </summary>
        int BitDepth { get; }

        /// <summary>
        /// Pixel size in micro-meters.
        /// </summary>
        double PixelSize { get; }

        bool IsTriggerCamera { get; }

        bool HasMechanicalShutter { get; }

        bool HasST4Port { get; }

        double ElectronPerADU { get; }

        IReadOnlyList<int> SupportedBins { get; }

        BayerPattern BayerPattern { get; }
    }
}