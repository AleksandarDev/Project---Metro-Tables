using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTables.Formula.Lexer {
	/// <summary>
	/// Represents errors that occur during Lexer execution 
	/// </summary>
	public class LexerException : Exception {
		/// <summary>
		/// Gets or sets Lexer source that throw error
		/// </summary>
		public virtual String LexerSource { get; set; }

		/// <summary>
		/// Gets or sets Lexer position when exception thrown
		/// </summary>
		public virtual Int32 LexerPosition { get; set; }


		/// <summary>
		/// Creates new LexerException object
		/// </summary>
		public LexerException() : base() { }
		/// <summary>
		/// Creates new LexerException object
		/// </summary>
		/// <param name="message">Message of exception to show</param>
		public LexerException(String message) : base(message) { }
		/// <summary>
		/// Creates new LexerException object
		/// </summary>
		/// <param name="message">Message of exception to show</param>
		/// <param name="lexerSource">Lexer source that throw error</param>
		/// <param name="lexerPosition">Lexer position when exception thrown</param>
		public LexerException(String message, String lexerSource, Int32 lexerPosition)
			: base(message) {
				LexerSource = lexerSource;
				LexerPosition = lexerPosition;
		}
		/// <summary>
		/// Creates new LexerException object
		/// </summary>
		/// <param name="message">Message of exception to show</param>
		/// <param name="lexerSource">Lexer source that throw error</param>
		/// <param name="lexerPosition">Lexer position when exception thrown</param>
		/// <param name="innerException">The exception that is cause of this exception, or a null reference if no inner exception is specified</param>
		public LexerException(String message, String lexerSource, Int32 lexerPosition, Exception innerException)
			: base(message, innerException) {
			LexerSource = lexerSource ?? String.Empty;
			LexerPosition = lexerPosition;
		}
	}
}