using System;

namespace MetroTables.Extensions.FormulaContracts {
	public class ArgumentMismatchException : EvaluatorException {
		public ArgumentMismatchException()
			: this(null) { }
		public ArgumentMismatchException(IExpressionElement element)
			: this(element, null) { }
		public ArgumentMismatchException(IExpressionElement element, Exception inner)
			: base("Evaluation exception. Operands number mismatched - two operands needed for evaluation.", inner, element) { }
	}
}
