using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GemBox.Spreadsheet;
using Metro_Tables.Code.Extensions;
using MetroTables.Code.Formula;
using MetroTables.Common.Interfaces;
using MetroTables.Extensions.FormulaContracts;

namespace MetroTables.Controls {
	public delegate void CellEventHandler(object sender);


	public class Cell : ContentControl, ICell {
		private bool isDefault = true;
		private bool isInEditMode = false;
		private bool isSelected = false;
		private int positionX = 0, positionY = 0;
		private Brush notSelectedBrush;
		private Border LeftBorder, TopBorder, RightBorder, BottomBorder, DiagonalDownBorder, DiagonalUpBorder;


		public Cell(int y, int x) {
			Label = CreateNewLabel();
			SetPosition(y, x);

			Content = Label;
		}


		private Label CreateNewLabel() {
			// TODO Pull default style
			Label label = new Label();

			label.Background = Brushes.White;

			label.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
			label.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

			label.Margin = new Thickness(1d);
			label.Padding = new Thickness(0);
			label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
			label.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

			return label;
		}

		// TODO comment
		public void SetValue(object value) {
			Label.Content = value;

			this.isDefault = false;

			if (OnCellSetValue != null) {
				OnCellSetValue(this);
			}
		}

		// TODO comment
		public object GetValue() {
			return Label.Content ?? String.Empty;
		}

		// TODO comment
		public void SetFormula(IFormulaExpression formula) {
			Formula = formula;

			this.isDefault = false;

			if (OnCellSetFormula != null) {
				OnCellSetFormula(this);
			}
		}

		public IFormulaExpression GetFormula() {
			return Formula;
		}

		IFormulaExpression ICell.GetFormula() {
			return GetFormula();
		}

		public void Select() {
			this.isSelected = true;

			this.notSelectedBrush = Label.Background;
			Label.Background = Brushes.LightBlue;

			if (OnCellSelected != null) {
				OnCellSelected(this);
			}
		}
		void ICell.Select() {
			Select();
		}

		public void Deselect() {
			this.isSelected = false;

			// TODO Apply style rather than changing color
			if (Label.Background == Brushes.LightBlue)
				Label.Background = notSelectedBrush;

			EndEditMode();

			if (OnCellDeselected != null) {
				OnCellDeselected(this);
			}
		}
		void ICell.Deselect() {
			Deselect();
		}

		private void SetPosition(int y, int x) {
			if (x < 0 || y < 0) {
				System.Diagnostics.Debug.WriteLine(String.Format("Position out of range [{0}, {1}]", y, x));
				return;
			}

			this.positionX = x;
			this.positionY = y;

			Grid.SetColumn(this, x);
			Grid.SetRow(this, y);
		}

		#region Import

		public void ImportData(ExcelCell excelCell) {
			// Check if style to import is default
			if (!excelCell.IsStyleDefault) {
				this.isDefault = false;
			}
			else {
				// Cell Value
				if (excelCell.Value == null) {
					SetValue(String.Empty);
				}
				else {
					SetValue(excelCell.Value);
				}

				Formula = FormulaManager.GetExpression(excelCell.Formula);
				IsFormulaHidden = excelCell.Style.FormulaHidden;

				// Style
				ImportStyle(excelCell);
			}
		}

		private void ImportStyle(ExcelCell excelCell) {
			//      Not implemented 
			//		    ShrinkToFit
			//		    WrapText
			CellStyle style = excelCell.Style;

			// Import font
			//      Not implemented
			//          ScriptPosition
			//          UnderlineStyle
			//          Strikeout
			ExcelFont font = style.Font;
			Label.FontFamily = new FontFamily(font.Name);
			Label.Foreground = new SolidColorBrush(font.Color.ConvertToNewColor());
			Label.FontStyle = font.Italic ? FontStyles.Italic : FontStyles.Normal;
			Label.FontWeight = FontWeight.FromOpenTypeWeight(font.Weight);
			Label.FontSize = font.Size / 20d;

			// Fill pattern
			ApplyFillPattern(style);

			// Alignment
			ApplyAlignment(style);

			// Indent
			Label.Padding = new Thickness(style.Indent, 0, style.Indent, 0);

			// Text Vertical
			Label.RenderTransform = new RotateTransform((style.IsTextVertical ? 90 : 0) + style.Rotation);

			// Locked
			if (style.Locked)
				IsReadOnly = true;

			// Number format
			NumberFormat = style.NumberFormat;

			// Borders
			// TODO Implement borders
			//ImportBorders(style.Borders);
		}

		private void ApplyFillPattern(CellStyle style) {
			ExcelFillPattern pattern = style.FillPattern;
			Brush patternBrush = new SolidColorBrush(Colors.White);
			switch (pattern.PatternStyle) {
				case FillPatternStyle.Solid:
					patternBrush = new SolidColorBrush(pattern.PatternForegroundColor.ConvertToNewColor());
					break;
				case FillPatternStyle.DiagonalCrosshatch:
				case FillPatternStyle.DiagonalStripe:
				case FillPatternStyle.Gray12:
				case FillPatternStyle.Gray25:
				case FillPatternStyle.Gray50:
				case FillPatternStyle.Gray6:
				case FillPatternStyle.Gray75:
				case FillPatternStyle.HorizontalStripe:
				case FillPatternStyle.ReverseDiagonalStripe:
				case FillPatternStyle.ThickDiagonalCrosshatch:
				case FillPatternStyle.ThinDiagonalCrosshatch:
				case FillPatternStyle.ThinDiagonalStripe:
				case FillPatternStyle.ThinHorizontalCrosshatch:
				case FillPatternStyle.ThinHorizontalStripe:
				case FillPatternStyle.ThinReverseDiagonalStripe:
				case FillPatternStyle.ThinVerticalStripe:
				case FillPatternStyle.VerticalStripe:
				case FillPatternStyle.None:
				default:
					patternBrush = new SolidColorBrush(Colors.Transparent);
					break;
			}
			Label.Background = patternBrush;
		}

		private void ApplyAlignment(CellStyle style) {
			switch (style.HorizontalAlignment) {
				case HorizontalAlignmentStyle.Justify:
				case HorizontalAlignmentStyle.CenterAcross:
				case HorizontalAlignmentStyle.Center:
					Label.HorizontalContentAlignment = HorizontalAlignment.Center;
					break;
				case HorizontalAlignmentStyle.Distributed:
				case HorizontalAlignmentStyle.Fill:
					Label.HorizontalContentAlignment = HorizontalAlignment.Stretch;
					break;
				default:
				case HorizontalAlignmentStyle.General:
				case HorizontalAlignmentStyle.Left:
					Label.HorizontalContentAlignment = HorizontalAlignment.Left;
					break;
				case HorizontalAlignmentStyle.Right:
					Label.HorizontalContentAlignment = HorizontalAlignment.Right;
					break;
			}

			switch (style.VerticalAlignment) {
				case VerticalAlignmentStyle.Bottom:
					Label.VerticalContentAlignment = VerticalAlignment.Bottom;
					break;
				default:
				case VerticalAlignmentStyle.Justify:
				case VerticalAlignmentStyle.Center:
					Label.VerticalContentAlignment = VerticalAlignment.Center;
					break;
				case VerticalAlignmentStyle.Distributed:
					Label.VerticalContentAlignment = VerticalAlignment.Stretch;
					break;
				case VerticalAlignmentStyle.Top:
					Label.VerticalContentAlignment = VerticalAlignment.Top;
					break;
			}
		}

		private void ImportBorders(CellBorders cellBorders) {
			Brush brush = ConvertToBrush(cellBorders[IndividualBorder.Left]);
			Thickness thickness = ConvertToThickness(cellBorders[IndividualBorder.Left]);

			LeftBorder = new Border();

			LeftBorder.BorderBrush = brush;
			LeftBorder.BorderThickness = thickness;
			LeftBorder.Width = thickness.Left;

			LeftBorder.HorizontalAlignment = HorizontalAlignment.Left;
			LeftBorder.VerticalAlignment = VerticalAlignment.Stretch;

			Content = LeftBorder;
			LeftBorder.Child = Label;
		}

		private static Thickness ConvertToThickness(CellBorder cellBorder) {
			switch (cellBorder.LineStyle) {
				case LineStyle.Hair:
					return new Thickness(0.1d);
				case LineStyle.Medium:
				case LineStyle.MediumDashDot:
				case LineStyle.MediumDashDotDot:
				case LineStyle.MediumDashed:
					return new Thickness(1.25d);
				case LineStyle.None:
					return new Thickness(0);
				case LineStyle.Thick:
					return new Thickness(3);
				case LineStyle.SlantDashDot:
				case LineStyle.DashDot:
				case LineStyle.DashDotDot:
				case LineStyle.Dashed:
				case LineStyle.Dotted:
				case LineStyle.Double:
				case LineStyle.Thin:
				default:
					return new Thickness(0.4d);
			}
		}

		private static Brush ConvertToBrush(CellBorder cellBorder) {
			switch (cellBorder.LineStyle) {
				case LineStyle.SlantDashDot:
				case LineStyle.MediumDashDot:
				case LineStyle.DashDot:

				case LineStyle.MediumDashDotDot:
				case LineStyle.DashDotDot:

				case LineStyle.MediumDashed:
				case LineStyle.Dashed:

				case LineStyle.Dotted:

				case LineStyle.Double:

				case LineStyle.None:
					return new SolidColorBrush(Colors.Transparent);
				case LineStyle.Hair:
				case LineStyle.Medium:
				case LineStyle.Thick:
				case LineStyle.Thin:
				default:
					return new SolidColorBrush(cellBorder.LineColor.ConvertToNewColor());
			}
		}
		#endregion

        #region Edit mode methods

        public void ActivateEditMode() {
			if (this.isInEditMode || !this.isSelected) return;

			this.isInEditMode = true;
			Content = CreateEditTextBox();

			if (OnCellEditModeActivate != null) {
				OnCellEditModeActivate(this);
			}
		}

		private TextBox CreateEditTextBox() {
			TextBox textBox = new TextBox() {
				Text = GetValue().ToString(),

				HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
				VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
				HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left,
				VerticalContentAlignment = System.Windows.VerticalAlignment.Center
			};
			textBox.PreviewKeyDown += (s, e) => {
				if (e.Key == System.Windows.Input.Key.Enter) {
					SetValue(textBox.Text);
					EndEditMode();
				}
				else if (e.Key == System.Windows.Input.Key.Escape) {
					CancelEditMode();
				}

				if (OnCellEdited != null) {
					OnCellEdited(textBox);
				}
			};

			return textBox;
		}

		public void CancelEditMode() {
			if (OnCellEditModeCancel != null) {
				OnCellEditModeCancel(this);
			}

			EndEditMode();
		}

		public void EndEditMode() {
			if (!this.isInEditMode) return;

			UpdateFormula();

			this.isInEditMode = false;
			Content = Label;

			if (OnCellEditModeEnd != null) {
				OnCellEditModeEnd(this);
			}
		}

		public void UpdateFormula() {
			if (Label.Content is string) {
				if (FormulaManager.IsFormula(GetValue().ToString())) {
					SetFormula(FormulaManager.GetExpression(GetValue().ToString()));
					SetValue(FormulaManager.Evaluate(Formula));

					if (OnCellFormulaUpdated != null) {
						OnCellFormulaUpdated(this);
					}
				}
			}
		}

		void ICell.ActivateEditMode() { ActivateEditMode(); }
		void ICell.CancelEditMode() { CancelEditMode(); }
		void ICell.EndEditMode() { EndEditMode(); }

        #endregion

        #region Properties

        bool ICell.IsDefault
        {
            get { return this.isDefault; }
        }

        bool ICell.IsInEditMode
        {
            get { return this.isInEditMode; }
        }

        // TODO To Explict
        public int PositionX
        {
            get { return this.positionX; }
            set { SetPosition(this.positionY, value); }
        }

        // TODO To Explict
        public int PositionY
        {
            get { return this.positionY; }
            set { SetPosition(value, this.positionX); }
        }

        // TODO remove
        public Boolean IsSelected
        {
            get { return this.isSelected; }
        }
        bool ICell.IsSelected
        {
            get { return IsSelected; }
        }

        // TODO To Explict
        public Boolean IsFormulaHidden { get; set; }
        // TODO To Explict
        public IFormulaExpression Formula { get; set; }

        // TODO To Explict
        public Boolean IsReadOnly { get; set; }
        // TODO To Explict
        public String NumberFormat { get; set; }


        public Label Label { get; private set; }
        Label ICell.Label
        {
            get { return Label; }
        }

		public event CellEventHandler OnCellSelected;
		public event CellEventHandler OnCellDeselected;

		public event CellEventHandler OnCellEditModeActivate;
		public event CellEventHandler OnCellEdited;
		public event CellEventHandler OnCellEditModeEnd;
		public event CellEventHandler OnCellEditModeCancel;

		public event CellEventHandler OnCellSetValue;
		public event CellEventHandler OnCellSetFormula;
		public event CellEventHandler OnCellFormulaUpdated;

		public event CellEventHandler OnCellStyleChanged;

        #endregion
	}
}
