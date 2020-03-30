using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.OpenNI
{
    public class VideoStream : Combinator<OpenNIWrapper.Device, IplImage>
    {
        const OpenNIWrapper.Device.SensorType DefaulSensorType = OpenNIWrapper.Device.SensorType.Depth;
        const bool DefaultMirroring = false;
        const OpenNIWrapper.VideoMode.PixelFormat DefaulPixelFormat = OpenNIWrapper.VideoMode.PixelFormat.Depth1Mm;
        static readonly Size DefaultSize = new Size(640, 480);
        const int DefaultFrameRate = 30;

        [Description("The sensor type.")]
        [DefaultValue(DefaulSensorType)]
        public OpenNIWrapper.Device.SensorType SensorType { get; set; } = DefaulSensorType;

        [Description("Mirrors the image when true.")]
        [DefaultValue(DefaultMirroring)]
        public bool Mirroring { get; set; } = DefaultMirroring;

        [Category("Video Mode")]
        [Description("The pixel format.")]
        [DefaultValue(DefaulPixelFormat)]
        public OpenNIWrapper.VideoMode.PixelFormat PixelFormat { get; set; } = DefaulPixelFormat;

        [Category("Video Mode")]
        [Description("The size of the image.")]
        public Size Size { get; set; } = DefaultSize;

        [Category("Video Mode")]
        [Description("The frame rate in frames per second.")]
        [DefaultValue(DefaultFrameRate)]
        public int FrameRate { get; set; } = DefaultFrameRate;

        //[Category("Camera Settings")]
        //public bool AutoExposure { get; set; }

        //[Category("Camera Settings")]
        //public bool AutoWhiteBalance { get; set; }

        //[Category("Camera Settings")]
        //public int Exposure { get; set; }

        //[Category("Camera Settings")]
        //public int Gain { get; set; }

        public override IObservable<IplImage> Process(IObservable<OpenNIWrapper.Device> source)
            => source
                .SelectMany(device =>
                {
                    var stream = device.CreateVideoStream(SensorType);
                    stream.Mirroring = Mirroring;
                    try
                    {
                        stream.VideoMode = new OpenNIWrapper.VideoMode
                        {
                            DataPixelFormat = PixelFormat,
                            Fps = FrameRate,
                            Resolution = new OpenNIWrapper.Size(this.Size.Width, this.Size.Height),
                        };
                    }
                    catch
                    {
                        return Observable.Throw<OpenNIWrapper.VideoStream>(new Exception("Video mode not supported by OpenNI video stream."));
                    }

                    if (!stream.IsValid)
                        return Observable.Throw<OpenNIWrapper.VideoStream>(new Exception("OpenNI video stream is not valid."));

                    if (stream.Start() != OpenNIWrapper.OpenNI.Status.Ok)
                        return Observable.Throw<OpenNIWrapper.VideoStream>(new Exception("Failed to start OpenNI video stream."));

                    return Observable.FromEvent<OpenNIWrapper.VideoStream.VideoStreamNewFrame, OpenNIWrapper.VideoStream>(
                        handler => stream.OnNewFrame += handler,
                        handler => stream.OnNewFrame -= handler);
                })
                .Where(stream => stream.IsFrameAvailable())
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
