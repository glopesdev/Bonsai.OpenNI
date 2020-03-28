using System.ComponentModel;

namespace Bonsai.OpenNI
{
    [Description("Subscribes to the native stream of depth images from a OpenNI device.")]
    public class DepthStream : VideoStream
    {
        const OpenNIWrapper.VideoMode.PixelFormat DefaultPixelFormat = OpenNIWrapper.VideoMode.PixelFormat.Depth1Mm;

        public DepthStream()
            : base(OpenNIWrapper.Device.SensorType.Depth)
        {
        }

        [DefaultValue(DefaultPixelFormat)]
        public override OpenNIWrapper.VideoMode.PixelFormat PixelFormat { get; set; } = DefaultPixelFormat;
    }
}
