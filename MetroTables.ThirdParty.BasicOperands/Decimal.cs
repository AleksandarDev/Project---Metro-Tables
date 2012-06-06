using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTables.Extensions.FormulaContracts;

namespace MetroTables.ThirdParty.BasicOperands {
	[Export(typeof(IExpressionOperand))]
	[ExportMetadata("DataType", "Decimal")]
	public class Decimal : IExpressionOperand {
		public System.Decimal Value { get; set; }
		dynamic IExpressionOperand.Value {
			get { return Value; }
			set { Value = value; }
		}


		// Used by MEF for importing
		public Decimal() : this(0m) { }
		public Decimal(System.Decimal value) {
			(this as IExpressionOperand).Value = value;
		}


		Boolean IExpressionElement.TryParse(out dynamic result, params dynamic[] args) {
			result = null;

			//throw new NullArgumentsException(this);
			//throw new ArgumentMismatchException(this);
			if (args == null) return false;
			if (args.Length != 1) return false;

			try {
				result = new Decimal(System.Decimal.Parse(args[0]));
			}
			catch (Exception) {
				return false;
			}

			return true;
		}

		dynamic IExpressionElement.Evaluate(params dynamic[] args) {
			return Value;
		}
		dynamic IExpressionElement.Evaluate() {
			IExpressionOperand @this = this as IExpressionOperand;
			return (this as IExpressionOperand).Evaluate(Value);
		}


		public static Decimal operator +(Decimal a, Decimal b) {
			return new Decimal(a.Value + b.Value);
		}
		public static Decimal operator -(Decimal a, Decimal b) {
			return new Decimal(a.Value - b.Value);
		}
		public static Decimal operator *(Decimal a, Decimal b) {
			return new Decimal(a.Value * b.Value);
		}
		public static Decimal operator /(Decimal a, Decimal b) {
			return new Decimal(a.Value / b.Value);
		}
		public static Decimal operator ^(Decimal a, Decimal b) {
			return new Decimal((System.Decimal)Math.Pow((Double)a.Value, (Double)b.Value));
		}
		public static Decimal operator %(Decimal a, Decimal b) {
			return new Decimal(a.Value % b.Value);
		}



		public override string ToString() {
			return String.Format("{0}", Value);
		}
	}
}
