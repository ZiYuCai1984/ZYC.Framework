using System.Windows.Controls;

namespace ZYC.Framework.Core;

public static class ListViewTools
{
    public static void SetListViewLastColumnWidth(ListView listView, double borderWidth = 8)
    {
        var listViewWidth = listView.ActualWidth;

        if (listView.View is not GridView gridView)
        {
            return;
        }

        var lastColumn = gridView.Columns.Last();

        var otherColumnsWidth = 0.0;
        foreach (var column in gridView.Columns)
        {
            if (column != lastColumn)
            {
                otherColumnsWidth += column.ActualWidth;
            }
        }

        gridView.Columns.Last().Width = Math.Max(0, listViewWidth - otherColumnsWidth - borderWidth);
    }
}