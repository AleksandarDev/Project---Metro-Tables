using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MetroTables.Formula.Lexer.Tokens;

namespace MetroTables.Code.Formula {
	public class CustomTokenDefinitions {
		public static TokenDefinition[] Definitions = new TokenDefinition[] {
				//
				// Operators
				//
				// Arithmetic operator 
				// -, +, *, /, %, ^,
				new TokenDefinition(TokenTypes.Operator, 
					new Regex(@"[\-+*/%\^]" , RegexOptions.IgnoreCase | RegexOptions.Singleline)),

				// Comparison operator 
				// >, <, =, >=, <=, <>
				new TokenDefinition(TokenTypes.Operator, 
					new Regex(@"[><=]|(>=)|(<=)|(<>)", RegexOptions.IgnoreCase | RegexOptions.Singleline)),

				// Text concatenation operator 
				// &, ','
				new TokenDefinition(TokenTypes.Operator, 
					new Regex(@"[&,]", RegexOptions.IgnoreCase | RegexOptions.Singleline)),

				// Reference operator 
				// :, ' '
				new TokenDefinition(TokenTypes.Operator, 
					new Regex(@"[:\s]", RegexOptions.IgnoreCase | RegexOptions.Singleline)),

				// Grouping operator 
				// (, ), [, ]
				new TokenDefinition(TokenTypes.Operator, 
					new Regex(@"[\(\)\[\]]" , RegexOptions.IgnoreCase | RegexOptions.Singleline)),

				//
				// Literals
				//
				// Numbers 
				// eg. 125, 125.25, 0.25
				new TokenDefinition(TokenTypes.Operand, 
					new Regex(@"[-+]?\d+(\,\d+)?", RegexOptions.IgnoreCase | RegexOptions.Singleline)),

				// Variables 
				// eg. BAR, F00
				new TokenDefinition(TokenTypes.Operator, 
					new Regex(@"[a-z_][\w]*", RegexOptions.IgnoreCase | RegexOptions.Singleline)),

				// String 
				// eg. "bar", "foo"
				// String needs to be registered before function because function is actualy string without quotes
				new TokenDefinition(TokenTypes.Operand, 
					new Regex("(\")(?:.)*?\\1", RegexOptions.IgnoreCase | RegexOptions.Singleline)),

				// Other letter text (functions) 
				// eg. SIN, COS, IF
				new TokenDefinition(TokenTypes.Operator, 
					new Regex(@"(?:[a-z]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline)),

				//
				// Other characters (not recognized)
				//
				// Only if all previous tokens were not found
				new TokenDefinition(TokenTypes.Other, 
					new Regex(@".", RegexOptions.IgnoreCase | RegexOptions.Singleline))
			};
	}
}
