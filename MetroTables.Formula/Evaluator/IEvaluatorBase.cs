using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTables.Extensions.FormulaContracts;

namespace MetroTables.Formula.Evaluator {
	public interface IEvaluatorBase {
		IExpressionOperand Evaluate(Queue<IExpressionElement> input);
	}
}
