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

        string SerialNumber { get; }

        bool IsUSB3Device { get; }

        /// <summary>
        /// Sensor die model name, e.g. "IMX533" or "KAF-8300".
        /// Returns null when the model cannot be determined from the camera name.
        /// Default implementation returns null — SDK wrappers override this.
        /// </summary>
        string? SensorModel => null;
    }
}
