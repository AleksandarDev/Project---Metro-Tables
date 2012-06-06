using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Metro_Tables.Code;

namespace Metro_Tables {
	// BUG SelectedCellsAction doesn't return valid list if no cells are selected


	public partial class App : Application {
		public static bool IsLowQuality = false;

		public static NavigationWindow NavigationWindow { get; private set; }
		private static WelcomePage welcomePage;
		private static HomePage homePage;

		private static Page currentPage;
		public static Page CurrentPage { 
			get { return currentPage; }
			set {
				if (value != null) {
					NavigationWindow.CurrentPage.NavigationService.Navigate(value);
					App.currentPage = value;
				}
				else System.Diagnostics.Debug.WriteLine("App: Trying to navigate to [null] page!", "Error");
			}
		}

		public static event MainManagerEventHandler OnLoaded;

		public static void Initialize(NavigationWindow navigationWindow) {
			App.NavigationWindow = navigationWindow;

			// TODO load settings
			// TOOD load IsLowQuality

			App.ShowWelcomePage();
		}

		public static void Load() {
			Task loadTask = new Task(App.LoadApplication);
			loadTask.Start();
		}

		private static void LoadApplication() {
			App.welcomePage.ChangeStatus("Loading home screen . . .");
			Current.Dispatcher.InvokeAsync(() => {
				App.homePage = new HomePage();
				if (App.OnLoaded != null) {
					App.OnLoaded(null, App.CurrentPage);
				}
			});
		}

		#region ShowPage methods

		public static void ShowWelcomePage() {
			if (App.welcomePage == null)
				App.welcomePage = new WelcomePage();

			App.NavigationWindow.CurrentPage.NavigationService.Navigate(App.welcomePage);
			App.currentPage = App.welcomePage;
		}

		public static void ShowHomePage(object sender) {
			if (App.homePage == null)
				App.homePage = new HomePage();

			if (sender is Metro_Tables.Controls.TablesControl) {
				// Deactivate top control option and run animation 
				// to translate and shrink control to it's 
				// normal (wait) state
				App.homePage.MinimizeTopControl(null);
			}
			else { // This includes Welcome page
				App.NavigationWindow.CurrentPage.NavigationService.Navigate(App.homePage);
				App.currentPage = App.homePage;
			}
		}

		#endregion

		#region Window Chrome methods

		public static void Close() {
			Application.Current.Shutdown();
		}

		public static void Maximize() {
			if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
				Application.Current.MainWindow.WindowState = WindowState.Normal;
			else Application.Current.MainWindow.WindowState = WindowState.Maximized;
		}

		public static void Minimaze() {
			Application.Current.MainWindow.WindowState = WindowState.Minimized;
		}
		#endregion
	}
}
