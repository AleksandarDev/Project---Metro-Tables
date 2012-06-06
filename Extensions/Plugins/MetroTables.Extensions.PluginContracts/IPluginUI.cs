using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace MetroTables.Extensions.PluginContracts {
	/// <summary>
	/// Used for Plugin UI classes
	/// </summary>
	public interface IPluginUI {
		#region Menu UI

		/// <summary>
		/// Gets whether Menu UI is available for this plugin
		/// </summary>
		bool IsMenuUIAvailable { get; }
		
		/// <summary>
		/// Gets or sets Menu UI visibility.
		/// </summary>
		bool IsMenuUIVisible { get; set; }

		/// <summary>
		/// Gets title of menu item.
		/// </summary>
		string MenuTitle { get; }

		/// <summary>
		/// Gets UserControl that needs to be added to menu of application.
		/// Every plugin can only have one menu item.
		/// </summary>
		/// <param name="owner">Object that represents owner control</param>
		/// <returns>Control for application menu item.</returns>
		UserControl GetMenuUI(object owner);

		#endregion
	}
}
