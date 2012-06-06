using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTables.Extensions.FormulaContracts {
	public interface IExpressionElement {
		dynamic Evaluate();
		dynamic Evaluate(params dynamic[] args);

		bool TryParse(out dynamic result, params dynamic[] args);
	}
}
