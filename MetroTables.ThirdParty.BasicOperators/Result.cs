using System;

// Formula contracts
using MetroTables.Extensions.FormulaContracts;

// MEF Support
using System.ComponentModel.Composition;

namespace MetroTables.ThirdParty.BasicOperators {
	[Export(typeof(IExpressionOperator))]
	[ExportMetadata("Operation", "Result")]
	public class Result : BasicOperator {
		public override string Symbol { get { return "="; } }

		public Result() : this(null) { }
		public Result(dynamic operand) {
			(this as IExpressionOperator).Arguments = new[] {
				operand
			};

			Type = "Result";

			Precedence = 1;
			ArgumentsNeeded = 1;
			IsLeftAssociativity = true;
		}


		public override dynamic TryParse(params dynamic[] args) {
			if (args == null) {
				throw new NullArgumentsException();
			}

			if (!IsThisOperator(args[0])) {
				throw new ElementParsingException(null, new Exception("Operator symbol not recognized '" + args[0] + "'"));
			}

			if (args.Length == 1) {
				return new Result();
			}
			else if (args.Length == 2) {
				return new Result(args[1]);
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

				return args[0];
			}
			catch (Exception ex) {
				// This exception happes when error
				// appears during operator evaluation
				throw new ElementEvaluationException(this, ex);
			}
		}
	}
}
