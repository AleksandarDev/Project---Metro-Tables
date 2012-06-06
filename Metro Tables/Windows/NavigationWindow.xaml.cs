using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Metro_Tables.Code;

namespace Metro_Tables {
	/// <summary>
	/// Interaction logic for NavigationWindow.xaml
	/// </summary>
	public partial class NavigationWindow : Window {
		public NavigationWindow() {
			InitializeComponent();

			App.Initialize(this);

			if (App.IsLowQuality) {
				WindowBorder.Padding = new Thickness(0);
				WindowBorder.Effect = null;
			}

			CurrentPage.Navigated += PageNavigated;
		}

		void PageNavigated(object sender, NavigationEventArgs e) {
			CurrentPage.RemoveBackEntry();
			Title = "Metro Tables";
		}
	}
}
