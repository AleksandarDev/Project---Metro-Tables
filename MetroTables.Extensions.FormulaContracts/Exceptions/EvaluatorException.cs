using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTables.Extensions.FormulaContracts;

namespace MetroTables.Extensions.FormulaContracts {
	public class EvaluatorException : Exception {
		public IExpressionElement Element { get; set; }


		public EvaluatorException()
			: base() { }
		public EvaluatorException(String message)
			: base(message) { }
		public EvaluatorException(String message, Exception exception)
			: base(message, exception) { }
		public EvaluatorException(String message, Exception exception, IExpressionElement element)
			: base(message, exception) {
			Element = element;
		}
	}
}
