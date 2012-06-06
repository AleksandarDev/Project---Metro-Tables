using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTables.Formula.Lexer {
	public interface ILexerBase {
		void AddTokenDefinition(Tokens.TokenDefinition definition);
		void AddTokenDefinition(params Tokens.TokenDefinition[] definitions);

		Queue<Tokens.Token> Analyze(string source);
	}
}
