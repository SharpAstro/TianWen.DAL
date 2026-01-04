using System.Collections.Generic;

namespace TianWen.DAL
{
    public interface INativeDeviceIterator<TDeviceInfo> : IEnumerable<(int DeviceId, TDeviceInfo DeviceInfo)>
        where TDeviceInfo : INativeDeviceInfo
    {
    }
}
