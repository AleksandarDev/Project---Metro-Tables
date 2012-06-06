using System;

// Formula contracts
using MetroTables.Extensions.FormulaContracts;

namespace MetroTables.ThirdParty.BasicOperators {
	public abstract class BasicOperator : IExpressionOperator {
		#region Variables

		public String	Type				{ get; set; }
		public Int32	Precedence			{ get; set;}
		public Int32	ArgumentsNeeded		{ get; set;}
		public Boolean	IsLeftAssociativity { get; set;}

		String	IExpressionOperator.Type				{ get { return Type; } }
		Int32	IExpressionOperator.Precedence			{ get { return Precedence; } }
		Int32	IExpressionOperator.ArgumentsNeeded		{ get { return ArgumentsNeeded; } }
		Boolean IExpressionOperator.IsLeftAssociativity { get { return IsLeftAssociativity; } }

		dynamic[] IExpressionOperator.Arguments { get; set; }

		public abstract String Symbol { get; }

		#endregion

		#region Parsing

		public abstract dynamic TryParse(params dynamic[] args);
		Boolean IExpressionElement.TryParse(out dynamic result, params dynamic[] args) {
			result = null;

			try {
				result = TryParse(args);
				return true;
			}
			catch (Exception) {
				return false;
			}
		}

		public virtual Boolean IsThisOperator(String value) {
			if (String.IsNullOrEmpty(value)) return false;

			if (value == Symbol) return true;
			else return false;
		}
		Boolean IExpressionOperator.IsThisOperator(String value) {
			return IsThisOperator(value);
		}

		#endregion

		#region Evaluation

		public abstract dynamic Evaluate(params dynamic[] args);
		dynamic IExpressionElement.Evaluate(params dynamic[] args) {
			return Evaluate(args);
		}
		dynamic IExpressionElement.Evaluate() {
			IExpressionOperator @this = this as IExpressionOperator;
			return @this.Evaluate(@this.Arguments);
		}

		#endregion

		#region Other methods
		
		public override string ToString() {
			IExpressionOperator @this = this as IExpressionOperator;
			return String.Format("{0} {1} {2}", @this.Arguments[0] ?? String.Empty, Symbol, @this.Arguments[1] ?? String.Empty);
		}

		#endregion
	}
}
