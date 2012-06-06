using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTables.Formula.Lexer.Tokens {
	/// <summary>
	/// Lexer Token Types
	/// </summary>
	public enum TokenTypes {
		Operator,
		Operand,

		Other,

		End
	}
}
