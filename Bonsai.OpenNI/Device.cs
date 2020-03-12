using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.OpenNI
{
    [Description("Creates and connects to an OpenNI device.")]
    public class Device : Source<DeviceInstance>
    {
        [Description("The index of the device.")]
        public int Index { get; set; }

        public override IObservable<DeviceInstance> Generate()
            => Observable.Defer(() =>
                {
                    var context = Context.Instance;
                    var devices = OpenNIWrapper.OpenNI.EnumerateDevices();
                    var deviceInfo = devices[Index];
                    var device = deviceInfo.OpenDevice();
                    var deviceInstance = new DeviceInstance(this, device);
                    return Observable
                        .Return(deviceInstance)
                        .Concat(Observable.Never<DeviceInstance>())
                        .Finally(() => device.Close());
                });
    }
}
