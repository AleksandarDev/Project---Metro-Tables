using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTables.Extensions.FormulaContracts {
	public interface IExpressionOperator : IExpressionElement {
		string Type { get; }

		int Precedence { get; }
		bool IsLeftAssociativity { get; }

		int ArgumentsNeeded { get; }

		bool IsThisOperator(string operatorType);
		dynamic[] Arguments { get; set; }
	}
}
