using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.OpenNI
{
    [DefaultProperty("Index")]
    [Description("A test device.")]
    public class TestDevice : Source<int>
    {
        [Description("The index of the device.")]
        public int Index { get; set; }

        public override IObservable<int> Generate()
            => Observable.Defer(() =>
                {
                    return Observable
                        .Return(42)
                        .Concat(Observable.Never<int>());
                });
    }
}
