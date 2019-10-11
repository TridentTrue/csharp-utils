// Intended to be used along with EPPlus Excel package
public static class EPPExt
{
    public static ExcelWorksheet FormatTitleRow(this ExcelWorksheet ws)
    {
        using (var range = ws.Cells[1, 1, 1, ws.Dimension.End.Column])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
            range.Style.Font.Color.SetColor(Color.White);
        }

        return ws;
    }

    public static ExcelWorksheet FormatAlternatingColours(this ExcelWorksheet ws)
    {
        object previousValue = null;
        int alternatingCount = 0;
        for (int row = 1; row <= ws.Dimension.End.Row; row++)
        {
            object currentValue = ws.Cells[row, 1].Value;
            if (currentValue != previousValue)
                alternatingCount++;
            ExcelRow rowRange = ws.Row(row);
            ExcelFill RowFill = rowRange.Style.Fill;
            RowFill.PatternType = ExcelFillStyle.Solid;

            RowFill.BackgroundColor.SetColor(alternatingCount % 2 == 0 ? Color.White : Color.WhiteSmoke);

            previousValue = currentValue;
        }

        return ws;
    }
}