using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MetroTables.Common.Interfaces;

namespace MetroTables.Controls {
	public class CellsCollection {
		public ISheetContract Sheet { get; private set; }
		public List<List<Cell>> Cells;

		// TODO Width cache
		// TODO Height cache
		public int Width {
			get {
				if (Cells.Count() > 0) {
					return Cells.Max(row => row.Count());
				}
				else return 0;
			}
		}
		public int Height {
			get { return Cells.Count(); }
		}


		public CellsCollection(ISheetContract sheet) {
			Sheet = sheet;
			Cells = new List<List<Cell>>();
		}


		/// <summary>
		/// Gets or sets cell
		/// </summary>
		/// <param name="rowIndex">Vertical position of cell</param>
		/// <param name="columnIndex">Horizontal position of cell</param>
		/// <returns>Returns Cell at given position or creates</returns>
		public Cell this[Int32 rowIndex, Int32 columnIndex] {
			get {
				// Check if given row and column indexes are not in range
				// of current collection or cell on that position is null
				if (this.Cells.Count <= rowIndex ||
					this.Cells[rowIndex].Count <= columnIndex ||
					this.Cells[rowIndex][columnIndex] == null) {
						// Create new cell on that position
						this[rowIndex, columnIndex] = new Cell(rowIndex, columnIndex);
				}

				// Return cell on given position
				return this.Cells[rowIndex][columnIndex];
			}
			set {
				// Create needed space for rows if needed
				while (this.Cells.Count <= rowIndex)
					Cells.Add(new List<Cell>());

				// Create needed space for columns if needed
				while (this.Cells[rowIndex].Count <= columnIndex)
					Cells[rowIndex].Add(null);

				// Create row definitions if needed
				while (Sheet.Grid.RowDefinitions.Count <= rowIndex) {
					RowDefinition rowDefinition = new RowDefinition() {
						Height = new GridLength(Sheet.DefaultCellHeight)
					};
					Sheet.Grid.RowDefinitions.Add(rowDefinition);
				}

				// Add column definitions if needed
				while (Sheet.Grid.ColumnDefinitions.Count <= columnIndex) {
					ColumnDefinition columnDefinition = new ColumnDefinition() {
						Width = new GridLength(Sheet.DefaultCellWidth)
					};
					Sheet.Grid.ColumnDefinitions.Add(columnDefinition);
				}

				// Add cell to collection and cells grid
				Cells[rowIndex][columnIndex] = value;
				Sheet.Grid.Children.Add(Cells[rowIndex][columnIndex]);
			}
		}

		/// <summary>
		/// Adds given cell to list
		/// </summary>
		/// <param name="cell">Cell to add to list</param>
		public void Add(Cell cell) {
			this[cell.PositionY, cell.PositionX] = cell;
		}
	}
}