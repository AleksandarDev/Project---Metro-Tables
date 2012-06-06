using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTables.Extensions.FormulaContracts;

namespace MetroTables.Formula.Parser {
	public interface IParserData {
		IEnumerable<IExpressionOperator> Operators { get; set; }
		IEnumerable<IExpressionOperand> Operands { get; set; }
	}
}
