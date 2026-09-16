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
        /// True when the device times a guide pulse ITSELF, so a caller states the duration up front
        /// and never has to stop it.
        /// </summary>
        /// <remarks>
        /// False by default, meaning the caller must run its own timer and call
        /// <see cref="PulseGuideOff(GuideDirection)"/> to end the pulse.
        /// </remarks>
        bool CanPulseGuideForDuration => false;

        /// <summary>
        /// Starts a guide pulse of a stated duration, timed by the DEVICE. Only valid when
        /// <see cref="CanPulseGuideForDuration"/> is true.
        /// </summary>
        /// <remarks>
        /// <para><b>This exists because the untimed pair could not express what the hardware
        /// offers, and the gap was silently destructive.</b> QHY's own entry point takes a duration
        /// in milliseconds, but with nowhere to put it the binding passed a hardcoded constant and
        /// implemented <see cref="PulseGuideOff(GuideDirection)"/> as a no-op returning success. So
        /// every correction ran for the constant instead of the requested time, and the caller's
        /// stop had no effect: a 500 ms nudge became a 50 second one that nothing could halt.</para>
        /// <para>A device answering false is not deficient, it simply needs the caller's timer; both
        /// shapes are legitimate and the capability is what tells them apart.</para>
        /// </remarks>
        /// <param name="direction">Axis and sign to pulse.</param>
        /// <param name="duration">How long the device should assert it.</param>
        CMOSErrorCode PulseGuideOn(GuideDirection direction, TimeSpan duration)
            => CMOSErrorCode.GeneralError;

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

        /// <summary>
        /// The sub-rectangle of the FULL readout that is exposed to light, in unbinned photosites
        /// measured from the readout's own origin. False when the sensor does not declare one, which
        /// is also the right answer for a sensor whose readout is entirely lit.
        /// </summary>
        /// <remarks>
        /// <para>The decoded raster is not necessarily the photograph. Many sensors read out shielded
        /// rows or columns beside the picture (the detector's own black reference), and a consumer
        /// that treats the whole raster as image puts a strip pinned near the black level into every
        /// statistic taken over it. This is the same distinction Canon raws already make through
        /// <c>CanonRawFile.ActiveArea</c>, and the same one the IRAF <c>DATASEC</c> card states on
        /// disk; it had no expression here at all, so a native camera could not report it.</para>
        /// <para>Default false so an implementation that has not been taught this is INDISTINGUISHABLE
        /// from a sensor with nothing to declare: both mean "treat the whole raster as the picture",
        /// which is the behaviour every caller had before this existed.</para>
        /// </remarks>
        bool TryGetEffectiveArea(out int startX, out int startY, out int width, out int height)
        {
            startX = startY = width = height = 0;
            return false;
        }

        /// <summary>
        /// The shielded sub-rectangle of the full readout, in unbinned photosites from the readout's
        /// origin. False when the sensor declares none, which many do.
        /// </summary>
        /// <remarks>
        /// This is a per-frame BIAS reference, not waste: it is the only part of a frame that says
        /// what the black level was during THAT exposure, which is exactly what a body whose level
        /// wanders between exposures needs. Do NOT assume it lies outside
        /// <see cref="TryGetEffectiveArea"/>; QHY bodies have been observed reporting an overscan
        /// INSIDE the effective area, so a caller must compare the two rather than trusting that
        /// cropping to the effective area preserves the reference.
        /// </remarks>
        bool TryGetOverscanArea(out int startX, out int startY, out int width, out int height)
        {
            startX = startY = width = height = 0;
            return false;
        }
    }
}