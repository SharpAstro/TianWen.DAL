namespace TianWen.DAL
{
    public interface INativeDeviceInfo
    {
        int ID { get; }

        string Name { get; }

        /// <summary>
        /// Custom ID is the same as <see cref="Name"/> except for USB 3 cameras.
        /// </summary>
        string CustomId { get; }

        bool Open();

        bool Close();

        /// <summary>
        /// The factory serial, or null when the device has none programmed. A vendor SDK reports a missing
        /// serial as an error code or as an all-zero pattern, and both are surfaced as null rather than as
        /// a string a caller could mistake for an identity.
        /// </summary>
        string? SerialNumber { get; }

        bool IsUSB3Device { get; }

        /// <summary>
        /// Sensor die model name, e.g. "IMX533" or "KAF-8300".
        /// Returns null when the model cannot be determined from the camera name.
        /// Default implementation returns null — SDK wrappers override this.
        /// </summary>
        string? SensorModel => null;

        /// <summary>
        /// True when the device can be RESET from software, as if it had been unplugged and plugged in
        /// again (<see cref="ResetDevice"/>). False by default, which is every SDK that offers no such call.
        /// </summary>
        bool CanResetDevice => false;

        /// <summary>
        /// Resets the device as a replug would, for a device that has stopped answering.
        /// </summary>
        /// <remarks>
        /// <para><b>This exists because a wedged device used to mean a person at the cable.</b> Measured on
        /// a ToupTek G3M678M 2026-09-23: in video mode the camera can stop delivering frames partway
        /// through a stream (163 frames, then nothing, with every call still succeeding), and stays that
        /// way across close and reopen. Its SDK's own "simulate a replug" brought it back in seconds.
        /// Nothing in this interface could express that, so the only recovery was physical.</para>
        /// <para><b>After a successful reset the device is CLOSED and has forgotten everything.</b> Every
        /// handle this instance held is invalid, the device re-enumerates, and every setting (region of
        /// interest, gain, offset, a cooler's target) is back at the device's power-on default. A caller
        /// therefore has to enumerate, <see cref="Open"/> and re-apply its settings, which is why this is a
        /// capability a DRIVER invokes deliberately, as a rung in its recovery, and never something a
        /// binding does on its own behind the caller: on a cooled camera a reset is a thermal event too.</para>
        /// <para>Default <see cref="CMOSErrorCode.GeneralError"/>, matching <see cref="CanResetDevice"/>
        /// false, so an implementation that has not been taught this refuses rather than pretends.</para>
        /// </remarks>
        CMOSErrorCode ResetDevice() => CMOSErrorCode.GeneralError;
    }
}
