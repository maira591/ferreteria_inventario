using System;
using System.ComponentModel;
using System.Reflection;

namespace Ferreteria.Infrastructure.Core
{
    public static class ObjectExtensions
    {
        public static PropertyInfo[] GetProperties<T>(this T @this)
        {
            return @this.GetType().GetProperties();
        }

        public static PropertyInfo[] GetProperties<T>(this T @this, BindingFlags bindingAttr)
        {
            return @this.GetType().GetProperties(bindingAttr);
        }

        public static PropertyInfo GetProperty<T>(this T @this, string name)
        {
            return @this.GetType().GetProperty(name);
        }

        public static PropertyInfo GetProperty<T>(this T @this, string name, BindingFlags bindingFlags)
        {
            return @this.GetType().GetProperty(name, bindingFlags);
        }

        public static object GetPropertyValue<T>(this T @this, string propertyName)
        {
            return @this.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(@this, null);
        }

        public static bool IsNull<T>(this T @this)
        where T : class
        {
            return @this == null;
        }

        public static T To<T>(this object @this)
        {
            if (@this == null || @this == DBNull.Value)
            {
                return default(T);
            }
            Type type = typeof(T);
            if (@this.GetType() == type)
            {
                return (T)@this;
            }
            TypeConverter converter = TypeDescriptor.GetConverter(@this);
            if (converter.CanConvertTo(type))
            {
                return (T)converter.ConvertTo(@this, type);
            }
            converter = TypeDescriptor.GetConverter(type);
            if (!converter.CanConvertFrom(@this.GetType()))
            {
                return (T)@this;
            }
            return (T)converter.ConvertFrom(@this);
        }

        public static object To(this object @this, Type type)
        {
            if (@this == null || @this == DBNull.Value)
            {
                return null;
            }
            Type type1 = type;
            if (@this.GetType() == type1)
            {
                return @this;
            }
            TypeConverter converter = TypeDescriptor.GetConverter(@this);
            if (converter.CanConvertTo(type1))
            {
                return converter.ConvertTo(@this, type1);
            }
            converter = TypeDescriptor.GetConverter(type1);
            if (!converter.CanConvertFrom(@this.GetType()))
            {
                return @this;
            }
            return converter.ConvertFrom(@this);
        }
    }
}
