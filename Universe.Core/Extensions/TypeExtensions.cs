namespace Universe.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsAssignableFromGenericType(this Type genericType, Type checkingType) =>
            checkingType.GetInterfaces()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericType);
    }
}
