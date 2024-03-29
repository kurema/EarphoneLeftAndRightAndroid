﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

namespace EarphoneLeftAndRight.ValueConverters
{
	public class NullToTextValueConverter : Xamarin.Forms.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string text;
			if (parameter == null || parameter is not string) text = "Null:Not Null";
			else text = (string)parameter;
			var texts = text.Split(':', (char)2);
			return value == null ? texts[0] : texts[1];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class NullOrEmptyStringToTextValueConverter : Xamarin.Forms.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string text;
			if (parameter == null || parameter is not string) text = "Null:Not Null";
			else text = (string)parameter;
			var texts = text.Split(':', (char)2);
			return value == null ? texts[0] : (value is string s && string.IsNullOrEmpty(s) ? texts[0] : texts[1]);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class AnyValueConverter : Xamarin.Forms.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var args = (parameter?.ToString() ?? "True:False").Split(':');
			if (value is not System.Collections.IEnumerable enu) return "";
			foreach (var _ in enu)
			{
				return args[0];
			}
			return args[1];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class TimeSpanFormatValueConverter : Xamarin.Forms.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || value is not TimeSpan)
			{
				return "";
			}
			if (parameter == null || parameter is not string) return ((TimeSpan)value).ToString();
			else return FormatTimeSpan((TimeSpan)value, (string)parameter);

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public static string FormatTimeSpan(TimeSpan ts, string tx)
		{
			var dic = new Dictionary<string, double>()
			{
				{ nameof(ts.Days),ts.Days },
				{ nameof(ts.Hours),ts.Hours },
				{ nameof(ts.Milliseconds),ts.Milliseconds },
				{ nameof(ts.Minutes),ts.Minutes },
				{ nameof(ts.Seconds),ts.Seconds },
				{ nameof(ts.Ticks),ts.Ticks },
				{ nameof(ts.TotalDays),ts.TotalDays },
				{ nameof(ts.TotalHours),ts.TotalHours },
				{ nameof(ts.TotalMilliseconds),ts.TotalMilliseconds },
				{ nameof(ts.TotalMinutes),ts.TotalMinutes },
				{ nameof(ts.TotalSeconds),ts.TotalSeconds },
				{ nameof(ts.TotalDays)+nameof(Math.Floor), Math.Floor(ts.TotalDays) },
				{ nameof(ts.TotalHours)+nameof(Math.Floor),Math.Floor(ts.TotalHours) },
				{ nameof(ts.TotalMilliseconds)+nameof(Math.Floor),Math.Floor(ts.TotalMilliseconds) },
				{ nameof(ts.TotalMinutes)+nameof(Math.Floor),Math.Floor(ts.TotalMinutes) },
				{ nameof(ts.TotalSeconds)+nameof(Math.Floor),Math.Floor(ts.TotalSeconds) },
			};
			{
				var reg = new System.Text.RegularExpressions.Regex(@"\[(\w+)\]");
				tx = reg.Replace(tx, new System.Text.RegularExpressions.MatchEvaluator((m) => { if (dic.ContainsKey(m.Groups[1].Value)) return dic[m.Groups[1].Value].ToString(); else return m.Value; }));
			}
			{
				var reg = new System.Text.RegularExpressions.Regex(@"\[:(\w+):\]");
				tx = reg.Replace(tx, new System.Text.RegularExpressions.MatchEvaluator((m) => { return ts.ToString(m.Groups[1].Value); }));
			}
			{
				var reg = new System.Text.RegularExpressions.Regex(@"\[(\w+):([^\[\]]+)\]");
				tx = reg.Replace(tx, new System.Text.RegularExpressions.MatchEvaluator((m) => { if (dic.ContainsKey(m.Groups[1].Value)) return String.Format(m.Groups[2].Value, dic[m.Groups[1].Value]); else return m.Value; }));
			}
			{
				var reg = new System.Text.RegularExpressions.Regex(@"\[if:(\w+):([^\[\]]+)\]");
				tx = reg.Replace(tx, new System.Text.RegularExpressions.MatchEvaluator((m) => { if (dic.ContainsKey(m.Groups[1].Value) && dic[m.Groups[1].Value] > 0) return m.Groups[2].Value; else return ""; }));
			}
			return tx;
		}
	}

	public class BooleanToColorValueConverter : Xamarin.Forms.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string text;
			if (parameter == null || parameter is not string) text = "#00FFFFFF:#00FFFFFF:#00FFFFFF";
			else text = (string)parameter;
			var texts = text.Split(':');

			if (value == null || value is not bool)
			{
				return Xamarin.Forms.Color.FromHex(texts[2]);
			}
			if ((bool)value == true)
			{
				return Xamarin.Forms.Color.FromHex(texts[0]);
			}
			else
			{
				return Xamarin.Forms.Color.FromHex(texts[1]);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class StringReplaceValueConverter : Xamarin.Forms.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var text = value.ToString();
			if (parameter == null) { return value; }
			var dicts = parameter.ToString().Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
			var dict = new Dictionary<string, string>();
			foreach (var item in dicts)
			{
				var kvp = item.Split(':', (char)2);
				if (kvp.Length != 2) continue;
				dict.Add(kvp[0], kvp[1]);
			}
			foreach (var item in dict)
			{
				text = text.Replace(item.Key, item.Value);
			}
			return text;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class StringReplaceMatchValueConverter : Xamarin.Forms.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var text = value.ToString();
			if (parameter == null) { return value; }
			var dicts = parameter.ToString().Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
			var dict = new Dictionary<string, string>();
			foreach (var item in dicts)
			{
				var kvp = item.Split(':', (char)2);
				if (kvp.Length != 2) continue;
				dict.Add(kvp[0], kvp[1]);
			}
			foreach (var item in dict)
			{
				if (text == item.Key) text = item.Value;
			}
			return text;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class ToStringSwitchValueConverter : Xamarin.Forms.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var text = value.ToString();
			if (parameter == null) { return value; }
			var dicts = parameter.ToString().Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
			var dict = new Dictionary<string, string>();
			foreach (var item in dicts)
			{
				var kvp = item.Split(':', (char)2);
				if (kvp.Length != 2) continue;
				dict.Add(kvp[0], kvp[1]);
			}
			foreach (var item in dict)
			{
				if (text == item.Key) return item.Value;
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class BooleanNotValueConverter : Xamarin.Forms.IValueConverter
	{
		public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || value is not bool)
			{
				return null;
			}
			return !((bool)value);
		}

		public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || value is not bool)
			{
				return null;
			}
			return !((bool)value);
		}
	}

	public class BooleanToStringValueConverter : Xamarin.Forms.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string text;
			if (parameter == null || parameter is not string) text = "::";
			else text = (string)parameter;
			var texts = text.Split(':');

			if (value == null || value is not bool)
			{
				return texts[2];
			}
			if ((bool)value == true)
			{
				return texts[0];
			}
			else
			{
				return texts[1];
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class LogValueConverter : Xamarin.Forms.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!double.TryParse(value.ToString(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out double d)) return 0;
			return Math.Log(d);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!double.TryParse(value.ToString(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out double d)) return 0;
			return Math.Pow(Math.E, d);
		}
	}

	public class RoundBackValueConverter : Xamarin.Forms.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!double.TryParse(value.ToString(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out double d1)) return 0;
			if (!double.TryParse(parameter.ToString(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out double d2)) return 0;
			return Math.Floor(d1 / d2) * d2;
		}
	}

	public class FloatToDoubleConverter : Xamarin.Forms.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is null) return (double)0;
			if (value is float) return (double)value;
			if (float.TryParse(value.ToString(), out var f)) return (double)f;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is null) return (float)0;
			if (value is double) return (float)value;
			if (double.TryParse(value.ToString(), out var f)) return (float)f;
			return 0;
		}
	}
}
