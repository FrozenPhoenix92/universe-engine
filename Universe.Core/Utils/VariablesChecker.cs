namespace Universe.Core.Utils;

public static class VariablesChecker
{
	public static void CheckIsNotNull(object? value, string variableName)
	{
		if (value is null)
			throw new ArgumentNullException(variableName);
	}

	public static void CheckIsNotNullOrEmpty(string? value, string variableName)
	{
		if (string.IsNullOrEmpty(value))
			throw new ArgumentNullException(variableName);
	}

	public static void ThrowIfFalse(bool condition, Exception exception)
	{
		if (!condition)
		{
			throw exception;
		}
	}
}
