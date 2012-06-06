using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Metro_Tables.Code.Extensions {
	public static class ImageExtensions {
		public static void SetBitmapSource(this Image image, String source, Int32 width = -1, Int32 height = -1, Boolean setImageSize = false) {
			Contract.Requires(!String.IsNullOrEmpty(source));


			SetBitmapSource(image, new Uri(source, UriKind.RelativeOrAbsolute), width, height, setImageSize);
		}

		public static void SetBitmapSource(this Image image, Uri source, Int32 width = -1, Int32 height = -1, Boolean setImageSize = false) {
			Contract.Requires(source != null);


			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();

			bitmapImage.UriSource = source;

			if (height > 0) bitmapImage.DecodePixelHeight = height;
			if (width > 0) bitmapImage.DecodePixelWidth = width;

			bitmapImage.EndInit();

			image.Source = bitmapImage;
			if (setImageSize) {
				image.Width = width;
				image.Height = height;
			}
		}
	}
}
