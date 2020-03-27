using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.OpenNI
{
    public abstract class VideoStream : Combinator<OpenNIWrapper.Device, IplImage>
    {
        const int DefaultFrameRate = 30;
        const bool DefaultMirroring = false;
        static readonly Size DefaultSize = new Size(640, 480);

        readonly OpenNIWrapper.Device.SensorType sensorType;

        public VideoStream(OpenNIWrapper.Device.SensorType sensorType)
            => this.sensorType = sensorType;

        [Description("The format of the image.")]
        public OpenNIWrapper.VideoMode.PixelFormat PixelFormat { get; set; }

        [Description("The size of the image.")]
        public Size Size { get; set; } = DefaultSize;

        [Description("The frame rate in frames per second.")]
        [DefaultValue(DefaultFrameRate)]
        public int FrameRate { get; set; } = DefaultFrameRate;

        [Description("Mirrors the image when true.")]
        [DefaultValue(DefaultMirroring)]
        public bool Mirroring { get; set; } = DefaultMirroring;

        public override IObservable<IplImage> Process(IObservable<OpenNIWrapper.Device> source)
            => source
                .Select(device => {

                    var stream = device.CreateVideoStream(sensorType);
                    stream.VideoMode = new OpenNIWrapper.VideoMode
                    {
                        DataPixelFormat = PixelFormat,
                        Fps = FrameRate,
                        Resolution = new OpenNIWrapper.Size(this.Size.Width, this.Size.Height),
                    };
                    stream.Mirroring = Mirroring;
                    return stream;
                })
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

                    switch (frame.VideoMode.DataPixelFormat)
                    {
                        case OpenNIWrapper.VideoMode.PixelFormat.Rgb888:
                            {
                                var image = new IplImage(size, IplDepth.U8, 3);
                                var frameHeader = new Mat(size, Depth.U8, 3, frame.Data, frame.DataStrideBytes);
                                CV.Copy(frameHeader, image);
                                return image;
                            }

                        case OpenNIWrapper.VideoMode.PixelFormat.Gray8:
                            {
                                var image = new IplImage(size, IplDepth.U8, 1);
                                var frameHeader = new Mat(size, Depth.U8, 1, frame.Data, frame.DataStrideBytes);
                                CV.Copy(frameHeader, image);
                                return image;
                            }

                        case OpenNIWrapper.VideoMode.PixelFormat.Depth1Mm:
                        case OpenNIWrapper.VideoMode.PixelFormat.Depth100Um:
                        case OpenNIWrapper.VideoMode.PixelFormat.Gray16:
                            {
                                // convert to 1F32 because threshold does not support 1U16
                                var image = new IplImage(size, IplDepth.F32, 1);
                                var frameHeader = new Mat(size, Depth.U16, 1, frame.Data, frame.DataStrideBytes);
                                CV.Convert(frameHeader, image);
                                return image;
                            }

                        default:
                            throw new InvalidOperationException("Pixel format is not acceptable for bitmap converting.");
                    }
                });

        public bool ShouldSerializeSize()
            => Size != DefaultSize;

        public void ResetSize()
            => Size = DefaultSize;
    }
}
