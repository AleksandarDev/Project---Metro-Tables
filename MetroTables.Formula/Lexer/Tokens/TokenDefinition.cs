using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MetroTables.Formula.Lexer.Tokens {
	/// <summary>
	/// Represents Token Definition used in lexical analisys
	/// </summary>
	public class TokenDefinition {
		/// <summary>
		/// Gets or sets if Tokens defined by this Token Definition are ignored in Lexical analisys
		/// </summary>
		public virtual Boolean IsIgnored { get; set; }

		/// <summary>
		/// Gets or sets Regex that defines Tokens in Lexical analisys
		/// </summary>
		public virtual Regex Expression { get; set; }

		/// <summary>
		/// Gets or sets what type matched Token will be
		/// </summary>
		public virtual TokenTypes Type { get; set; }


		/// <summary>
		/// Creates new TokenDefinition object
		/// </summary>
		public TokenDefinition() : this(TokenTypes.End, null) { }
		/// <summary>
		/// Creates new TokenDefinition object
		/// </summary>
		/// <param name="type">Type that matched Token will be</param>
		/// <param name="match">Regex that defines Tokens in Lexical analisys</param>
		/// <param name="isIgnored">Defines if Tokens defined by this Token Definition are ignores in Lexical analisys. This is true if you want to skip matched Tokens</param>
		public TokenDefinition(TokenTypes type, Regex match, Boolean isIgnored = false) {
			Type = type;
			Expression = match;
			IsIgnored = isIgnored;
		}
	}
}
