﻿using System;
using System.ComponentModel;

namespace ToraConHelper.Views;

public class EnumDescriptionConverter(Type type) : EnumConverter(type)
{
    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string))
        {
            if (value != null)
            {
                var fi = value.GetType().GetField(value.ToString());
                if (fi != null)
                {
                    var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    return ((attributes.Length > 0) && (!string.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
                }
            }

            return string.Empty;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}