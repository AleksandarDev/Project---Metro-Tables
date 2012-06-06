using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTables.Extensions.FormulaContracts;

namespace MetroTables.Formula.Evaluator {
	public class EvaluatorBase : IEvaluatorBase {
		public virtual Extensions.FormulaContracts.IExpressionOperand Evaluate(Queue<Extensions.FormulaContracts.IExpressionElement> source) {
			// Reverse Polish notation algorithm can be found here
			// http://en.wikipedia.org/wiki/Reverse_Polish_notation
			if (source == null) throw new ArgumentNullException("source", "Source must contain at least one expression element");

			// Stack for saving operands
			Stack<IExpressionOperand> operandsStack = new Stack<IExpressionOperand>();

			// While there are input tokens left
			while (source.Count > 0) {
				// If the token is a value
				//		Push it onto the stack.
				if (source.Peek() is IExpressionOperand) {
					operandsStack.Push(source.Dequeue() as IExpressionOperand);
				}
				// Otherwise, the token is an operator (operator here includes both operators, and functions)
				else {
					IExpressionOperator @operator = source.Dequeue() as IExpressionOperator;

					// It is known a priori that the operator takes n arguments.
					// If there are fewer than n values on the stack
					//		(Error) The user has not input sufficient values in the expression.
					if (operandsStack.Count < @operator.ArgumentsNeeded) {
						throw new EvaluatorException("Evaluator exception. Missing some operands for operation!", null, @operator);
					}
					else {
						// Else, Pop the top n values from the stack to operator
						// This needs to be read FIFO and here we have stack (LIFO) because
						// of formula operands ordering
						for (int index = @operator.ArgumentsNeeded - 1; index >= 0; --index) {
							@operator.Arguments[index] = operandsStack.Pop();
						}

						// Evaluate the operator, with the values as arguments and
						// push the returned results, if any, back onto the stack
						operandsStack.Push(@operator.Evaluate());
					}
				}
			}

			// If there are more values in the stack
			//		(Error) The user input has too many values.
			if (operandsStack.Count != 1) {
				throw new EvaluatorException("Evaluator exception. Result is not invalid format!", null, null);
			}
			else {
				// If there is only one value in the stack
				// that value is the result of the calculation
				return operandsStack.Pop();
			}
		}
	}
}
