using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTables.Extensions.FormulaContracts;
using MetroTables.Formula.Parser;

namespace MetroTables.Code.Formula {
	/// <summary>
	/// Represents collection of available operators and operands
	/// </summary>
	public class ParserData : IParserData {
		[ImportMany]
		IEnumerable<IExpressionOperator> IParserData.Operators { get; set; }

		[ImportMany]
		IEnumerable<IExpressionOperand> IParserData.Operands { get; set; }

		private CompositionContainer container;


		public ParserData(String path) {
			if (path == null) throw new ArgumentNullException("path", "Path can't be null!");


			ComposeParts(path);
		}


		// TODO: Remove duplicate code with PluginManager
		private void ComposeParts(String path) {
			// Loads directory assemblies
			DirectoryCatalog catalog = new DirectoryCatalog(path);
			this.container = new CompositionContainer(catalog);

			//Fill the imports of this object
			try {
				// Assigns Exports to Imports in this object
				this.container.ComposeParts(this);
			}
			catch (CompositionException compositionException) {
				throw new ParserException("Can't compose parser data", compositionException);
			}
			catch (Exception ex) {
				throw new ParserException("Unknown exception occured while composing parser data", ex);
			}
		}
	}
}
