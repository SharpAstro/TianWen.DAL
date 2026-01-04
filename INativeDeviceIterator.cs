using System.Collections.Generic;

namespace TianWen.DAL
{
    public interface INativeDeviceIterator<TDeviceInfo> : IEnumerable<TDeviceInfo>
        where TDeviceInfo : INativeDeviceInfo
    {
    }
}
