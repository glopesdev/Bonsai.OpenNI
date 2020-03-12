using System.ComponentModel;

namespace Bonsai.OpenNI
{
    [Description("Subscribes to the native stream of depth images in a OpenNI device.")]
    public class DepthStream : Stream
    {
        public DepthStream()
            : base(OpenNIWrapper.Device.SensorType.Depth)
        {
        }

    }
}
