using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Bonsai.OpenNI
{
    [Description("Applies a fixed threshold to the depth map. Outputs and 8 bit single channel depth map.")]
    public class DepthThreshold : Transform<IplImage, IplImage>
    {
        const int MaxThresholdValue = 4000; // 4 meters when using PixelFormat.Depth1Mm
        const int DefaultThresholdValue = 500; // 0.5 meters when using PixelFormat.Depth1Mm

        [Range(0, MaxThresholdValue)]
        [Precision(0, 1)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The threshold value used to test individual pixels.")]
        [DefaultValue(DefaultThresholdValue)]
        public double ThresholdValue { get; set; } = DefaultThresholdValue;

        public override IObservable<IplImage> Process(IObservable<IplImage> source)
            => source.Select(input =>
            {
                var output = new IplImage(input.Size, IplDepth.U8, 1);
                CV.Threshold(input, output, ThresholdValue, 255, ThresholdTypes.BinaryInv);
                return output;
            });
    }
}
