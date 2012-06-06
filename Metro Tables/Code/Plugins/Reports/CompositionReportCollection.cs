using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTables.Code.Plugins.Reports {
	/// <summary>
	/// Represents a collection of CompositionReport objects
	/// </summary>
	public sealed class CompositionReportCollection : IEnumerable<CompositionReport> {
		// Variables
		private List<CompositionReport> reports;


		/// <summary>
		/// Initializes a new instance of CompositionReportCollection class that is empty
		/// </summary>
		public CompositionReportCollection() {
			this.reports = new List<CompositionReport>();
		}


		/// <summary>
		/// Adds an report to the end of CompositionReportCollection
		/// </summary>
		/// <param name="report">Report to add to the collection</param>
		public void Add(CompositionReport report) {
			this.reports.Add(report);
		}

		/// <summary>
		/// Removes the report at the specified index of CompositionReportCollection
		/// </summary>
		/// <param name="index">Index of the report to remove from the CompositionReportCollection</param>
		public void Remove(int index) {
			if (index < 0 || index > this.reports.Count - 1)
				throw new IndexOutOfRangeException("index");

			this.reports.RemoveAt(index);
		}

		/// <summary>
		/// Removes all elements from the CompositionReportCollection
		/// </summary>
		public void Clear() {
			this.reports.Clear();
		}


		/// <summary>
		/// Returns collection of all unsuccessfull reports
		/// </summary>
		/// <returns>Returns collection of all unsuccessfull reports</returns>
		public IEnumerable<CompositionReport> GetUnsuccessfull() {
			return this.Where<CompositionReport>(report => !report.IsSuccessfull);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the CompositionReportCollection
		/// </summary>
		/// <returns>Returns an enumerator that iterates through the CompositionReportCollection</returns>
		public IEnumerator<CompositionReport> GetEnumerator() {
			return this.reports.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the CompositionReportCollection
		/// </summary>
		/// <returns>Returns an enumerator that iterates through the CompositionReportCollection</returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}


		#region Properties

		/// <summary>
		/// Gets whether are all reports successfull
		/// </summary>
		public bool IsAllSuccessfull {
			get { return this.All<CompositionReport>(report => report.IsSuccessfull); }
		}

		#endregion
	}
}