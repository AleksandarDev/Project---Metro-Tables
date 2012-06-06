using System;

namespace MetroTables.Code.Converters {
	public static class HeadersHelper {
		/// <summary>
		/// Converts index (number) to string letters in Excel style
		/// </summary>
		/// <param name="index">Index to convert starting from inclusive 1 (one)</param>
		/// <returns>String representing given index in Excel format (A = 1, B = 2, AA = 27, ...)</returns>
		/// <remarks>
		/// Code source
		/// http://stackoverflow.com/questions/837155/fastest-function-to-generate-excel-column-letters-in-c
		/// </remarks>
		public static string IndexToLetters(int index) {
			// TODO: Implement this method call for all Excel like letter indexing
			if (index <= 0) return "<NA>";

			return new string((char)('A' + (((index - 1) % 26))), ((index - 1) / 26) + 1);
		}
	}
}
