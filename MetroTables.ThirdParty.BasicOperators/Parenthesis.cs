using System;

// Formula contracts
using MetroTables.Extensions.FormulaContracts;

// MEF Support
using System.ComponentModel.Composition;


namespace MetroTables.ThirdParty.BasicOperators {
	[Export(typeof(IExpressionOperator))]
	[ExportMetadata("Operation", "Grouping")]
	public class Parenthesis : BasicOperator {
		public Boolean IsOpenParenthesis {
			get { return (this as IExpressionOperator).Arguments[0]; }
		}

		public override string Symbol {
			get { return IsOpenParenthesis ? "(" : ")"; }
		}


		public Parenthesis() : this(true) { }
		public Parenthesis(Boolean isOpen) {
			(this as IExpressionOperator).Arguments = new dynamic[] {
				isOpen
			};

			Type = "Parenthesis";

			Precedence = Int32.MaxValue;
			ArgumentsNeeded = 0;
			IsLeftAssociativity = true;
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

			if (args != null) return args[0];
			else return (this as IExpressionOperator).Arguments[0];
		}

		public override dynamic TryParse(params dynamic[] args) {
			if (args == null) {
				throw new NullArgumentsException();
			}

			// If only operation symbol is passed
			if (args.Length == 1) {
				return new Parenthesis(args[0] == "(" ? true : false);
			}
			else throw new ArgumentMismatchException(null);
		}

		public override bool IsThisOperator(String value) {
			if (String.IsNullOrEmpty(value)) return false;

			if (value == "(" || value == ")") return true;
			else return false;
		}
	}
}
