using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MetroTables.Extensions.FormulaContracts;

namespace MetroTables.Common.Interfaces {
	public interface ICell {
		bool IsDefault { get; }
		bool IsInEditMode { get; }
		bool IsSelected { get; }

		Label Label { get; }

		int PositionX { get; set; }
		int PositionY { get; set; }


		void SetValue(object value);
		void SetFormula(IFormulaExpression formula);
		IFormulaExpression GetFormula();

		void Select();
		void Deselect();

		void ActivateEditMode();
		void CancelEditMode();
		void EndEditMode();
	}
}
