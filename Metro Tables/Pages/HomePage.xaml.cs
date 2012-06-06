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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Metro_Tables.Code;
using Metro_Tables.Code.Extensions;
using Metro_Tables.Controls;
using Microsoft.Win32;

namespace Metro_Tables {
	/// <summary>
	/// Interaction logic for HomePage.xaml
	/// </summary>
	public partial class HomePage : Page {
		private enum TileSelections {
			None,
			NewWorksheet,
			Open,
			Save
		}

		private TablesControl tablesControl;

		private TileSelections selectedTile = TileSelections.None;


		public HomePage() {
			InitializeComponent();
			Initialize();
		}

		private void Initialize() {
			gridDim.Visibility = System.Windows.Visibility.Hidden;
			topControl.Visibility = System.Windows.Visibility.Hidden;
			listBoxOptions.Visibility = Visibility.Hidden;
			labelSelectedTile.Visibility = Visibility.Hidden;

			Load();
		}

		private void Load() {
			tablesControl = new TablesControl();
			topControl.Content = tablesControl;

			GenerateSparkles();
		}

		private void GenerateSparkles() {
			int numSparkles = 25;

			double minSize = 5 ;
			double maxSize = 15;

			GradientStopCollection gsc = new GradientStopCollection();
			gsc.Add(new GradientStop((Color)FindResource("TileGradientColorStart30"), 0.3d));
			gsc.Add(new GradientStop((Color)FindResource("TileGradientColorEnd100"), 1d));
			Brush fill = new LinearGradientBrush(gsc);

			Random random = new Random();

			for (int index = 0; index < numSparkles; index++) {
				Rectangle rect = new Rectangle();

				double randX = random.NextDouble() * App.NavigationWindow.Width;
				double randY = random.NextDouble() * App.NavigationWindow.Height;
				double randSize = random.NextDouble() * maxSize + minSize;

				rect.Fill = fill;
				rect.Width = rect.Height = randSize;

				Canvas.SetTop(rect, randY);
				Canvas.SetLeft(rect, randX);

				SparklesCanvas.Children.Add(rect);
			}
		}

		#region Top control methods

		// Show > Activate > Minimaze > Deactivate
		// Show sets visibility to Visible and makes opacity 0 zo 100 animation
		// Activate sets visibility to Visible and scales + translates control to center and to fill window
		// Minimize sets visibility to Visible and scales + translates control to X -45 Y 45 and scale X 0,4 Y 0,4
		// Deactivate sets visiblity to Hidden and animates ofaciti 100 to 0

		public void ShowTopControl(Object control, Boolean playAnimation = true) {
			topControl.Content = control ?? tablesControl.Content;
			topControl.Visibility = Visibility.Visible;


			if (playAnimation) BeginStoryboard((Storyboard)FindResource("TopControlShowStoryboard"));
		}
		
		public void ActivateTopControl(Object control, Boolean playAnimation = true) {
			CheckForSave();

			topControl.Content = control ?? tablesControl.Content;
			topControl.Visibility = Visibility.Visible;

			if (playAnimation) BeginStoryboard((Storyboard)FindResource("TopControlActivateStoryboard"));
		}

		public void MinimizeTopControl(Object control = null, Boolean playAnimation = true) {
			topControl.Content = control ?? topControl.Content;
			topControl.Visibility = System.Windows.Visibility.Visible;

			if (playAnimation) BeginStoryboard((Storyboard)FindResource("TopControlMinimizeStoryboard"));
		}

		public void DeactivateTopControl(Boolean playAnimation = true) {
			if (playAnimation) BeginStoryboard((Storyboard)FindResource("TopControlDeactivateStoryboard"));

			topControl.Visibility = Visibility.Hidden;
		}

		private void CheckForSave() {
			TablesControl tablesControl = topControl.Content as TablesControl;
			
			// If tables control isn't top control, there is nothing to save
			if (tablesControl == null) return;

			// TODO: Save document if needed
		}

		#endregion

		#region Dim message methods

		public void ActivateDim(UserControl control, Boolean playAnimation = true) {
			dimMessageControl = control ?? dimMessageControl;
			gridDim.Visibility = Visibility.Visible;

			if (playAnimation) BeginStoryboard((Storyboard)FindResource("DimControlActivationStoryboard"));
		}
		public void DeactivateDim(Boolean playAnimation = true) {
			if (playAnimation) BeginStoryboard((Storyboard)FindResource("DimControlDeactivationStoryboard"));

			dimMessageControl = null;
			gridDim.Visibility = Visibility.Hidden;
		}

		#endregion

		
		private void WindowChromeDrag_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			if (e.ClickCount == 2) {
				App.Maximize();
			}
			else App.NavigationWindow.DragMove();
		}


		#region Main-Menu options

		private void buttonOpen_Click(object sender, RoutedEventArgs e) {
			if (selectedTile == TileSelections.Open) {
				// Activate default action
				ActionOpenFromFile();
			}
			else {
				selectedTile = TileSelections.Open;

				listBoxOptions.Items.Clear();
				listBoxOptions.Visibility = System.Windows.Visibility.Visible;
				listBoxOptions.Items.Add(GetNewOptionsItem("From file...", ActionOpenFromFile));
				listBoxOptions.Items.Add(GetNewOptionsItem("From URL...", ActionOpenFromURL));
				listBoxOptions.Items.Add(GetNewOptionsItem("Recent", ActionOpenFromRecent));
				listBoxOptions.SelectedIndex = 0;

				labelSelectedTile.Content = "Open worksheet";
				labelSelectedTile.Visibility = System.Windows.Visibility.Visible;

				Image image = new Image();
				image.SetBitmapSource("/Metro%20Tables;component/Images/OpenXLSX.png");
				ShowTopControl(image);
			}
		}

		private void ButtonNewWorksheet_OnClick(object sender, RoutedEventArgs e) {
			if (selectedTile == TileSelections.NewWorksheet) {
				// Activate default action
				ActionShowCurrent();
			}
			else {
				selectedTile = TileSelections.NewWorksheet;

				listBoxOptions.Items.Clear();
				listBoxOptions.Visibility = Visibility.Visible;
				listBoxOptions.Items.Add(GetNewOptionsItem("Current worksheet", ActionShowCurrent));
				listBoxOptions.Items.Add(GetNewOptionsItem("Empty worksheet", ActionNewEmptyWorksheet));
				listBoxOptions.Items.Add(GetNewOptionsItem("From Template...", ActionNewFromTemplate));
				listBoxOptions.SelectedIndex = 0;

				labelSelectedTile.Content = "New worksheet";
				labelSelectedTile.Visibility = Visibility.Visible;

				ShowTopControl(tablesControl);
			}
		}

		private void buttonSave_Click(object sender, RoutedEventArgs e) {
			if (selectedTile == TileSelections.Save) {
				// Activate default action
				ActionSave();
			}
			else {
				selectedTile = TileSelections.Save;

				listBoxOptions.Items.Clear();
				listBoxOptions.Visibility = Visibility.Visible;
				listBoxOptions.Items.Add(GetNewOptionsItem("Save current", ActionSave));
				listBoxOptions.SelectedIndex = 0;

				labelSelectedTile.Content = "Save";
				labelSelectedTile.Visibility = Visibility.Visible;

				Image image = new Image();
				image.SetBitmapSource("/Metro%20Tables;component/Images/SaveWork.png");
				ShowTopControl(image);
			}
		}

		private ListBoxItem GetNewOptionsItem(Object content, Action onClickAction) {
			ListBoxItem item = new ListBoxItem();

			item.Content = content;
			item.Style = (Style)FindResource("MetroListBoxItemStyle");

			item.MouseDoubleClick += (s, e) => onClickAction.Invoke();

			return item;
		}

		#region Action calls

		private void ActionShowCurrent() {
			ActivateTopControl(tablesControl);
		}

		private void ActionOpenFromRecent() {
			throw new NotImplementedException();
		}

		private void ActionOpenFromURL() {
			throw new NotImplementedException();
		}

		private void ActionSave() {
			throw new NotImplementedException();
		}

		private void ActionNewFromTemplate() {
			throw new NotImplementedException();
		}

		private void ActionNewEmptyWorksheet() {
			tablesControl = new TablesControl();

			ActivateTopControl(tablesControl);
		}

		private void ActionOpenFromFile() {
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.FileName = "Worksheet1";
			openDialog.DefaultExt = ".xlsx";
			openDialog.Filter = "Office Open XML|*.xlsx|Excel 2007/2010|*.xlsx";
			openDialog.Multiselect = false;
			openDialog.Title = "Metro Tables - Open document...";


			Nullable<bool> result = openDialog.ShowDialog();

			if (result.HasValue && result.Value == true) {
				if (openDialog.CheckFileExists) {
					try {
						tablesControl = new TablesControl(openDialog.FileName);
						ActivateTopControl(tablesControl);
					}
					catch (Exception ex) {
						MessageBox.Show("Document can't be opened!", "Metro Tables", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
				else {
					MessageBox.Show("Selected file doesn't exist!", "Metro Tables", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		#endregion

		#endregion
	}
}
