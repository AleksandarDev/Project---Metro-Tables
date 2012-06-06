using System;

// Formula contracts
using MetroTables.Extensions.FormulaContracts;

// MEF Support
using System.ComponentModel.Composition;

namespace MetroTables.ThirdParty.BasicOperators {
	[Export(typeof(IExpressionOperator))]
	[ExportMetadata("Operation", "Subtraction")]
	public class Subtraction : BasicOperator {
		public override string Symbol { get { return "-"; } }


		public Subtraction() : this(null, null) { }
		public Subtraction(dynamic left, dynamic right) {
			(this as IExpressionOperator).Arguments = new[] {
				left,
				right
			};

			Type = "Subtraction";

			ArgumentsNeeded = 2;
			Precedence = 2;
			IsLeftAssociativity = true;
		}


		public override dynamic TryParse(params dynamic[] args) {
			if (args == null) {
				throw new NullArgumentsException();
			}

			// If only operation symbol is passed
			if (args.Length == 1) {
				if (!IsThisOperator(args[0])) {
					throw new ElementParsingException(null, new Exception("Operator symbol not recognized '" + args[0] + "'"));
				}
				return new Subtraction();
			}
			// If two operands and symbol is passed
			else if (args.Length == 3) {
				if (!IsThisOperator(args[1])) {
					throw new ElementParsingException(null, new Exception("Operator symbol not recognized '" + args[1] + "'"));
				}
				return new Subtraction(args[0], args[2]);
			}
			else throw new ArgumentMismatchException(null);
		}

		public override dynamic Evaluate(params dynamic[] args) {
			if (args == null && (this as IExpressionOperator).Arguments == null) {
				// This exception happens when no
				// arguments are passed to method (null)
				throw new NullArgumentsException(this);
			}

			if (args.Length != ArgumentsNeeded) {
				// This exception happens when number
				// of argument doesn't match needed number
				throw new ArgumentMismatchException(this);
			}

			try {
				// Replace passed argument if null
				if (args == null) args = (this as IExpressionOperator).Arguments;

				return args[0] - args[1];
			}
			catch (Exception ex) {
				// This exception happes when error
				// appears during operator evaluation
				throw new ElementEvaluationException(this, ex);
			}
		}
	}
}
