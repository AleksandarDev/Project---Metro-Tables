using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTables.Formula.Lexer.Tokens;

namespace MetroTables.Formula.Lexer {
	public abstract class LexerBase : ILexerBase {
		#region Variables

		/// <summary>
		/// Gets list of registered Token Definitions
		/// </summary>
		public IEnumerable<TokenDefinition> TokenDefinitions {
			get { return this.tokenDefinitions; }
		}
		protected List<TokenDefinition> tokenDefinitions;

		#endregion

		#region Constructor methods

		/// <summary>
		/// Creates new Base object
		/// </summary>
		public LexerBase() {
			// Variable declaration
			this.tokenDefinitions = new List<TokenDefinition>();
		}

		#endregion

		#region Token definitions

		/// <summary>
		/// Adds Token Definitions to Lexer Generator
		/// </summary>
		/// <param name="definitions">Array of Token Definitions to add</param>
		public virtual void AddTokenDefinition(params TokenDefinition[] definitions) {
			for (int index = 0; index < definitions.Length; index++) {
				AddTokenDefinition(definitions[index]);
			}
		}

		/// <summary>
		/// Adds Token Definition to Lexer Generator
		/// </summary>
		/// <param name="definition">Token Definition to add</param>
		public virtual void AddTokenDefinition(TokenDefinition definition) {
			Contract.Requires(this.tokenDefinitions != null);
			Contract.Requires(definition != null);

			(this.tokenDefinitions as List<TokenDefinition>).Add(definition);
		}

		#region Interface implementation

		/// <summary>
		/// Adds Token Definitions to Lexer Generator
		/// </summary>
		/// <param name="definitions">Array of Token Definitions to add</param>
		void ILexerBase.AddTokenDefinition(params Tokens.TokenDefinition[] definitions) {
			AddTokenDefinition(definitions);
		}

		/// <summary>
		/// Adds Token Definition to Lexer Generator
		/// </summary>
		/// <param name="definition">Token Definition to add</param>
		void ILexerBase.AddTokenDefinition(TokenDefinition definition) {
			AddTokenDefinition(definition);
		}

		#endregion

		#endregion

		#region Lexical analisys

		/// <summary>
		/// Generates Lexer Tokens using registered Token Definitions
		/// </summary>
		/// <param name="source">Source from which to generate Lexer Tokens</param>
		/// <returns>All found Lexer Tokens generated using registered Token Definitions from given source</returns>
		public abstract Queue<Tokens.Token> Analyze(string source);

		/// <summary>
		/// Generates Lexer Tokens using registered Token Definitions
		/// </summary>
		/// <param name="source">Source from which to generate Lexer Tokens</param>
		/// <returns>All found Lexer Tokens generated using registered Token Definitions from given source</returns>
		Queue<Token> ILexerBase.Analyze(String source) {
			return Analyze(source);
		}

		#endregion
	}
}
