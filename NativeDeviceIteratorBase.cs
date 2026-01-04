using System.Collections;
using System.Collections.Generic;

namespace TianWen.DAL
{
    public abstract class NativeDeviceIteratorBase<TDeviceInfo> : INativeDeviceIterator<TDeviceInfo>
        where TDeviceInfo : struct, INativeDeviceInfo
    {
        public IEnumerator<TDeviceInfo> GetEnumerator()
        {
            var count = DeviceCount();

            for (var index = 0; index < count; index++)
            {
                var info = GetDeviceInfo(index);
                if (info.HasValue)
                {
                    yield return info.Value;
                }
            }
        }

        protected abstract int DeviceCount();

        protected abstract TDeviceInfo? GetDeviceInfo(int index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}