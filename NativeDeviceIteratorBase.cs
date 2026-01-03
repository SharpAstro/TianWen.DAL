using System.Collections;
using System.Collections.Generic;

namespace TianWen.DAL
{
    public abstract class NativeDeviceIteratorBase<TDeviceInfo, TSerialNumber> : INativeDeviceIterator<INativeDeviceInfo<TSerialNumber>, TSerialNumber>
        where TDeviceInfo : struct, INativeDeviceInfo<TSerialNumber>
        where TSerialNumber : struct
    {
        public IEnumerator<(int DeviceId, INativeDeviceInfo<TSerialNumber> DeviceInfo)> GetEnumerator()
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
