using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTables.Extensions.FormulaContracts;
using MetroTables.Formula.Parser;

namespace MetroTables.Code.Formula {
	public class CustomParser : ParserBase {

		public CustomParser(IParserData data) 
			: base(data) { }


		public override Queue<Extensions.FormulaContracts.IExpressionElement> Parse(Queue<MetroTables.Formula.Lexer.Tokens.Token> input) {
			Queue<IExpressionElement> result = new Queue<IExpressionElement>();

			// TODO Run some perser filter to remove unnecessary elements
			ParseInput(input, out result);
			ShuntingYardSort(result, out result);

			return result;
		}

		protected void ParseInput(Queue<MetroTables.Formula.Lexer.Tokens.Token> input, out Queue<IExpressionElement> result) {
			result = new Queue<IExpressionElement>();

			// Cancel parsing if no input
			if (input == null) return;

			int previousCount = input.Count;

			// Parsing all tokens to operators and operands
			while (input.Count > 0) {
				if (input.Peek().Type == MetroTables.Formula.Lexer.Tokens.TokenTypes.Operator) {
					// Parsing operators
					foreach (IExpressionOperator expressionOperator in Data.Operators) {
						// Check if current operator symbol matches token value
						if (expressionOperator.IsThisOperator(input.Peek().Value)) {
							// Tryes to parse token using current operator
							dynamic parseResult;
							bool isParseResultValid = expressionOperator.TryParse(out parseResult, input.Dequeue().Value);

							// If operator parsing isn't valid, continue to next operator
							if (isParseResultValid) {
								result.Enqueue(parseResult);
							}

							break;
						}
					}
				}
				else if (input.Peek().Type == MetroTables.Formula.Lexer.Tokens.TokenTypes.Operand) {
					// Parsing operand
					foreach (IExpressionOperand expressionOperand in Data.Operands) {
						// Tryes to parse token using current operand
						dynamic parseResult;
						bool isParseResultValid = expressionOperand.TryParse(out parseResult, input.Dequeue().Value);

						// If operand parsing isn't valid, continue to next operand
						if (isParseResultValid) {
							result.Enqueue(parseResult);

							break;
						}
					}
				}
				else throw new ParserException("Matching operator or operand not found!", null);

				// Avoiding stack overflow when token is operator or operand 
				// but it isn't on lists of operators and operands
				if (previousCount == input.Count) {
					throw new ParserException("StackOverflow occured while parsing input. Check Lexer definitions and Parser operators/operands.", null, input.Peek());
				}
				else previousCount = input.Count;
			}
		}

		protected static void ShuntingYardSort(Queue<IExpressionElement> input, out Queue<IExpressionElement> result) {
			// Algorithm from
			// http://en.wikipedia.org/wiki/Shunting_yard_algorithm

			Queue<IExpressionElement> queue = input as Queue<IExpressionElement>;

			// Variables used for sorting
			Queue<IExpressionElement> sorted = new Queue<IExpressionElement>();
			Stack<IExpressionOperator> operatorStack = new Stack<IExpressionOperator>();

			while (queue.Count > 0) {
				// Check if next element is Expression Operand if so
				// enqueues it and dequeues it from input queue
				if (queue.Peek() is IExpressionOperand) {
					sorted.Enqueue(queue.Dequeue());
				}
				// Checks if next elements is Expresion Operator or 
				// grouping elements (parenthesis)
				else if (queue.Peek() is IExpressionOperator) {
					// Dequeue current operator
					IExpressionOperator @operator = queue.Dequeue() as IExpressionOperator;

					// Check is current operator is grouping operator
					if (@operator.Type == "Parenthesis") {
						// Dequeues parenthesis from input queue
						//Evaluator.Parenthesis parenthesis = @operator as Evaluator.Parenthesis;

						// Left parenthesis push on operators stack
						if (@operator.Arguments[0]) {
							operatorStack.Push(@operator);
						}
						// Right parenthesis need further examination
						else {
							// Pop operators to result queue until reached
							// first left parenthesis
							while (operatorStack.Count > 0 &&
									!((operatorStack.Peek().Type == "Parenthesis") &&
									operatorStack.Peek().Arguments[0])) {
								sorted.Enqueue(operatorStack.Pop());
							}

							// If any operators left
							if (operatorStack.Count > 0) {
								// Pops left parenthesis from operators stack
								operatorStack.Pop();
							}
							// If all operators are poped and no parentheses
							// left - throw new parentheses mismatch exception'
							else {
								throw new ParserException("Parser sorting exception. Parentheses mismatched - please check formula for errors.");
							}
						}
					}
					else {
						// Check for operator precedence and associativity
						while (operatorStack.Count > 0 &&
								!(operatorStack.Peek().Type == "Parenthesis") &&
								((operatorStack.Peek().IsLeftAssociativity && operatorStack.Peek().Precedence >= @operator.Precedence) ||
								(!operatorStack.Peek().IsLeftAssociativity && operatorStack.Peek().Precedence > @operator.Precedence))) {
							sorted.Enqueue(operatorStack.Pop());
						}

						// Push current operator to operators stack
						operatorStack.Push(@operator);
					}
				}
				else {
					// This exception happens when current queue 
					// element isn't operand nor operator
					throw new ParserException("Parser sorting exception. Unexpected element - please check Lexer and Parser for errors.", null, queue.Peek());
				}
			}

			// Pop entire operators stack to result queue
			while (operatorStack.Count > 0) {
				if (operatorStack.Peek().Type == "Parenthesis") {
					// This exception happens when current
					// stach element is parenthesis, in this
					// point of algorith there shouldn't be any
					// parenthesis left - mismatched parentheses
					throw new ParserException("Parser sorting exception. Parentheses mismatched - please check formula for errors.", null, operatorStack.Peek());
				}
				sorted.Enqueue(operatorStack.Pop());
			}


			result = sorted;
		}
	}
}
