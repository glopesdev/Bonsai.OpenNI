using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.OpenNI
{
    [DefaultProperty("Index")]
    [Description("Creates and connects to an OpenNI device.")]
    public class Device : Source<OpenNIWrapper.Device>
    {
        [Description("The index of the device.")]
        public int Index { get; set; }

        public override IObservable<OpenNIWrapper.Device> Generate()
            => Observable.Defer(() =>
                {
                    var context = Context.Instance;
                    var devices = OpenNIWrapper.OpenNI.EnumerateDevices();
                    var deviceInfo = devices[Index];
                    var device = deviceInfo.OpenDevice();
                    return Observable
                        .Return(device)
                        .Concat(Observable.Never<OpenNIWrapper.Device>())
                        .Finally(() => device.Close());
                });
    }
}
