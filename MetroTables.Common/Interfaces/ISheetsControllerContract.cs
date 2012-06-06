using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GemBox.Spreadsheet;

namespace MetroTables.Common.Interfaces {
	public interface ISheetsControllerContract {
		ExcelFile SourceDocument { get; }

		string Name { get; set; }
		string Path { get; }

		bool OpenFromFile(string path);

		ISheetContract GetSheet(int index);
		ISheetContract GetSelectedSheet();
		int GetSelectedSheetIndex();

		void SetVisibility(int sheetIndex, bool isVisible);
	}
}
