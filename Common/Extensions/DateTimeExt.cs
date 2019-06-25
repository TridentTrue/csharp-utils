public static class DateTimeExt
{
	public static string ToSortableString(this DateTime dateTime)
	{
		if (dateTime == DateTime.MinValue)
			return string.Empty;

		return $"<span style='display: none;'>{dateTime.ToString("s")}</span>{dateTime.ToShortDateString()}";
	}

	public static int GetFiscalYear(this DateTime dateTime)
	{
		return dateTime.Month >= 4 ? dateTime.Year : dateTime.Year - 1;
	}
}