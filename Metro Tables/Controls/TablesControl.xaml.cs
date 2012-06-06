using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
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
using GemBox.Spreadsheet;
using MetroTables.Common.Interfaces;
using MetroTables.Controls;
using MetroTables.Extensions.PluginContracts;
using MetroTables.Code.Plugins;
using MetroTables.Code.Plugins.Reports;
using System.Collections.ObjectModel;
using MetroTables.Code.Formula;

namespace Metro_Tables.Controls {
	/// <summary>
	/// Interaction logic for TablesControl.xaml
	/// </summary>
	public partial class TablesControl : UserControl, ISheetsControllerContract {
		private const string PluginsPath = @"Extensions\Plugins";
		private PluginManager<IPluginUI> plugins;

		List<string> SystemFontFamilies { get; set; }


		#region Constructors

		public TablesControl() {
			InitializeComponent();

			SourceDocument = new ExcelFile();

			Initialize();
		}

		public TablesControl(String path) {
			InitializeComponent();

			SourceDocument = new ExcelFile();
			OpenFromFile(path);

			Initialize();
		}

		#endregion

		// NOTE: Not implemented
		public void UserControl_Loaded(object sender, RoutedEventArgs e) {

		}

		private void Initialize() {
			// Sets default document values
			Name = "Worksheet 1";
			Path = @"\Worksheet 1.xlsx";

			// Clears sheets
			SheetsTabControl.Items.Clear();

			// Imports document if available
			ImportDocument();

			// Initializes available extensions
			InitializeExtensions();

			// Adds tab item that lets user to add new sheets
			TabItem addNewSheetTabItem = FromExcelWorksheetToTabItem();
			addNewSheetTabItem.Header = "+";
			addNewSheetTabItem.PreviewMouseUp += (s, se) => {
				TabItem newTabItem = FromExcelWorksheetToTabItem();
				newTabItem.Header = "Sheet " + SheetsTabControl.Items.Count;
				SheetsTabControl.Items.Insert(
					SheetsTabControl.Items.Count - 1,
					newTabItem);
				SheetsTabControl.SelectedIndex = SheetsTabControl.Items.Count - 2;
			};
			SheetsTabControl.Items.Add(addNewSheetTabItem);

			// Loading System Font Families
			SystemFontFamilies = new List<string>();
			foreach (FontFamily ff in Fonts.SystemFontFamilies) {
				SystemFontFamilies.Add(ff.ToString());
			}
			SystemFontFamilies.Sort();
			ComboBoxFontFamily.ItemsSource = SystemFontFamilies;
		}

		/// <summary>
		/// Initializes and sets plugins in operation
		/// </summary>
		private void InitializeExtensions() {
			try {
				// Loads and composes extensions
				this.plugins = new PluginManager<IPluginUI>(PluginsPath);
				this.plugins.PluginUpdated += (sender, eventArgs) => UpdatePlugins(eventArgs.Plugins);
				this.plugins.Initialize();

				// Initialize formula manager
				MetroTables.Code.Formula.FormulaManager.Initialize(PluginsPath);
			}
			catch (Exception) {
				MessageBox.Show("Can't load some plugins!", "Metro Tables", MessageBoxButton.OK, MessageBoxImage.Warning);

				// Then dont load
				return;
			}
		}

		private void UpdatePlugins(IEnumerable<IPluginUI> plugins) {
			// TODO Add some kind of indicator
			this.Dispatcher.Invoke(() => {
				// Remove all plugin UIs
				for (int index = 0; index < TablesControlMenu.Items.Count; index++) {
					string menuItemTitle = (TablesControlMenu.Items[index] as TabItem).Header as String;
					if (menuItemTitle.Contains("(plugin)")) {
						TablesControlMenu.Items.RemoveAt(index--);
					}
				}

				foreach (IPluginUI plugin in plugins) {
					RegisterPlugin(plugin);
				}
			});
		}

		private void RegisterPlugin(IPluginUI plugin) {
			if (plugin.IsMenuUIAvailable) {
				TabItem pluginUI = new TabItem();
				pluginUI.Style = (Style)FindResource("SimpleTabItem");

				pluginUI.Header = plugin.MenuTitle + " (plugin)";
				pluginUI.Content = plugin.GetMenuUI(this);

				TablesControlMenu.Items.Add(pluginUI);
			}
		}

		private void RemovePlugin(IPluginUI plugin) {
			foreach (TabItem item in TablesControlMenu.Items.OfType<TabItem>()) {
				if (item.Header.ToString() == plugin.MenuTitle) {
					TablesControlMenu.Items.Remove(item);
					return;
				}
			}
		}

		/// <summary>
		/// Shows user a list of unsuccessfull compositions
		/// </summary>
		/// <param name="unsuccessfullReports">List of unsuccessfull reports</param>
		private void ReportUnsuccessfullCompositions(IEnumerable<CompositionReport> unsuccessfullReports) {
#if DEBUG
			foreach (CompositionReport report in unsuccessfullReports){
				System.Diagnostics.Debug.WriteLine(report.ToString());
			}
#endif
			MessageBox.Show(unsuccessfullReports.Select(report => report.ToString()).ToString(), "Metro Tables - Plugin error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}

		bool ISheetsControllerContract.OpenFromFile(string path) {
			return OpenFromFile(path);
		}
		
		public bool OpenFromFile(String path) {
			// If file doesn't exist then ask user if repeat
			// file selction process - recursion on this function
			if (path == null || !System.IO.File.Exists(path)) {
				MessageBoxResult result = MessageBox.Show("Invalid file path \"" + path + "\"!\nError code: 44 4f 43 4f 46 46 30 31\n\nRetry?", "Metro Tables", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
					OpenFromFile(path);

				return false;
			}

			// Check if extension is valid
			if (System.IO.Path.GetExtension(path) != ".xlsx") {
				MessageBox.Show("Invalid file format \"" + System.IO.Path.GetFileName(path) + "\"!\nError code: 44 4f 43 4f 46 46 30 32", "Metro Tables", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			// Try to load Xlsx file from given path
			try {
				string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
				string extension = System.IO.Path.GetExtension(path);

				Path = path;
				Name = fileName;

				this.SourceDocument.LoadXlsx(path, XlsxOptions.None);
			}
			catch (Exception) {
				MessageBox.Show("Unknown error occured!\nError code: 44 4f 43 4f 46 46 30 33", "Metro Tables", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			// Uspijesno otvaranje dokumenta
			return true;
		}

		#region Import document methods

		/// <summary>
		/// Imports all settings and data from opened document
		/// </summary>
		private void ImportDocument() {
			// Import title (document name)
			PageTitle.Text = Name;

			// Import worksheets
			ImportSheets();
		}

		/// <summary>
		/// Clears current sheets and adds all sheets from loaded document
		/// </summary>
		private void ImportSheets() {
			// Clear all sheets
			SheetsTabControl.Items.Clear();

			// If there is nothing to import create new empty sheet
			if (SourceDocument.Worksheets.Count == 0) {
				TabItem sheet = FromExcelWorksheetToTabItem();
				sheet.Header = "Sheet 1";
				SheetsTabControl.Items.Add(sheet);
			}
			// Else go through list of sheets and add them to SheetsTabControl by 
			// converting it to TabItem using defined method
			else {
				foreach (ExcelWorksheet worksheet in SourceDocument.Worksheets) {
					SheetsTabControl.Items.Add(FromExcelWorksheetToTabItem(worksheet));
				}
			}

			SheetsTabControl.SelectedIndex = 0;
		}

		/// <summary>
		/// Converts ExcelWorksheet to TabItem sheet equivalent
		/// </summary>
		/// <param name="worksheet">Worksheet to convert, null will create new empty sheet.</param>
		/// <returns>TabItem of given ExcelWorksheet</returns>
		protected TabItem FromExcelWorksheetToTabItem(ExcelWorksheet worksheet = null) {
			// Create new TabItem
			TabItem sheet = new TabItem() {
				//Style = (Style)FindResource("SimpleTabItem")
			};

			// Create new SheetControl and fill the information
			SheetControl control = new SheetControl(worksheet);

			// If worksheet isn't null, start importing data
			if (worksheet != null) {
				sheet.Header = worksheet.Name;
				// TODO Import rest
				SetVisibility(ref control, worksheet.Visibility == SheetVisibility.Visible);
			}

			// Sets events
			control.SelectionEnded += (s, se) => {
				UpdateMenuFontSize(se.FontSize);
				ComboBoxFontFamily.SelectedIndex = SystemFontFamilies.IndexOf(se.FontFamily);

				if (se.Formula != null) {
					
				}
			};

			// Set content to just created control and return tab item
			sheet.Content = control;
			return sheet;
		}

		protected void UpdateMenuFontSize(double size) {
			if (size < 0) ComboBoxFontSizes.SelectedIndex = -1;
			else {
				if (!fontSizes.Contains(size)) {
					fontSizes.Add(size);
					fontSizes.Sort();
				}
				ComboBoxFontSizes.SelectedIndex = fontSizes.IndexOf(size);
			}
		}

		/// <summary>
		/// Sets visibility of sheet 
		/// </summary>
		/// <param name="sheetIndex">Zero-based index of sheet</param>
		/// <param name="isVisible">Value that shows whether sheet is visible</param>
		public void SetVisibility(int sheetIndex, bool isVisible) {
			// Checks if index is in range 0..tabs count
			if (sheetIndex >= SheetsTabControl.Items.Count) {
				System.Diagnostics.Debug.WriteLine("SheetIndex out of range!");
				return;
			}

			// Calls private method that requires object to change visibility
			SheetControl sheet = SheetsTabControl.Items.GetItemAt(sheetIndex) as SheetControl;
			if (sheet != null) {
				SetVisibility(ref sheet, isVisible);
			}
			else {
				System.Diagnostics.Debug.WriteLine("TabItem is not of type SheetControl!");
				return;
			}
		}

		/// <summary>
		/// Sets visibility of sheet 
		/// </summary>
		/// <param name="sheetIndex">Zero-based index of sheet</param>
		/// <param name="isVisible">Value that shows whether sheet is visible</param>
		void ISheetsControllerContract.SetVisibility(int sheetIndex, bool isVisible) {
			SetVisibility(sheetIndex, isVisible);
		}

		/// <summary>
		/// Sets visiblity of tab item
		/// </summary>
		/// <param name="sheet">Sheet to change property to</param>
		/// <param name="p">Is sheet visible</param>
		protected virtual void SetVisibility(ref SheetControl sheet, bool isVisible) {
			if (sheet == null) throw new ArgumentNullException("sheet", "Can't change visiblity of sheet that is null!");

			// Sets control visibility 
			sheet.Visibility = isVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

			if (!isVisible) {
				// TODO: Add tabItem to list of hidden sheets
			}
		}

		#endregion

		#region Sheet selected methods

		/// <summary>
		/// Gets SheetControl from index
		/// </summary>
		/// <param name="index">Zero-based index of sheet</param>
		/// <returns>SheetControl object on specified index. If index is out of range or object on that index is not type of SheetControl, returns null.</returns>
		public SheetControl GetSheet(int index) {
			// Checks if index is in range 0..tabs count
			if (index >= SheetsTabControl.Items.Count) {
				System.Diagnostics.Debug.WriteLine("SheetIndex out of range!");
				return null;
			}

			
			TabItem tabItem = SheetsTabControl.Items.GetItemAt(index) as TabItem;
			if (tabItem == null ||
				tabItem.Content == null) {
				System.Diagnostics.Debug.WriteLine("TabItem is not found!");
				return null;
			}
			else {
				SheetControl sheet = tabItem.Content as SheetControl;
				if (sheet != null) {
					return sheet;
				}
				else {
					System.Diagnostics.Debug.WriteLine("TabItem's content is not of type SheetControl!");
					return null;
				}
			}
		}

		/// <summary>
		/// Gets ISheetContract from index
		/// </summary>
		/// <param name="index">Zero-based index of sheet</param>
		/// <returns>ISheetContract object on specified index. If index is out of range or object on that index is not type of ISheetContract, returns null.</returns>
		ISheetContract ISheetsControllerContract.GetSheet(int index) {
			return GetSheet(index) as ISheetContract;
		}

		/// <summary>
		/// Gets object of selected sheet
		/// </summary>
		/// <returns>SheetControl -- selected object</returns>
		public SheetControl GetSelectedSheet() {
			return GetSheet(GetSelectedSheetIndex());
		}

		/// <summary>
		/// Gets object of selected sheet
		/// </summary>
		/// <returns>ISheetContract -- selected object</returns>
		ISheetContract ISheetsControllerContract.GetSelectedSheet() {
			return GetSelectedSheet() as ISheetContract;
		}

		/// <summary>
		/// Gets selected sheet index
		/// </summary>
		/// <returns>Zero-based index of selected sheet</returns>
		public int GetSelectedSheetIndex() {
			return SheetsTabControl.SelectedIndex;
		}

		/// <summary>
		/// Gets selected sheet index
		/// </summary>
		/// <returns>Zero-based index of selected sheet</returns>
		int ISheetsControllerContract.GetSelectedSheetIndex() {
			return GetSelectedSheetIndex();
		}

		#endregion

		#region UI Call methods

		private void BackButton_Click(object sender, RoutedEventArgs e) {
			App.ShowHomePage(this);
		}

		private void BoldSelection_Click(object sender, RoutedEventArgs e) {
			((SheetControl)SheetsTabControl.SelectedContent).ApplyToSelection(cell => {
				if (cell.FontWeight != FontWeights.Bold)
					cell.FontWeight = FontWeights.Bold;
				else cell.FontWeight = FontWeights.Normal;
			});
		}

		private void ItalicSelection_Click(object sender, RoutedEventArgs e) {
			((SheetControl)SheetsTabControl.SelectedContent).ApplyToSelection(cell => {
				if (cell.FontStyle != FontStyles.Italic)
					cell.FontStyle = FontStyles.Italic;
				else cell.FontStyle = FontStyles.Normal;
			});
		}

		private void IncFontSizeSelection_Click(object sender, RoutedEventArgs e) {
			((SheetControl)SheetsTabControl.SelectedContent).ApplyToSelection(cell => {
				cell.FontSize = cell.FontSize + 2;
				UpdateMenuFontSize(cell.FontSize);
			});
		}

		private void DecFontSizeSelection_Click(object sender, RoutedEventArgs e) {
			((SheetControl)SheetsTabControl.SelectedContent).ApplyToSelection(cell => {
				cell.FontSize = cell.FontSize - 2;
				UpdateMenuFontSize(cell.FontSize);
			});
		}

		private void ComboBoxFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			((SheetControl)SheetsTabControl.SelectedContent).ApplyToSelection(cell => {
				if (ComboBoxFontFamily.SelectedValue != null)
					cell.FontFamily = new FontFamily(ComboBoxFontFamily.SelectedValue.ToString());
			});
		}

		private void ComboBoxFontSizes_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			((SheetControl)SheetsTabControl.SelectedContent).ApplyToSelection(cell => {
				cell.FontSize = (double)ComboBoxFontSizes.SelectedValue;
			});
		}

		private List<double> fontSizes = new List<double> {
				6, 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 30, 32, 40, 48, 58, 72, 96
			};
		private void ComboBoxFontSizes_Loaded(object sender, RoutedEventArgs e) {
			ComboBoxFontSizes.ItemsSource = fontSizes;
		}

		private void TopAlignmentSelection_Click(object sender, RoutedEventArgs e) {
			((SheetControl)SheetsTabControl.SelectedContent).ApplyToSelection(cell => {
				cell.Label.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
			});
		}

		private void LeftAlignmentSelection_Click(object sender, RoutedEventArgs e) {
			((SheetControl)SheetsTabControl.SelectedContent).ApplyToSelection(cell => {
				cell.Label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
			});
		}

		private void MiddleAlignmentSelection_Click(object sender, RoutedEventArgs e) {
			((SheetControl)SheetsTabControl.SelectedContent).ApplyToSelection(cell => {
				cell.Label.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
			});
		}

		private void CenterAlignmentSelection_Click(object sender, RoutedEventArgs e) {
			((SheetControl)SheetsTabControl.SelectedContent).ApplyToSelection(cell => {
				cell.Label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
			});
		}

		private void BottomAlignmentSelection_Click(object sender, RoutedEventArgs e) {
			((SheetControl)SheetsTabControl.SelectedContent).ApplyToSelection(cell => {
				cell.Label.VerticalContentAlignment = System.Windows.VerticalAlignment.Bottom;
			});
		}

		private void RightAlignmentSelection_Click(object sender, RoutedEventArgs e) {
			((SheetControl)SheetsTabControl.SelectedContent).ApplyToSelection(cell => {
				cell.Label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
			});
		}

		#endregion


		#region Properties

		public ExcelFile SourceDocument { get; private set; }
		ExcelFile ISheetsControllerContract.SourceDocument { get { return SourceDocument; } }

		public String Name { get; set; }
		string ISheetsControllerContract.Name {
			get { return Name; }
			set { Name = value; }
		}

		public String Path { get; protected set; }
		string ISheetsControllerContract.Path {
			get { return Path; }
		}

		#endregion
	}
}