using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bonsai.OpenNI
{
    public class DeviceConverter : TypeConverter
    {
#pragma warning disable IDE0052 // Remove unread private members
        readonly Context context = Context.Instance; // initializes OpenNI 
#pragma warning restore IDE0052 // Remove unread private members

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            => true;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var length = OpenNIWrapper.OpenNI.EnumerateDevices().Length;
            var values = new object[length];
            for (var index = 0; index < length; index++)
                values[index] = index;
            return new StandardValuesCollection(values);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value) 
            => value is int index 
            && index >= 0 
            && index < OpenNIWrapper.OpenNI.EnumerateDevices().Length;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) 
            => sourceType == typeof(string);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) 
            => destinationType == typeof(int);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                var regex = new Regex(@"^\d+");
                var results = regex.Matches(str);
                if (results.Count > 0 && int.TryParse(results[0].Value, out var index))
                    return index;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var index = (int)value;
                var devices = OpenNIWrapper.OpenNI.EnumerateDevices();
                var device = devices[index];
                return $"{value} - {device.Name} ({device.Vendor})";
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
