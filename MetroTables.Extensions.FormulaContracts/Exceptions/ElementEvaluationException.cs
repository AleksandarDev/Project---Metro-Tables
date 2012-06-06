using System;

namespace MetroTables.Extensions.FormulaContracts {
	public class ElementEvaluationException : EvaluatorException {
		public ElementEvaluationException()
			: this(null) { }
		public ElementEvaluationException(IExpressionElement element)
			: this(element, null) { }
		public ElementEvaluationException(IExpressionElement element, Exception inner)
			: base("Evaluation exception. Can't evaluate operation - error occured while evaluating operation.", inner, element) { }
	}
}
