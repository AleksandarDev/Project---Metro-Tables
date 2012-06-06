using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Metro_Tables;
using MetroTables.Code.Plugins.Reports;

namespace MetroTables.Code.Plugins {
	// TODO comment
	// TODO implement
	public class PluginEventArgs<T> : EventArgs {
		public IEnumerable<T> Plugins { get; set; }
	}

	// TODO comment
	public delegate void PluginEventHandler<T>(object sender, PluginEventArgs<T> args);

	/// <summary>
	/// Class that contains all needed methods for plugins manipulation
	/// </summary>
	public class PluginManager<T> {
		protected IEnumerable<DirectoryCatalog> catalogs;

		/// <summary>
		/// Creates new object of Plugins class and composes plugins from path
		/// </summary>
		/// <param name="path">Location of plugins folder</param>
		/// <exception cref="System.ArgumentNullException">path argument is null</exception>
		/// <permission cref="SecurityAction.Demand">Full trust</permission>
		[PermissionSet(SecurityAction.Demand, Name="FullTrust")]
		public PluginManager(String path) {
			// Check arguments
			if (path == null)
				throw new ArgumentNullException("path", "Path can't be null!");

			// Save arguments
			Path = path;

			// Initialize system directory watcher
			InitializeWatcher(path);
		}

		/// <summary>
		/// Initializes and attaches events to directory watcher
		/// </summary>
		/// <param name="path">Location of plugins folder</param>
		protected virtual void InitializeWatcher(string path) {
			FileSystemWatcher watcher = new FileSystemWatcher() {
				// Set path to given directory
				Path = path,

				// Setup notify filter to watch for LastWrite timer,
				// size change, renaming file or directory
				NotifyFilter = NotifyFilters.LastWrite |
									NotifyFilters.Size |
									NotifyFilters.FileName |
									NotifyFilters.DirectoryName,
				// Watch only for .dll files					
				Filter = @"*.dll",

				// Begin watching
				EnableRaisingEvents = true
			};

			// Attach custom events
			watcher.Created += HandleWatcherChanged;
			watcher.Changed += HandleWatcherChanged;
			watcher.Deleted += HandleWatcherChanged;
		}

		// TODO comment
		protected virtual void HandleWatcherChanged(object sender, FileSystemEventArgs e) {
			System.Diagnostics.Debug.WriteLine("File/Directory Changed!");

			ComposeParts(this, Path);

			if (PluginUpdated != null) {
				// TODO add event args
				PluginUpdated(this, new PluginEventArgs<T>() { Plugins = PluginsList });
			}
		}

		public virtual void Initialize() {
			ComposeParts(this, Path);

			if (PluginUpdated != null) {
				// TODO add event args
				PluginUpdated(this, new PluginEventArgs<T>() { Plugins = PluginsList });
			}
		}

		/// <summary>
		/// Composes all available parts from given paths
		/// </summary>
		/// <param name="target">Object which needs to get composed parts (has Import attributes)</param>
		/// <param name="paths">The path to the directory to scan for assemblies to add compose</param>
		/// <returns>Returns CompositionReportCollection of composition</returns>
		public static CompositionReportCollection ComposeParts(object target, params string[] paths) {
			// Create new collection of composition reports
			CompositionReportCollection reports = new CompositionReportCollection();

			// Check arguments
			if (target == null)
				reports.Add(new CompositionReport(target, null, "target is null", new ArgumentNullException("target")));
			if (paths == null)
				reports.Add(new CompositionReport(target, null, "paths is null", new ArgumentNullException("paths")));

			// Creates directories catalog
			AggregateCatalog catalog = new AggregateCatalog(
				BuildCatalogCollection(paths, reports).ToArray());

			/// Composes catalogs
			ComposeCatalog(target, catalog, reports);

			// Returns reports
			return reports;
		}

		/// <summary>
		/// Composes parts from given catalog collection for given target object
		/// </summary>
		/// <param name="target">Target object for which to compose parts</param>
		/// <param name="catalog">Collection of catalogs from which to load parts</param>
		/// <param name="reports">Reports collection</param>
		protected static void ComposeCatalog(object target, AggregateCatalog catalog, CompositionReportCollection reports) {
			// Creates container from catalogs list
			CompositionContainer container = new CompositionContainer(catalog);

			//Fill the imports of the target object
			try {
				// UI compositions requires STA thread
				App.Current.Dispatcher.Invoke(() => {
					// Assigns Exports to Imports in this object
					container.ComposeParts(target);
				});
			}
			catch (CompositionException compositionException) {
				reports.Add(new CompositionReport(target, null, "Can't compose container", compositionException));
			}
			catch (Exception ex) {
				reports.Add(new CompositionReport(target, null, "Unknown exception occured", ex));
			}
		}

		/// <summary>
		/// Builds a list of directory catalogs from given directory path strings
		/// </summary>
		/// <param name="paths">Arry of string directory paths</param>
		/// <param name="reports">Reports collection</param>
		/// <returns>Yield returns DirectoryCatalog for each  of valid directory path</returns>
		protected static IEnumerable<DirectoryCatalog> BuildCatalogCollection(string[] paths, CompositionReportCollection reports) {
			// Creates catalogs list from paths
			foreach (string path in paths) {
				if (path == null) {
					reports.Add(new CompositionReport(null, path, "path is null", null));
					continue;
				}
				if (!System.IO.Directory.Exists(path)) {
					reports.Add(new CompositionReport(null, "path not found", null));
					continue;
				}

				DirectoryCatalog catalog;

				// Try to create catalog
				try {
					catalog = new DirectoryCatalog(path);
					reports.Add(new CompositionReport(null, path, "Directory successfully added"));
				}
				catch (UnauthorizedAccessException unauthorizedAccessException) {
					reports.Add(new CompositionReport(null, path, "The caller doesn not have the required permission.", unauthorizedAccessException));
					continue;
				}
				catch (Exception ex) {
					reports.Add(new CompositionReport(null, path, "Unknown exception occured", ex));
					continue;
				}

				yield return catalog;
			}
		}

		#region Properties

		/// <summary>
		/// Path of plugins to load
		/// </summary>
		public string Path { get; private set; }

		/// <summary>
		/// List of available plugins
		/// </summary>
		[ImportMany]
		public IEnumerable<T> PluginsList { get; set; }

		// TODO comment
		public event PluginEventHandler<T> PluginUpdated;

		#endregion
	}
}