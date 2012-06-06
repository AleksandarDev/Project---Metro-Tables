using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTables.Formula.Lexer.Tokens {
	/// <summary>
	/// Represents the result of lexical analisys
	/// </summary>
	public class Token {
		/// <summary>
		/// Gets or sets position on Lexer source (start position)
		/// </summary>
		public virtual Int32 Position { get; set; }

		/// <summary>
		/// Gets or sets type of Token (e.g. Number, Text, etc.)
		/// </summary>
		public virtual TokenTypes Type { get; set; }

		/// <summary>
		/// Gets or sets value/content of token
		/// </summary>
		public virtual String Value { get; set; }


		/// <summary>
		/// Creates new Token object
		/// </summary>
		public Token() : this(TokenTypes.End, null, -1) { }
		/// <summary>
		/// Creates new Token object
		/// </summary>
		/// <param name="type">Type of Token (e.g. "Number", "Text", etc.)</param>
		/// <param name="value">Value/Content of token</param>
		/// <param name="position">Position on Lexer source (start position)</param>
		public Token(TokenTypes type, String value, Int32 position) {
			Type = type;
			Value = value ?? String.Empty;
			Position = position;
		}


		/// <summary>
		/// Generates string from this object
		/// </summary>
		/// <returns>Returns string with important information about object</returns>
		public override string ToString() {
			return String.Format("'{0}'\t{1} at index {2}", Value, Type.ToString(), Position);
		}
	}
}
