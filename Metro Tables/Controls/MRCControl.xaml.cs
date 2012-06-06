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

namespace Metro_Tables.Controls {
	/// <summary>
	/// Interaction logic for MRCControl.xaml
	/// </summary>
	public partial class MRCControl : UserControl {
		public MRCControl() {
			InitializeComponent();
		}

		private void ButtonClose_Click(object sender, RoutedEventArgs e) {
			App.Close();
		}

		private void ButtonMaximize_Click(object sender, RoutedEventArgs e) {
			App.Maximize();
		}

		private void ButtonMinimaze_Click(object sender, RoutedEventArgs e) {
			App.Minimaze();
		}
	}
}
