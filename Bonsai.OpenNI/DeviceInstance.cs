using System;
using System.Collections.Generic;
using System.Text;

namespace Bonsai.OpenNI
{
    public class DeviceInstance
    {
        public DeviceInstance(Device source, OpenNIWrapper.Device device)
        {
            Source = source;
            Device = device;
        }

        public Device Source { get; }
        public OpenNIWrapper.Device Device { get; }
    }
}
