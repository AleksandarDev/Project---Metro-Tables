using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MetroTables.Formula.Lexer;
using MetroTables.Formula.Lexer.Tokens;

namespace MetroTables.Code.Formula {
	public class CustomLexer : LexerBase {
		/// <summary>
		/// Generates Lexer Tokens using registered Token Definitions
		/// </summary>
		/// <param name="source">Source from which to generate Lexer Tokens</param>
		/// <returns>All found Lexer Tokens generated using registered Token Definitions from given source</returns>
		public override Queue<MetroTables.Formula.Lexer.Tokens.Token> Analyze(string source) {
			Contract.Requires(this.tokenDefinitions != null);
			Contract.Requires(source != null);

#if DEBUG
			System.Diagnostics.Debug.WriteLine(String.Format("Lexical tokenization for \t``{0}``", source));
			DateTime startTime = DateTime.Now;
#endif

			Queue<Token> queue = new Queue<Token>();
			int currentPosition = 0;

			// Starts search loop
			while (currentPosition < source.Length) {
				TokenDefinition matchedDefinition = null;
				int matchLength = 0;

				// Goese through all registered token definitions and tryes to find a match
				for (int index = 0; index < this.tokenDefinitions.Count; index++) {
					Match match = this.tokenDefinitions[index].Expression.Match(source, currentPosition);

					// If match was found and starts at current position
					if (match.Success && (match.Index - currentPosition) == 0) {
						matchedDefinition = this.tokenDefinitions[index];
						matchLength = match.Length;

						// We don't need to find any more matches so we break loop
						break;
					}
				}

				// If match is found yields new Lexer Token
				if (matchedDefinition != null) {
					// Create matched string
					string value = source.Substring(currentPosition, matchLength);

					// If match is not suppose to be ignored
					if (!matchedDefinition.IsIgnored) {
						queue.Enqueue(new Token(matchedDefinition.Type, value, currentPosition));
					}
					else System.Diagnostics.Debug.WriteLine(String.Format("Lexer - Ignored '{0}' at index {1} from source [{2}]", source[currentPosition], currentPosition, source));
				}
				else {
					throw new LexerException(String.Format("Unrecognizes symbol '{0}' at index {1} from source [{2}]!", source[currentPosition], currentPosition, source), source, currentPosition);
				}

				currentPosition += matchLength;
			}

			Contract.Ensures(Contract.Result<Queue<Token>>() != null);

#if DEBUG
			foreach (Token token in queue) {
				System.Diagnostics.Debug.WriteLine("\t" + token.ToString());
			}

			TimeSpan timeToYieldResult = DateTime.Now - startTime;
			System.Diagnostics.Debug.WriteLine("Lexical tokenization last for " + timeToYieldResult.ToString() + "\n");
#endif

			return queue;
		}
	}
}
