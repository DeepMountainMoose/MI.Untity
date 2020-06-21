using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace MI.Extensions
{
    internal static class UniversalTypeConverter
    {
        private const string ImplicitOperatorMethodName = "op_Implicit";
        private const string ExplicitOperatorMethodName = "op_Explicit";

        public static bool TryConvert(object value, Type destinationType, out object result) => TryConvert(value, destinationType, out result, CultureInfo.CurrentCulture);

        public static bool TryConvert(object value, Type destinationType, out object result, CultureInfo culture)
        {
            var typeInfo = destinationType.GetTypeInfo();

            if (value == null || value == DBNull.Value)
            {
                result = typeInfo.IsValueType ? Activator.CreateInstance(destinationType) : null;
                return true;
            }

            if (destinationType == typeof(string))
            {
                result = value.ToString();
                return true;
            }

            destinationType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;
            if (typeInfo.IsInstanceOfType(value))
            {
                result = value;
                return true;
            }

            return TryConvertCore(value, destinationType, out result, culture);
        }

        #region TryConvertCollection

        public static bool TryConvertCollection(IReadOnlyCollection<object> values, Type destinationType, out object result) => TryConvertCollection(values, destinationType, out result, CultureInfo.CurrentCulture);

        public static bool TryConvertCollection(IReadOnlyCollection<object> values, Type destinationType, out object result, CultureInfo culture)
        {
            var typeInfo = destinationType.GetTypeInfo();

            result = null;
            if (destinationType.IsArray)
                return TryConvertArray(values, destinationType.GetElementType(), ref result, culture);

            var genericTypeDefinition = destinationType.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(IEnumerable<>))
                return TryConvertArray(values, destinationType.IsArray ? destinationType.GetElementType() : typeInfo.GetGenericArguments()[0], ref result, culture);

            if (genericTypeDefinition == typeof(List<>))
                return TryConvertList(values, typeInfo.GetGenericArguments()[0], ref result, culture);

            return TryConverOtherCollection(values, destinationType, ref result, culture);
        }

        private static bool TryConvertArray(IReadOnlyCollection<object> values, Type elementType, ref object result, CultureInfo culture)
        {
            var array = (IList)Array.CreateInstance(elementType, values.Count);

            var index = 0;
            foreach (var obj in values)
            {
                if (TryConvert(obj, elementType, out var tr, culture))
                    array[index++] = tr;
                else
                    return false;
            }

            result = array;

            return true;
        }

        private static bool TryConvertList(IReadOnlyCollection<object> values, Type elementType, ref object result, CultureInfo culture)
        {
            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

            foreach (var obj in values)
            {
                if (TryConvert(obj, elementType, out var tr, culture))
                    list.Add(tr);
                else
                    return false;
            }

            result = list;

            return true;
        }

        private static bool TryConverOtherCollection(IReadOnlyCollection<object> values, Type destinationType, ref object result, CultureInfo culture)
        {
            try
            {
                var genericArguments = destinationType.GetTypeInfo().GetGenericArguments();
                if (genericArguments.Length != 1)
                    return false;

                var elementType = genericArguments[0];

                var list = Activator.CreateInstance(destinationType) as IList;
                if (list == null)
                    return false;

                foreach (var obj in values)
                {
                    if (TryConvert(obj, elementType, out var tr, culture))
                        list.Add(tr);
                    else
                        return false;
                }

                result = list;

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion TryConvertCollection

        private static bool TryConvertCore(object value, Type destinationType, out object result, CultureInfo culture)
        {
            result = null;
            if (value.GetType() == destinationType)
            {
                result = value;
                return true;
            }

            if (TryConvertByDefaultTypeConverters(value, destinationType, culture, ref result))
                return true;

            if (TryConvertByIConvertibleImplementation(value, destinationType, culture, ref result))
                return true;

            if (TryConvertByIntermediateConversion(value, destinationType, ref result, culture))
                return true;

            var typeInfo = destinationType.GetTypeInfo();

            if (typeInfo.IsEnum && TryConvertToEnum(value, destinationType, ref result))
                return true;

            if (TryConvertXPlicit(value, destinationType, ExplicitOperatorMethodName, ref result))
                return true;

            if (TryConvertXPlicit(value, destinationType, ImplicitOperatorMethodName, ref result))
                return true;

            if (TryConvertSpecialValues(value, destinationType, ref result))
                return true;

            if (value is string && string.IsNullOrWhiteSpace((string)value))
            {
                result = typeInfo.IsValueType ? Activator.CreateInstance(destinationType) : null;
                return true;
            }

            return false;
        }

        private static bool TryConvertByDefaultTypeConverters(object value, Type destinationType, CultureInfo culture, ref object result)
        {
            var converter = TypeDescriptor.GetConverter(destinationType);
            if (converter.CanConvertFrom(value.GetType()))
            {
                try
                {
                    result = converter.ConvertFrom(null, culture, value);
                    return true;
                }
                catch
                {
                }
            }

            converter = TypeDescriptor.GetConverter(value);
            if (converter.CanConvertTo(destinationType))
            {
                try
                {
                    result = converter.ConvertTo(null, culture, value, destinationType);
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        private static bool TryConvertByIConvertibleImplementation(object value, Type destinationType, IFormatProvider formatProvider, ref object result)
        {
            try
            {
                result = Convert.ChangeType(value, destinationType, formatProvider);

                return true;
            }
            catch { }
            return false;
        }

        private static bool TryConvertXPlicit(object value, Type destinationType, string operatorMethodName, ref object result) => TryConvertXPlicit(value, value.GetType(), destinationType, operatorMethodName, ref result) || TryConvertXPlicit(value, destinationType, destinationType, operatorMethodName, ref result);

        private static bool TryConvertXPlicit(object value, Type invokerType, Type destinationType, string xPlicitMethodName, ref object result)
        {
            var typeInfo = destinationType.GetTypeInfo();

            foreach (var m in invokerType.GetTypeInfo().GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(method => method.Name == xPlicitMethodName && typeInfo.IsAssignableFrom(method.ReturnType))
                .Select(method => new { method, parameters = method.GetParameters() })
                .Where(@t => @t.parameters.Length == 1 && @t.parameters[0].ParameterType == value.GetType())
                .Select(@t => @t.method))
            {
                try
                {
                    result = m.Invoke(null, new[] { value });
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        private static bool TryConvertByIntermediateConversion(object value, Type destinationType, ref object result, CultureInfo culture)
        {
            if (value is char && (destinationType == typeof(double) || destinationType == typeof(float)))
                return TryConvertCore(Convert.ToUInt16(value), destinationType, out result, culture);

            if ((value is double || value is float) && destinationType == typeof(char) && (double)value <= char.MaxValue)
                return TryConvertCore(Convert.ToUInt16(value), destinationType, out result, culture);

            return false;
        }

        private static bool TryConvertToEnum(object value, Type destinationType, ref object result)
        {
            try
            {
                result = Enum.ToObject(destinationType, value);
                return true;
            }
            catch { }

            try
            {
                result = Enum.Parse(destinationType, value.ToString());
                return true;
            }
            catch { }

            return false;
        }

        #region TryConvertSpecialValues

        private static bool TryConvertSpecialValues(object value, Type destinationType, ref object result)
        {
            if (value is char && destinationType == typeof(bool))
                return TryConvertCharToBool((char)value, ref result);

            if (value is string && destinationType == typeof(bool))
                return TryConvertStringToBool((string)value, ref result);

            if (value is bool && destinationType == typeof(char))
                return ConvertBoolToChar((bool)value, out result);

            return false;
        }

        private static bool TryConvertCharToBool(char value, ref object result)
        {
            if (value == '1' ||
                value == 'J' ||
                value == 'Y' ||
                value == 'T')
            {
                result = true;
                return true;
            }
            if (value == '0' ||
                value == 'N' ||
                value == 'F')
            {
                result = false;
                return true;
            }
            return false;
        }

        private static bool TryConvertStringToBool(string value, ref object result)
        {
            var trueValues = new List<string>(new[] { "1", "j", "ja", "y", "yes", "true", "t", ".t." });
            if (trueValues.Contains(value.Trim().ToLower()))
            {
                result = true;
                return true;
            }
            var falseValues = new List<string>(new[] { "0", "n", "no", "nein", "false", "f", ".f." });
            if (falseValues.Contains(value.Trim().ToLower()))
            {
                result = false;
                return true;
            }
            return false;
        }

        private static bool ConvertBoolToChar(bool value, out object result)
        {
            result = value ? 'T' : 'F';
            return true;
        }

        #endregion TryConvertSpecialValues
    }
}
