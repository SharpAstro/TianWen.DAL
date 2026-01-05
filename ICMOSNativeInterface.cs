using System;
using System.Collections.Generic;

namespace TianWen.DAL
{
    public interface ICMOSNativeInterface : INativeDeviceInfo
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

        bool HasCooler { get; }

        bool HasST4Port { get; }

        double ElectronPerADU { get; }

        IReadOnlyList<int> SupportedBins { get; }

        IReadOnlyList<PixelDataFormat> SupportedPixelDataFormats { get; }

        BayerPattern BayerPattern { get; }

        bool TryGetControlRange(CMOSControlType ctrlType, out int min, out int max);

        CMOSErrorCode GetControlValue(CMOSControlType controlType, out int value, out bool isAuto);

        CMOSErrorCode SetControlValue(CMOSControlType controlType, int value, bool isAuto = false);

        CMOSErrorCode PulseGuideOn(GuideDirection direction);

        CMOSErrorCode PulseGuideOff(GuideDirection direction);

        /// <summary>
        /// Starts an exposure with an open mechanical shutter <see cref="HasMechanicalShutter"/> (i.e. a light exposure).
        /// </summary>
        /// <returns><see cref="CMOSErrorCode.Success"/> if exposure was started successfully.</returns>
        CMOSErrorCode StartLightExposure();

        /// <summary>
        /// Starts an exposure with a closed mechanical shutter <see cref="HasMechanicalShutter"/> (i.e. a dark exposure).
        /// </summary>
        /// <returns><see cref="CMOSErrorCode.Success"/> if exposure was started successfully.</returns>
        CMOSErrorCode StartDarkExposure();

        CMOSErrorCode StopExposure();

        CMOSErrorCode GetExposureStatus(out ExposureStatus exposureStatus);

        CMOSErrorCode GetStartPosition(out int startX, out int startY);

        CMOSErrorCode SetStartPosition(int startX, int startY);

        CMOSErrorCode GetROIFormat(out int width, out int height, out int bin, out PixelDataFormat pixelDataFormat);

        CMOSErrorCode SetROIFormat(int width, int height, int bin, PixelDataFormat pixelDataFormat);

        CMOSErrorCode GetDataAfterExposure(IntPtr buffer, int bufferSize);
    }
}