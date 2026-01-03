using System;

namespace TianWen.DAL
{
    public interface INativeDeviceInfo<TSerialNumber>
        where TSerialNumber : struct
    {
        int ID { get; }

        string Name { get; }

        /// <summary>
        /// Custom ID is the same as <see cref="Name"/> except for USB 3 cameras.
        /// </summary>
        string CustomId { get; }

        bool Open();

        bool Close();

        TSerialNumber? SerialNumber { get; }

        bool IsUSB3Device { get; }
    }
}
