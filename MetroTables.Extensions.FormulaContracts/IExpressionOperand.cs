using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTables.Extensions.FormulaContracts {
	public interface IExpressionOperand : IExpressionElement {
		dynamic Value { get; set; }
	}
}
