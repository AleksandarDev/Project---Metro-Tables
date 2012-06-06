using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTables.Extensions.FormulaContracts;
using MetroTables.Formula;
using MetroTables.Formula.Evaluator;
using MetroTables.Formula.Lexer.Tokens;
using MetroTables.Formula.Parser;

namespace MetroTables.Code.Formula {
	public static class FormulaManager {
		private static string componentsPath = String.Empty;

		private static CustomLexer lexer;
		private static CustomParser parser;
		private static EvaluatorBase evaluator;


		public static void Initialize(string componentsPath) {
			FormulaManager.componentsPath = componentsPath;

			FormulaManager.lexer = new CustomLexer();
			FormulaManager.lexer.AddTokenDefinition(CustomTokenDefinitions.Definitions);

			IParserData parserData = new ParserData(FormulaManager.componentsPath);
			FormulaManager.parser = new CustomParser(parserData);

			FormulaManager.evaluator = new EvaluatorBase();
		}


		public static bool IsFormula(string input) {
			if (String.IsNullOrEmpty(input)) return false;
			if (input.Length <= 1) return false;

			if (input[0] == '=') return true;

			return false;
		}

		public static IExpressionOperand Evaluate(string input) {
			if (!IsFormula(input)) return null;

			Queue<Token> lexerResult = FormulaManager.lexer.Analyze(input);
			Queue<IExpressionElement> parserResult = FormulaManager.parser.Parse(lexerResult);
			IExpressionOperand evaluatorResult = FormulaManager.evaluator.Evaluate(parserResult);

			return evaluatorResult;
		}

		public static IFormulaExpression GetExpression(string input) {
			if (!IsFormula(input)) return new FormulaExpression(input, new Queue<IExpressionElement>());

			Queue<Token> lexerResult = FormulaManager.lexer.Analyze(input);
			Queue<IExpressionElement> parserResult = FormulaManager.parser.Parse(lexerResult);

			return new FormulaExpression(input, parserResult);
		}

		public static object Evaluate(IFormulaExpression Formula) {
			return FormulaManager.evaluator.Evaluate(Formula.Elements);
		}
	}
}
