using System;

namespace MetroTables.Extensions.FormulaContracts {
	public class NullArgumentsException : EvaluatorException {
		public NullArgumentsException()
			: this(null) { }
		public NullArgumentsException(IExpressionElement element)
			: this(element, null) { }
		public NullArgumentsException(IExpressionElement element, Exception inner)
			: base("Evaluation exception. No arguments recieved (null).", inner, element) { }
	}
}
