using System;
using System.Collections.Generic;

namespace TianWen.DAL
{
    /// <summary>
    /// What a CMOS body can be asked and told, grouped by subject: what the sensor IS, what the body
    /// CAN do, its controls, guiding, exposure, region of interest, and the frame itself.
    /// </summary>
    /// <remarks>
    /// The grouping is for readers only. Member ORDER in an interface carries no binary meaning, so
    /// unlike <see cref="CMOSControlType"/> this can be rearranged at any version; members added in
    /// 2.1 and 3.0 had simply landed at the end, away from the ones they belong with.
    /// </remarks>
    public interface ICMOSNativeInterface : INativeDeviceInfo
    {
        // ---- What the sensor is -------------------------------------------------------------

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

        BayerPattern BayerPattern { get; }

        IReadOnlyList<int> SupportedBins { get; }

        IReadOnlyList<PixelDataFormat> SupportedPixelDataFormats { get; }

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

        // ---- What the body can do -----------------------------------------------------------

        bool IsTriggerCamera { get; }

        bool HasMechanicalShutter { get; }

        bool HasCooler { get; }

        bool HasST4Port { get; }

        /// <summary>
        /// True when the white balance has a GREEN channel of its own
        /// (<see cref="CMOSControlType.WB_G"/>) rather than red and blue against an implicit green.
        /// </summary>
        /// <remarks>
        /// Not cosmetic. A caller setting a neutral white balance has to write every channel the
        /// body has, and writing two of three leaves the third wherever it was last put.
        /// </remarks>
        bool HasThreeChannelWhiteBalance => false;

        // ---- Controls -----------------------------------------------------------------------

        double ElectronPerADU { get; }

        bool TryGetControlRange(CMOSControlType ctrlType, out int min, out int max);

        /// <summary>
        /// The white balance scale this body uses: its bounds and, crucially, the value on it that
        /// applies NO gain. False when the body has no white balance at all, which is every mono
        /// camera.
        /// </summary>
        /// <remarks>
        /// <para><b>Neutral is not the same number on two vendors, and it is not the SDK's declared
        /// DEFAULT either.</b> ZWO's channels run about [1, 99] with unity at the midpoint 50;
        /// Player One's run [-1200, 1200] with unity at 0. Both are measured rather than assumed: a
        /// ZWO frame captured at 65 carries a red gain of 1.312 in its pixels, which is 65/50, and a
        /// Player One frame captured at 0 measures exactly 1.000. A vendor's default is a third
        /// thing again, being a pleasant daylight balance rather than no balance at all.</para>
        /// <para><b>This exists because one constant was being written to every vendor.</b> The
        /// driver set 50 on connect whatever the body was, which is unity on a ZWO and a red and
        /// blue lift on a Player One. It went unnoticed because NO header records a white balance:
        /// it is baked into the pixels, so the only way to see it after the fact is to measure the
        /// per-photosite quantisation step, and the only way to know it in advance is to ask the
        /// camera. This is the asking.</para>
        /// <para>A white balance is a digital gain on the RAW stream, so it scales the pedestal a
        /// dark frame has to match. Getting it wrong does not merely tint a preview; it makes a
        /// calibration library disagree with the lights it was shot for.</para>
        /// </remarks>
        /// <param name="min">Lowest settable value.</param>
        /// <param name="max">Highest settable value.</param>
        /// <param name="neutral">The value applying no gain, which a caller writes to leave the raw
        /// stream alone.</param>
        bool TryGetWhiteBalanceRange(out int min, out int max, out int neutral)
        {
            min = max = neutral = 0;
            return false;
        }

        CMOSErrorCode GetControlValue(CMOSControlType controlType, out int value, out bool isAuto);

        CMOSErrorCode SetControlValue(CMOSControlType controlType, int value, bool isAuto = false);

        // ---- Guiding ------------------------------------------------------------------------

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

        // ---- Exposure -----------------------------------------------------------------------

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

        // ---- Region of interest -------------------------------------------------------------

        CMOSErrorCode GetStartPosition(out int startX, out int startY);

        CMOSErrorCode SetStartPosition(int startX, int startY);

        CMOSErrorCode GetROIFormat(out int width, out int height, out int bin, out PixelDataFormat pixelDataFormat);

        CMOSErrorCode SetROIFormat(int width, int height, int bin, PixelDataFormat pixelDataFormat);

        // ---- The frame ----------------------------------------------------------------------

        CMOSErrorCode GetDataAfterExposure(IntPtr buffer, int bufferSize);
    }
}
