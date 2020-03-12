using OpenCV.Net;
using System;
using System.Reactive.Linq;

namespace Bonsai.OpenNI
{
    public abstract class Stream : Combinator<OpenNIWrapper.Device, IplImage>
    {
        readonly OpenNIWrapper.Device.SensorType sensorType;

        public Stream(OpenNIWrapper.Device.SensorType sensorType)
            => this.sensorType = sensorType;

        public override IObservable<IplImage> Process(IObservable<OpenNIWrapper.Device> source)
            => source
                .Select(device => device.CreateVideoStream(sensorType))
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
