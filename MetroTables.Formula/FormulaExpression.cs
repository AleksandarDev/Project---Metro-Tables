using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTables.Extensions.FormulaContracts;

namespace MetroTables.Formula {
	public class FormulaExpression : IFormulaExpression {
		public string Source { get; set; }
		string IFormulaExpression.Source {
			get { return Source; }
			set { Source = value; }
		}

		public Queue<IExpressionElement> Elements { get; set; }
		Queue<IExpressionElement> IFormulaExpression.Elements {
			get { return Elements; }
			set { Elements = value; }
		}


		public FormulaExpression(string source, Queue<IExpressionElement> elements) {
			Source = source;
			Elements = elements;
		}
	}
}
