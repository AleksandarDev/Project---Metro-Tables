using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTables.Formula.Parser {
	/// <summary>
	/// Represents error occured while parsing tokens
	/// </summary>
	public class ParserException : Exception {
		public object Element { get; set; }


		public ParserException()
			: base() { }
		public ParserException(string message)
			: base(message) { }
		public ParserException(string message, Exception exception)
			: base(message, exception) { }
		public ParserException(string message, Exception exception, object element)
			: base(message, exception) {
			Element = element;
		}
	}
}
