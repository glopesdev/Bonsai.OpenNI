using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.OpenNI
{
    [Description("Subscribes to the native stream of depth images in a OpenNI device.")]
    public class DepthStream : Combinator<DeviceInstance, IplImage>
    {
        public override IObservable<IplImage> Process(IObservable<DeviceInstance> source)
            => source
                .Select(device => device.Device.CreateVideoStream(OpenNIWrapper.Device.SensorType.Depth))
                .Where(stream => stream.IsValid && stream.Start() == OpenNIWrapper.OpenNI.Status.Ok)
                .SelectMany(stream =>
                    Observable.FromEvent<OpenNIWrapper.VideoStream.VideoStreamNewFrame, OpenNIWrapper.VideoStream>(
                        handler => stream.OnNewFrame += handler,
                        handler => stream.OnNewFrame -= handler))
                    .Where(stream => stream.IsValid && stream.IsFrameAvailable())
                    .Select(stream =>
                    {
                        using var frame = stream.ReadFrame();
                        var size = new Size(frame.FrameSize.Width, frame.FrameSize.Height);
                        var image = new IplImage(size, IplDepth.U16, 1);
                        var frameHeader = new Mat(size, Depth.U16, 1, frame.Data, frame.DataStrideBytes);
                        CV.Copy(frameHeader, image);
                        return image;
                    });
    }
}
