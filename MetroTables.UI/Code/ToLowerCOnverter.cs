using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MetroTables.UI.Code {
	public class ToLowerConverter : MarkupConverter {
		protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string)
				return ((string)value).ToLower();

			return value;
		}

		protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return Binding.DoNothing;
		}
	}
}
