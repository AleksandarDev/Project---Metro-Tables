using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MetroTables.Common.Interfaces;
using MetroTables.Extensions.PluginContracts;

namespace MetroTables.Extensions.Plugins.SamplePlugin {
	[Export(typeof(IPluginUI))]
	public sealed partial class MenuUI : UserControl, IPluginUI {
		public ISheetsControllerContract SheetController { get; private set; }


		public MenuUI() {
			InitializeComponent();
		}

		#region IPluginUI

		bool IPluginUI.IsMenuUIAvailable {
			get { return true; }
		}

		bool IPluginUI.IsMenuUIVisible {
			get { return (this as IPluginUI).IsMenuUIVisible; }
			set { (this as IPluginUI).IsMenuUIVisible = value; }
		}
		private bool isMenuUIVisible = true;

		string IPluginUI.MenuTitle {
			get { return "Sample Plugin"; }
		}

		UserControl IPluginUI.GetMenuUI(object owner) {
			// Check if owner is valid control
			if (owner == null ||
				!(owner is ISheetsControllerContract)) {
					MessageBox.Show("Can't initialize SamplePlugin plugin!\n(owner is not valid)", "SamplePlugin", MessageBoxButton.OK, MessageBoxImage.Error);
					(this as IPluginUI).IsMenuUIVisible = false;
			}
			else {
				SheetController = owner as ISheetsControllerContract;
			}

			return this;
		}

		#endregion

		private void Button_Click_1(object sender, RoutedEventArgs e) {
			Random random = new Random();

			ISheetContract current = SheetController.GetSelectedSheet();
			current.ApplyToSelection(cell => {
				cell.SetValue(random.Next().ToString());
			});
		}

		private void Button_Click_2(object sender, RoutedEventArgs e) {
			ISheetContract current = SheetController.GetSelectedSheet();
			current.ApplyToSelection(cell => {
				cell.Label.Background = (sender as Button).Background;
			});
		}

		private void Button_Click_3(object sender, RoutedEventArgs e) {

		}
	}
}
