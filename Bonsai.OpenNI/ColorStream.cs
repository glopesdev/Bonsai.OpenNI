using System.ComponentModel;

namespace Bonsai.OpenNI
{
    [Description("Subscribes to the native stream of color images from a OpenNI device.")]
    public class ColorStream : VideoStream
    {
        const OpenNIWrapper.VideoMode.PixelFormat DefaultPixelFormat = OpenNIWrapper.VideoMode.PixelFormat.Rgb888;

        public ColorStream()
            : base(OpenNIWrapper.Device.SensorType.Color)
        {
        }

        [DefaultValue(DefaultPixelFormat)]
        public override OpenNIWrapper.VideoMode.PixelFormat PixelFormat { get; set; } = DefaultPixelFormat;
    }
}
