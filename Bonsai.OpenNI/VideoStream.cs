using OpenCV.Net;
using System;
using System.Reactive.Linq;

namespace Bonsai.OpenNI
{
    public abstract class VideoStream : Combinator<OpenNIWrapper.Device, IplImage>
    {
        readonly OpenNIWrapper.Device.SensorType sensorType;

        public VideoStream(OpenNIWrapper.Device.SensorType sensorType)
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
                    var image = (IplImage)null;
                    var frameHeader = (Mat)null;

                    switch (frame.VideoMode.DataPixelFormat)
                    {
                        case OpenNIWrapper.VideoMode.PixelFormat.Rgb888:
                            image = new IplImage(size, IplDepth.U8, 3);
                            frameHeader = new Mat(size, Depth.U8, 3, frame.Data, frame.DataStrideBytes);
                            break;

                        case OpenNIWrapper.VideoMode.PixelFormat.Gray8:
                            image = new IplImage(size, IplDepth.U8, 1);
                            frameHeader = new Mat(size, Depth.U8, 1, frame.Data, frame.DataStrideBytes);
                            break;

                        case OpenNIWrapper.VideoMode.PixelFormat.Depth1Mm:
                        case OpenNIWrapper.VideoMode.PixelFormat.Depth100Um:
                        case OpenNIWrapper.VideoMode.PixelFormat.Gray16:
                            image = new IplImage(size, IplDepth.U16, 1);
                            frameHeader = new Mat(size, Depth.U16, 1, frame.Data, frame.DataStrideBytes);
                            break;

                        default:
                            throw new InvalidOperationException("Pixel format is not acceptable for bitmap converting.");
                    }

                    CV.Copy(frameHeader, image);
                    return image;
                });
    }
}
