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
using System.Windows.Shell;
using Metro_Tables.Code;

namespace Metro_Tables {
	public partial class WelcomePage : Page {
		public WelcomePage() {
			InitializeComponent();

			labelState.Content = "Loading . . .";
		}


		public void ChangeStatus(String status) {
			this.Dispatcher.Invoke(new Action(() => { this.labelState.Content = status; }));
		}

		// This method is called from StartAnimation Storyboard
		public void WelcomeAnimationOnCompleted() {
			App.NavigationWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
			App.OnLoaded += MainManagerOnOnLoaded;
			App.Load();
		}

		private void MainManagerOnOnLoaded(object sender, Page currentPage) {
			App.NavigationWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;

			Storyboard endAnimationStoryboard = (Storyboard)FindResource("EndAnimation");
			endAnimationStoryboard.Completed += (s, e) => App.ShowHomePage(this);
			BeginStoryboard(endAnimationStoryboard);
		}
	}
}
