using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Metro_Tables.Code.Enums;

namespace Metro_Tables.Code.Extensions {
	public static class GridExtensions {
		//public static Collection<TElement> GetElements<TElement>(this Grid grid, Int32 row, Int32 column)
		//	where TElement : UIElement {
		//	Contract.Requires(grid.Children != null);


		//	var elements = from UIElement element in grid.Children
		//				   where element is TElement &&
		//						 Grid.GetRow(element) == row &&
		//						 Grid.GetColumn(element) == column
		//				   select element as TElement;
		//	return new Collection<TElement>(elements.ToList());
		//}
		//public static Collection<TElement> GetElements<TElement>(this Grid grid, HeaderOrientation orientation, Int32 index)
		//	where TElement : UIElement {
		//	Contract.Requires(grid.Children != null);


		//	var elements = from UIElement element in grid.Children
		//				   where element is TElement &&
		//							orientation == HeaderOrientation.Vertical ?
		//								Grid.GetColumn(element) == index :
		//								Grid.GetRow(element) == index
		//				   select element as TElement;
		//	return new Collection<TElement>(elements.ToList());
		//}
	}
}
