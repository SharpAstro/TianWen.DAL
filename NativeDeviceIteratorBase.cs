using System.Collections;
using System.Collections.Generic;

namespace TianWen.DAL
{
    public abstract class NativeDeviceIteratorBase<TDeviceInfo> : INativeDeviceIterator<TDeviceInfo>
        where TDeviceInfo : struct, INativeDeviceInfo
    {
        public IEnumerator<(int DeviceId, TDeviceInfo DeviceInfo)> GetEnumerator()
        {
            var count = DeviceCount();

            for (var index = 0; index < count; index++)
            {
                var (id, info) = GetId(index);
                if (id.HasValue && info.HasValue)
                {
                    yield return (id.Value, info.Value);
                }
            }
        }

        protected abstract int DeviceCount();

        protected abstract (int? DeviceId, TDeviceInfo? DeviceInfo) GetId(int index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
