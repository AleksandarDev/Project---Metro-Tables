using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTables.Code.Plugins.Reports {
	/// <summary>
	/// Represents an report of composition 
	/// </summary>
	public struct CompositionReport {
		/// <summary>
		/// Use this constructor upon successfull composition
		/// </summary>
		/// <param name="target">Composition target object</param>
		/// <param name="path">Composition assembly directory path</param>
		/// <param name="message">Optional. Message of report. Default parameter is "Composition successfull"</param>
		public CompositionReport(object target, string path, string message = "Composition successfull")
			: this() {
			Path = path;
			Target = target;
			Message = message;
			Exception = null;

			IsSuccessfull = true;
		}

		/// <summary>
		/// Use this contructor upon unsuccessfull composition
		/// </summary>
		/// <param name="target">Composition target object</param>
		/// <param name="path">Composition assembly directory path</param>
		/// <param name="message">Message of report</param>
		/// <param name="exception">Exception that occured on composition</param>
		public CompositionReport(object target, string path, string message, Exception exception)
			: this() {
			Path = path;
			Target = target;
			Message = message;
			Exception = exception;

			IsSuccessfull = false;
		}


		/// <summary>
		/// Creates and returns string representation of the current report
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			if (IsSuccessfull) return ToStringSuccessfull();
			else return ToStringUnsuccessfull();
		}

		/// <summary>
		/// Successfull composition string
		/// </summary>
		/// <returns>Creates and returns successfull composition string of the current report</returns>
		private string ToStringSuccessfull() {
			return String.Format("Target: \t{0}\nPath: \t{1}\nMessage: \t{2}\n",
				Target, Path, Message);
		}

		/// <summary>
		/// Unsuccessfull composition string
		/// </summary>
		/// <returns>Creates and returns unsuccessfull composition string of the current report</returns>
		private string ToStringUnsuccessfull() {
			return String.Format("Target: \t{0}\nPath: \t{1}\nMessage: \t{2}\nException: \n{3}\n",
				Target, Path, Message, Exception);
		}

		#region Properties

		/// <summary>
		/// Gets or sets path to assembly (directory)
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Gets or sets composition target object
		/// </summary>
		public object Target { get; set; }

		/// <summary>
		/// Gets or sets status of composition
		/// </summary>
		public bool IsSuccessfull { get; set; }

		/// <summary>
		/// Gets or sets message of the report
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets exception that occured during composition.
		/// </summary>
		public Exception Exception { get; set; }

		#endregion
	}
}
