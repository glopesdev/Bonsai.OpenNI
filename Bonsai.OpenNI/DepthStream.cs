using System.ComponentModel;

namespace Bonsai.OpenNI
{
    [Description("Subscribes to the native stream of depth images from a OpenNI device.")]
    public class DepthStream : VideoStream
    {
        public DepthStream()
            : base(OpenNIWrapper.Device.SensorType.Depth)
        {
            PixelFormat = OpenNIWrapper.VideoMode.PixelFormat.Depth1Mm;
        }

    }
}
