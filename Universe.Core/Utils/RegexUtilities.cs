using System.Text.RegularExpressions;

namespace Universe.Core.Utils;

public class RegexUtilities
{
	public static bool IsValidEmail(string value) => new Regex("^.+@.+\\..+$").IsMatch(value);
}
