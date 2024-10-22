namespace Universe.Core.Common;

public static class InitializedPermissionsTracker
{
	private static IList<string> _initializedPermissions = new List<string>();


	public static IEnumerable<string> GetInitializedPermissions() => new List<string>(_initializedPermissions);

	internal static void AddPermission(string permission) => _initializedPermissions.Add(permission);
}
