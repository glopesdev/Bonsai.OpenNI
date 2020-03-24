using System.ComponentModel;

namespace Bonsai.OpenNI
{
    [Description("Subscribes to the native stream of infrared images from a OpenNI device.")]
    public class InfraredStream : VideoStream
    {
        public InfraredStream()
            : base(OpenNIWrapper.Device.SensorType.Ir)
        {
            PixelFormat = OpenNIWrapper.VideoMode.PixelFormat.Gray8;
        }

    }
}
