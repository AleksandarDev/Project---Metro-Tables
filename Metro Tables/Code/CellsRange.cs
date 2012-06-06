using System.Windows;
using MetroTables.Controls;

namespace Metro_Tables.Controls {
	public class CellsRange {
		private CellsCollection source;

		public Point Start { get; set; }
		public Point End { get; set; }

		public CellsRange(CellsCollection source) {
			this.source = source;
		}

		public CellsRange(CellsCollection source, Point start, Point end) {
			this.source = source;

			Start = start;
			End = end;
		}
		// TODO Implement get and set for selection/range
	}
}