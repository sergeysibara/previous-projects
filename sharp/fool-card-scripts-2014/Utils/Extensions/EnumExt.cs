using System;

public static class EnumExt
{
	/// <summary>
	/// Проверяет наличие текущего элемента в переданном перечне значений
	/// </summary>
	public static bool In(this Enum value, params Enum[] array)
	{
		int index = Array.IndexOf(array, value);
		return (index > -1);
	}
}