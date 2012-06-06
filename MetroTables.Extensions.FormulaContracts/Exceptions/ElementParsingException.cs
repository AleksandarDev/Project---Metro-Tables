using System;

namespace MetroTables.Extensions.FormulaContracts {
	public class ElementParsingException : EvaluatorException {
		public ElementParsingException()
			: this(null) { }
		public ElementParsingException(IExpressionElement element)
			: this(element, null) { }
		public ElementParsingException(IExpressionElement element, Exception inner)
			: base("Parsing exception. Can't parse element - error occured while parsing element.", inner, element) { }
	}
}
