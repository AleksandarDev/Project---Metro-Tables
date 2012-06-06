using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTables.Extensions.FormulaContracts {
	public interface IFormulaExpression {
		string Source { get; set; }
		Queue<IExpressionElement> Elements { get; set; }
	}
}
