using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTables.Extensions.FormulaContracts;

namespace MetroTables.Formula.Parser {
	public interface IParserBase {
		IParserData Data { get; set; }

		Queue<IExpressionElement> Parse(Queue<Lexer.Tokens.Token> input);
	}
}
