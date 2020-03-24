using System.ComponentModel;

namespace Bonsai.OpenNI
{
    [Description("Subscribes to the native stream of color images from a OpenNI device.")]
    public class ColorStream : VideoStream
    {
        public ColorStream()
            : base(OpenNIWrapper.Device.SensorType.Color)
        {
            PixelFormat = OpenNIWrapper.VideoMode.PixelFormat.Rgb888;
        }

    }
}
