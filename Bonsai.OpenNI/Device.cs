using Bonsai.Reactive;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.OpenNI
{
    [DefaultProperty("Index")]
    [Description("Creates and connects to an OpenNI device.")]
    public class Device : Source<OpenNIWrapper.Device>
    {
        [TypeConverter(typeof(DeviceConverter))]
        [Description("The index of the device.")]
        public int Index { get; set; }

        public override IObservable<OpenNIWrapper.Device> Generate()
            => Observable.Defer(() =>
                {
                    var context = Context.Instance; // initializes OpenNI 

                    var devices = OpenNIWrapper.OpenNI.EnumerateDevices();
                    if (Index < 0 || Index >= devices.Length)
                        return Observable.Throw<OpenNIWrapper.Device>(new IndexOutOfRangeException("Index for OpenNI device is out of range."));

                    var deviceInfo = devices[Index];
                    var device = deviceInfo.OpenDevice();
                    return Observable
                        .Return(device)
                        .Concat(Observable.Never<OpenNIWrapper.Device>())
                        .Finally(() => device.Close());
                });
    }
}
