using System.ComponentModel;

namespace Bonsai.OpenNI
{
    [Description("Subscribes to the native stream of infrared images from a OpenNI device.")]
    public class InfraredStream : VideoStream
    {
        const OpenNIWrapper.VideoMode.PixelFormat DefaultPixelFormat = OpenNIWrapper.VideoMode.PixelFormat.Gray8;

        public InfraredStream()
            : base(OpenNIWrapper.Device.SensorType.Ir)
        {
        }

        [DefaultValue(DefaultPixelFormat)]
        public override OpenNIWrapper.VideoMode.PixelFormat PixelFormat { get; set; } = DefaultPixelFormat;
    }
}
