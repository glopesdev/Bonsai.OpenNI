using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace Bonsai.OpenNI
{
    [Description("Applies a near and far truncate to the depth map.")]
    public class DepthTruncate : Transform<IplImage, IplImage>
    {
        const int MaxThresholdValue = 4000; // 4 meters when using PixelFormat.Depth1Mm
        const int DefaultNearThresholdValue = 0;
        const int DefaultFarThresholdValue = MaxThresholdValue;
        const bool DefaultBinary = false;
        const bool DefaultConvert8Bit = false;

        [Range(0, MaxThresholdValue)]
        [Precision(0, 1)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The near threshold value used to test individual pixels.")]
        [DefaultValue(DefaultNearThresholdValue)]
        public ushort NearThresholdValue { get; set; } = DefaultNearThresholdValue;

        [Range(0, MaxThresholdValue)]
        [Precision(0, 1)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The far threshold value used to test individual pixels.")]
        [DefaultValue(DefaultFarThresholdValue)]
        public ushort FarThresholdValue { get; set; } = DefaultFarThresholdValue;

        [Description("Binarizes the resulting depth map.")]
        [DefaultValue(DefaultBinary)]
        public bool Binary { get; set; } = DefaultBinary;

        [Description("Converts the resulting depth map to an 8 bit depth map.")]
        [DefaultValue(DefaultConvert8Bit)]
        public bool Convert8Bit { get; set; } = DefaultConvert8Bit;

        public override IObservable<IplImage> Process(IObservable<IplImage> source)
            => source.Select(input => Convert8Bit
                    ? Process8U(input)
                    : Process16U(input));

        IplImage Process8U(IplImage input)
        {
            var output = new IplImage(input.Size, IplDepth.U8, 1);

            if (NearThresholdValue >= FarThresholdValue)
            {
                output.GetMat().SetZero();
            }
            else if (Binary)
            {
                Transform<ushort, byte>(input, output,
                    value => value >= NearThresholdValue && value < FarThresholdValue
                        ? byte.MaxValue
                        : (byte)0);
            }
            else
            {
                var scale = (double)byte.MaxValue / (FarThresholdValue - NearThresholdValue);
                Transform<ushort, byte>(input, output,
                    value => value >= NearThresholdValue && value < FarThresholdValue
                        ? (byte)(scale * (value - NearThresholdValue))
                        : (byte)0);
            }

            return output;
        }

        IplImage Process16U(IplImage input)
        {
            var output = new IplImage(input.Size, IplDepth.U16, 1);

            if (NearThresholdValue >= FarThresholdValue)
            {
                output.GetMat().SetZero();
            }
            else if (Binary)
            {
                Transform<ushort, ushort>(input, output,
                    value => value >= NearThresholdValue && value < FarThresholdValue
                        ? ushort.MaxValue
                        : (ushort)0);
            }
            else
            {
                Transform<ushort, ushort>(input, output,
                    value => value >= NearThresholdValue && value < FarThresholdValue
                        ? (ushort)(value - NearThresholdValue)
                        : (ushort)0);
            }

            return output;
        }

        static unsafe void Transform<TInput, TOutput>(IplImage input, IplImage output, Func<TInput, TOutput> func)
        {
            var rows = input.Size.Height;
            var columns = input.WidthStep / Unsafe.SizeOf<TInput>();

            for (var row = 0; row < rows; row++)
            {
                ref var inputRow = ref Unsafe.AsRef<TInput>(input.GetRow(row).Data.ToPointer());
                ref var outputRow = ref Unsafe.AsRef<TOutput>(output.GetRow(row).Data.ToPointer());
                for (var column = 0; column < columns; column++)
                {
                    ref var inputValue = ref Unsafe.Add(ref inputRow, column);
                    ref var outputValue = ref Unsafe.Add(ref outputRow, column);
                    outputValue = func(inputValue);
                }
            }
        }
    }
}
