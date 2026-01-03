using System.Collections.Generic;

namespace TianWen.DAL
{
    public interface INativeDeviceIterator<TDeviceInfo, TSerialNumber> : IEnumerable<(int DeviceId, TDeviceInfo DeviceInfo)>
        where TDeviceInfo : INativeDeviceInfo<TSerialNumber>
        where TSerialNumber : struct
    {
    }
}
