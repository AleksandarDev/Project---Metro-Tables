using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MetroTables.Common.Interfaces {
	public interface ISheetContract {
		Grid Grid { get; }

		double DefaultCellHeight { get; }
		double DefaultCellWidth { get; }

		void ApplyToSelection(Action<ICell> action);
	}
}
