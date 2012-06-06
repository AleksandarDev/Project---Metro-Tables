using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

namespace Metro_Tables.Code.Extensions {
	public static class ColorExtensions {
		public static Color ConvertToNewColor(this System.Drawing.Color @this) {
			return Color.FromArgb(@this.A, @this.R, @this.G, @this.B);
		}
	}
}
