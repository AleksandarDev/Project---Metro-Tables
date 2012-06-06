using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTables.Extensions.FormulaContracts;

namespace MetroTables.Formula.Parser {
	public abstract class ParserBase : IParserBase {
		public IParserData Data { get; set; }
		IParserData IParserBase.Data {
			get { return Data; }
			set { Data = value; }
		}


		public ParserBase(IParserData parserData) {
			Data = parserData;
		}


		public abstract Queue<IExpressionElement> Parse(Queue<Lexer.Tokens.Token> input);
		Queue<IExpressionElement> IParserBase.Parse(Queue<Lexer.Tokens.Token> input) {
			return Parse(input);
		}
	}
}
