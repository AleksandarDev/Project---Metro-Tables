using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GemBox.Spreadsheet;

using MetroTables.Controls;
using Metro_Tables.Code.Extensions;
using MetroTables.Common.Interfaces;
using MetroTables.Code.Converters;
using MetroTables.Extensions.FormulaContracts;

namespace MetroTables.Controls {
	public class SheetSelectionEventArgs : EventArgs {
		public bool IsSelectedOneCell { get; set; }
		public int SelectionStartX { get; set; }
		public int SelectionStartY { get; set; }
		public int SelectionEndX { get; set; }
		public int SelectionEndY { get; set; }

		public double FontSize { get; set; }
		public string FontFamily { get; set; }

		public IFormulaExpression Formula { get; set; }

		public int SelectionWidth {
			get { return SelectionEndX - SelectionStartX; }
		}
		public int SelectionHeight {
			get { return SelectionEndY - SelectionStartY; }
		}


		public SheetSelectionEventArgs(ICell cell) {
			IsSelectedOneCell = true;

			SelectionStartX = SelectionEndX = cell.PositionX;
			SelectionStartY = SelectionEndY = cell.PositionY;
		}


		public SheetSelectionEventArgs(int startX, int startY, int endX, int endY) {
			IsSelectedOneCell = false;

			SelectionStartX = startX;
			SelectionEndX = endX;
			SelectionStartY = startY;
			SelectionEndY = endY;
		}
	}
	public delegate void SheetSelectionEventHandler(object sender, SheetSelectionEventArgs args);

	public partial class SheetControl : UserControl, ISheetContract {
		#region Variables
		// Constants
		private const Double DefaultCellWidth = 60d;
		private const Double DefaultCellHeight = 27d;
		private const Double GuidelineThickness = 0.4d;
		private const Double MinScrollBarViewportSize = 5.0d;
		private static readonly Brush HeadersBackgroundColor = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));

		// Document to work with
		private ExcelWorksheet worksheet;

		// Scroll variables
		private Int32 positionRow = 0;
		private Int32 positionColumn = 0;
		private Int32 numRowsInView = 0;
		private Int32 numColumnsInView = 0;

		// Grid variables
		private CellsCollection cells;

		// Cell edit variable
		private ICell editedCell;

		// Selection events
		public event SheetSelectionEventHandler SelectionStarted;
		public event SheetSelectionEventHandler SelectionChanged;
		public event SheetSelectionEventHandler SelectionEnded;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates new object of type SheetControl and imports data from ExcelWorksheet if available
		/// </summary>
		/// <param name="worksheet">Optional. Worksheet to import</param>
		public SheetControl(ExcelWorksheet worksheet = null) {
			InitializeComponent();

			// Assign argument
			this.worksheet = worksheet;

			ReloadSheet();
		}

		#endregion

		private void UserControlLoaded(object sender, RoutedEventArgs e) {
			
		}

		#region Loading

		/// <summary>
		/// Reloads whole sheet.
		/// Clears all and loads with new data, this is used on
		/// sheet initial loading and on any windows resize and such
		/// </summary>
		private void ReloadSheet() {
			// Loads all other components
			LoadSheetGrid();
			LoadGuidelines();
			LoadHeaders();
			LoadScrollNavigation();

			// Adds first cell to initialize grids variables
			this.cells.Add(new Cell(0, 0));

			// Import worksheet if exist
			if (this.worksheet != null) {
				ImportWorksheet();
			}
		}

		/// <summary>
		/// Loads all controls related to scroll
		/// </summary>
		private void LoadScrollNavigation() {
			// TODO Change viewport size and scrollbar maximul on cell added

			// Configure horizontal scroll bar
			ScrollBarHorizontal.Maximum = 0.001d;
			//ScrollBarHorizontal.ViewportSize = Math.Max(MinScrollBarViewportSize, ScrollBarHorizontal.Maximum / (double)this.numColumnsInView);
			ScrollBarHorizontal.Scroll += ScrollBarHorizontal_Scroll;

			// Configure vertical scroll bar
			ScrollBarVertical.Maximum = 0.001d;
			//ScrollBarVertical.ViewportSize = Math.Max(MinScrollBarViewportSize, ScrollBarVertical.Maximum / (double)this.numRowsInView);
			ScrollBarVertical.Scroll += ScrollBarVertical_Scroll;
		}

		/// <summary>
		/// Imports all cells from opened document
		/// </summary>
		private void ImportWorksheet() {
			// Gets cell range from loaded sheet
			CellRange usedCells = worksheet.GetUsedCellRange(true);

			// Is there are any used cells
			if (usedCells != null && usedCells.Count() > 0) {
				// Go throught all of them
				for (int rowIndex = 0; rowIndex < usedCells.Height; rowIndex++) {
					for (int columnIndex = 0; columnIndex < usedCells.Width; columnIndex++) {
						// Create new cell for each of loaded used cells,
						// set cells position according to current index and
						// import call data
						Cell cell = new Cell(usedCells.FirstRowIndex + rowIndex, usedCells.FirstColumnIndex + columnIndex);
						cell.ImportData(usedCells[rowIndex, columnIndex]);
						//SheetGrid.Children.Add(cell);

						// TODO import grid definitions


						// Add cell to list
						cells.Add(cell);
					}
				}
			}
		}

		/// <summary>
		/// Creates headers on left and top borders of control
		/// </summary>
		private void LoadHeaders() {
			// Sets background color for corner label (top-left label)
			// TODO: Add functionality to corner label
			CornerLabel.Background = HeadersBackgroundColor;

			// Clears vertical headers stack list and fills it with new headers
			VerticalHeaders.Children.Clear();
			for (int rowIndex = 0; rowIndex < numRowsInView; rowIndex++) {
				// Creates new label for vertical header
				// with content equal to row index
				Label label = CreateNewHeaderLabel(
					rowIndex + 1,
					DefaultCellHeight, 
					DefaultCellWidth);

				// Adds label to stack list
				VerticalHeaders.Children.Add(label);
			}

			// Clears horizontal headers stack list and fills it with new headers
			// Starts with 1 because of content method isnt zero-based (1=A, 2=B, ...)
			HorizontalHeaders.Children.Clear();
			for (int columnIndex = 1; columnIndex <= numColumnsInView; columnIndex++) {
				// Creates new label for horizontal header
				// with content that represents current column
				Label label = CreateNewHeaderLabel(
					HeadersHelper.IndexToLetters(columnIndex),
					DefaultCellHeight,
					DefaultCellWidth);

				// Adds label to stack list
				HorizontalHeaders.Children.Add(label);
			}
		}

		/// <summary>
		/// Creates new Header label.
		/// Horizontal and Vertical content alignment are set to center,
		/// padding to zero and background color to headers default background color
		/// </summary>
		/// <param name="content">Content of header</param>
		/// <param name="height">Header height</param>
		/// <param name="width">Header width</param>
		/// <returns>New label with given values</returns>
		private Label CreateNewHeaderLabel(object content, double height, double width) {
			Label label = new Label();

			label.Content = content;
			label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
			label.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

			label.Padding = new Thickness(0);
			label.Height = height;
			label.Width = width;

			label.Background = HeadersBackgroundColor;

			return label;
		}

		/// <summary>
		/// Loads all needed cells
		/// </summary>
		private void LoadSheetGrid() {
			// Clears cells from grid and creates new list of cells
			SheetGrid.Children.Clear();
			this.cells = new CellsCollection(this);

			// This prevents error when SheetGrid.Actual(Width & Height) is zero 
			this.numRowsInView = this.numColumnsInView = 0;

			// Check if user can see any cells
			if (SheetGrid.ActualWidth != 0 &&
				SheetGrid.ActualHeight != 0) {
				// Calculate number of visible cells
				this.numRowsInView = (int)Math.Ceiling(SheetGrid.ActualHeight / DefaultCellHeight);
				this.numColumnsInView = (int)Math.Ceiling(SheetGrid.ActualWidth / DefaultCellWidth);
			}
		}

		/// <summary>
		/// Creates guidelines for current view
		/// </summary>
		private void LoadGuidelines() {
			// NOTE Need to remove guidelines to show borders?
			// TODO Calculate where guideline needs to be removed to make good Borders for cells

			// Default guidelines color
			Brush lineColor = Brushes.Gray;

			// Clear already existing guidelines
			SheetGridGuidelines.Children.Clear();

			// Build list of row guidelines
			for (int rowIndex = 0; rowIndex < numRowsInView + 1; rowIndex++) {
				Line line = GetNewGuideline(lineColor, Orientation.Horizontal, rowIndex * DefaultCellHeight);

				SheetGridGuidelines.Children.Add(line);
			}

			// Build list of column guidelines
			for (int columnIndex = 0; columnIndex < numColumnsInView + 1; columnIndex++) {
				Line line = GetNewGuideline(lineColor, Orientation.Vertical, columnIndex * DefaultCellWidth);

				SheetGridGuidelines.Children.Add(line);
			}
		}

		/// <summary>
		/// Creates new Line that represents guideline
		/// </summary>
		/// <param name="lineColor">Color of line</param>
		/// <param name="orientation">Orientation of line (horizontal or vertical)</param>
		/// <param name="offset">Offset from (0,0) coordinates</param>
		/// <returns></returns>
		private Line GetNewGuideline(Brush lineColor, Orientation orientation, double offset) {
			Line line = new Line();

			line.Stroke = lineColor;
			line.StrokeThickness = GuidelineThickness;

			if (orientation == Orientation.Vertical) {
				line.X1 = line.X2 = offset;
				line.Y1 = 0;
				line.Y2 = SheetGridGuidelines.ActualHeight;
			}
			else {
				line.Y1 = line.Y2 = offset;
				line.X1 = 0;
				line.X2 = SheetGridGuidelines.ActualWidth;
			}
			return line;
		}

		#endregion


		private void EnterEditMode() {
			this.isSelectionActive = false;
			IEnumerable<Cell> selectedCell = GetSelectedCells();

			if (selectedCell == null ||
				selectedCell.Count() != 1) {
					return;
			}

			ICell cell = selectedCell.First();

			this.editedCell = cell;
			this.editedCell.ActivateEditMode();
		}

		#region Selections

		Boolean isSelectionActive = false;
		Int32 selectionX1 = 0;
		Int32 selectionY1 = 0;
		Int32 selectionX2 = 0;
		Int32 selectionY2 = 0;

		private void HeadersGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (e.LeftButton == MouseButtonState.Pressed) {
				int row = 0, column = 0;
				bool valid = GetHeaderCoordinatesFromPosition(e.GetPosition(HeadersGrid), out row, out column);

				if (valid) {
					// TODO Select header
					System.Diagnostics.Debug.WriteLine("Header selected [" + row + ", " + column + "]", "Info");
				}
			}
		}

		private void SheetGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (e.LeftButton == MouseButtonState.Pressed &&
				e.RightButton == MouseButtonState.Released) {
				int row = 0, column = 0;
				bool valid = GetCoordinatesFromPosition(e.GetPosition(SheetGrid), out row, out column);

				if (valid) {
					if (this.editedCell == null ||
						!this.editedCell.IsInEditMode) {
						DeselectCells();
						SelectionStart(row, column);
						HighlightSelection();

						if (e.ClickCount == 2) {
							EnterEditMode();
						}

						e.Handled = true;
					}
					else {
						if (row != this.editedCell.PositionY ||
							column != this.editedCell.PositionX) {
								this.editedCell.EndEditMode();
								this.editedCell = null;
						}
					}
					// TODO Select cell
					// TODO Highlight header
					// TODO Borders for selection
					System.Diagnostics.Debug.WriteLine("Cell selected [" + row + ", " + column + "]", "Info");
				}
			}
			else if (e.RightButton == MouseButtonState.Pressed) {
				//DeselectCells();

				// TODO show context menu
			}
		}

		private void DeselectCells() {
			isSelectionActive = false;
 
			// TODO Check edit mode
			foreach (List<Cell> listCells in cells.Cells) {
				foreach (Cell cell in listCells.Where((p, e) => p != null && p.IsSelected)) {
					cell.Deselect();
				}
			}
		}

		private void SelectionStart(int row, int column) {
			isSelectionActive = true;

			selectionX1 = selectionX2 = column;
			selectionY1 = selectionY2 = row;

			if (SelectionStarted != null) {
				SelectionStarted(this, new SheetSelectionEventArgs(this.cells[row, column]));
			}
		}

		int prevMouseMoveX = 0, prevMouseMoveY = 0;
		private void SheetGrid_PreviewMouseMove(object sender, MouseEventArgs e) {
			if (isSelectionActive && 
				e.LeftButton == MouseButtonState.Pressed &&
				e.RightButton == MouseButtonState.Released) {
				int row = 0, column = 0;
				bool valid = GetCoordinatesFromPosition(e.GetPosition(SheetGrid), out row, out column);

				if (valid) {
					// Highlight refresh only if selection shanged
					if (row != prevMouseMoveY || column != prevMouseMoveX) {
						selectionX2 = column;
						selectionY2 = row;

						DeselectCells();
						isSelectionActive = true;
						HighlightSelection();

						if (SelectionChanged != null) {
							SelectionChanged(this, new SheetSelectionEventArgs(selectionX1, selectionY1, selectionX2, selectionY2));
						}
					}

					prevMouseMoveX = column;
					prevMouseMoveY = row;
				}
			}
		}

		private void SheetGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e) {
			// TODO: Fill-in font family
			// TODO: Fill-in font size
			// TODO: Fill-in formula bar
			double fontSize = -1;
			bool sameFontSize = true;

			string fontFamily = null;
			bool sameFontFamily = true;

			IFormulaExpression formula = null;
			bool sameFormula = true;

			foreach (ICell cell in GetSelectedCells()) {
				if (formula != null && formula == cell.GetFormula()) {
					sameFormula = false;
				}
				else formula = cell.GetFormula();

				if (fontSize != -1 && fontSize != cell.Label.FontSize) {
					sameFontSize = false;
				}
				else fontSize = cell.Label.FontSize;

				if (fontFamily != null && fontFamily != cell.Label.FontFamily.ToString()) {
					sameFontFamily = false;
				}
				else fontFamily = cell.Label.FontFamily.ToString();
			}

			FormulaControl.FormulaEditor.Clear();
			if (formula != null && sameFormula)
				FormulaControl.FormulaEditor.AppendText(formula.Source);

			if (SelectionEnded != null) {
				SelectionEnded(this, new SheetSelectionEventArgs(selectionX1, selectionY1, selectionX2, selectionY2) {
					FontSize = (sameFontSize ? fontSize : -1),
					FontFamily = (sameFontFamily ? fontFamily : String.Empty),
					Formula = (sameFormula ? formula : null)
				});
			}
		}

		private void HighlightSelection() {
			ApplyToSelection(cell => cell.Select());
		}

		private Boolean GetHeaderCoordinatesFromPosition(Point position, out Int32 coordinatesY, out Int32 coordinatesX) {
			coordinatesX = this.positionColumn;
			coordinatesY = this.positionRow;

			// Check if in range
			if (position.X < 0 || 
				position.Y < 0 ||
				position.X > HeadersGrid.ActualWidth ||
				position.Y > HeadersGrid.ActualHeight)
				return false;

			if (position.X > DefaultCellWidth) {
				if (position.Y > DefaultCellHeight)
					return false;

				return WidthApprox(ref position, ref coordinatesX);
			}
			else if (position.Y > DefaultCellHeight) {
				if (position.X > DefaultCellWidth)
					return false;

				return HeightApprox(ref position, ref coordinatesY);
			}
			else if (position.Y <= DefaultCellHeight &&
					 position.X <= DefaultCellWidth) {
				return true;
			}
			else return false;

			return true;
		}

		private Boolean GetCoordinatesFromPosition(Point position, out Int32 coordinatesY, out Int32 coordinatesX) {
			coordinatesY = this.positionRow;
			coordinatesX = this.positionColumn;

			// Check if in range
			if (position.X < 0 ||
				position.Y < 0 ||
				position.X > SheetGrid.ActualWidth ||
				position.Y > SheetGrid.ActualHeight)
				return false;

			HeightApprox(ref position, ref coordinatesY);
			WidthApprox(ref position, ref coordinatesX);

			return true;
		}

		private bool HeightApprox(ref Point position, ref Int32 coordinatesY) {
			double sumHeight = ScrollView.VerticalOffset;
			for (int index = 0; index < numRowsInView; index++) {
				double sumHeightNext = sumHeight + GetDefinition(Orientation.Vertical , positionRow + index);
				if (sumHeightNext >= position.Y) {
					return true;
				}
				else {
					sumHeight = sumHeightNext;
					coordinatesY++;
				}
			}
			return false;
		}

		private bool WidthApprox(ref Point position, ref Int32 coordinatesX) {
			double sumWidth = ScrollView.HorizontalOffset;
			for (int index = 0; index < numColumnsInView; index++) {
				double sumWidthNext = sumWidth + GetDefinition(Orientation.Horizontal , positionColumn + index);
				if (sumWidthNext >= position.X) {
					return true;
				}
				else {
					sumWidth = sumWidthNext;
					coordinatesX++;
				}
			}
			return false;
		}

		public double GetDefinition(Orientation orientation, int index) {
			if (orientation == Orientation.Horizontal) {
				if (index < 0 || index > SheetGrid.ColumnDefinitions.Count() - 1)
					return DefaultCellWidth;
				else return SheetGrid.ColumnDefinitions[index].Width.Value;
			}
			else {
				if (index < 0 || index > SheetGrid.RowDefinitions.Count() - 1)
					return DefaultCellHeight;
				else return SheetGrid.RowDefinitions[index].Height.Value;
			}
		}

		void ISheetContract.ApplyToSelection(Action<ICell> action) {
			ApplyToSelection(action);
		}

		public void ApplyToSelection(Action<Cell> action) {
			foreach (Cell cell in GetSelectedCells()) {
				action(cell);
			}
		}

		public IEnumerable<Cell> GetSelectedCells() {
			int X1 = Math.Min(selectionX2, selectionX1);
			int X2 = Math.Max(selectionX2, selectionX1);
			int Y1 = Math.Min(selectionY2, selectionY1);
			int Y2 = Math.Max(selectionY2, selectionY1);

			for (int indexY = Y1; indexY <= Y2; indexY++) {
				for (int indexX = X1; indexX <= X2; indexX++) {
					yield return cells[indexY, indexX];
				}
			}
		}

		#endregion

		#region Scroll

		private void ScrollBarVertical_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) {
			if (alreadyHandled) {
				alreadyHandled = false;
				e.Handled = true;
				return;
			}

			
			ScrollVertical((int)(e.NewValue - positionRow));
			e.Handled = true;
		}

		bool alreadyHandled = false;

		/// <summary>
		/// Scrolls ScrollView for given delta of cells
		/// </summary>
		/// <param name="delta">Number of cells to move</param>
		private void ScrollVertical(int delta) {
			int newPosition = Math.Max(this.positionRow + delta, 0);

			// Calculates new offset by adding newly calculated offset from range and current overtical offset
			double offsetDelta = CalculateDefinitionRange(Orientation.Vertical, this.positionRow, newPosition);
			double newOffset = Math.Max(0, ScrollView.VerticalOffset + (delta < 0 ? -offsetDelta : offsetDelta));

			// Set current vertical cell
			this.positionRow = newPosition;

			// Move ScrollView to offset
			if (ScrollView.VerticalOffset != newOffset) {
				alreadyHandled = true;
			}
			ScrollView.ScrollToVerticalOffset(newOffset);
			
			// Sets scroll bar position and size
			ScrollBarVertical.Value = this.positionRow;
			ScrollBarVertical.ViewportSize = Math.Max(MinScrollBarViewportSize, ScrollBarVertical.Maximum / (double)this.numRowsInView);
			ScrollBarVertical.Maximum = Math.Max(this.positionRow, ScrollBarVertical.Maximum);

			// Fill vertical headers
			int currentIndex = positionRow + 1;
			foreach (var header in VerticalHeaders.Children) {
				(header as Label).Content = (currentIndex++).ToString();
			}
		}

		/// <summary>
		/// Calculates sum of grid definition size from given range
		/// </summary>
		/// <param name="orientation">Orientation of definitions</param>
		/// <param name="start">Inclusive range start index</param>
		/// <param name="length">Exclusive end range index</param>
		/// <returns>Returns sum of definition size from given range</returns>
		private double CalculateDefinitionRange(Orientation orientation, int a, int b) {
			double result = 0;

			int startIndex = Math.Min(b, a);
			int endIndex = Math.Max(b, a);

			// Adds each definition size to variable
			for (int index = startIndex; index < endIndex; index++) {
				result += GetDefinition(orientation, index);
			}

			return result;
		}

		private void ScrollBarHorizontal_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) {
			// TODO Implement
		}

		private void ScrollView_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e) {
			if (IsInitialized) {
				if (alreadyHandled) {
					alreadyHandled = false;
					e.Handled = true;
					return;
				}

				ScrollVertical((int)(e.VerticalChange / DefaultCellHeight));
				e.Handled = true;
			}
			else e.Handled = false;
		}

		private void ScrollView_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			int rowDelta = (int)(-e.Delta / DefaultCellHeight) / 2;
			for (int index = 0; index < rowDelta; index++) {
				cells.Add(new Cell(this.positionRow + this.numRowsInView + index, 0));
			}
			ScrollVertical(rowDelta);
		}

		#endregion

		private void HeadersGrid_GotFocus(object sender, RoutedEventArgs e) {
			Metro_Tables.App.Close();
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) {
			ReloadSheet();
		}

		Grid ISheetContract.Grid {
			get { return SheetGrid; }
		}

		double ISheetContract.DefaultCellHeight {
			get { return DefaultCellHeight; }
		}

		double ISheetContract.DefaultCellWidth {
			get { return DefaultCellWidth; }
		}
	}
}
