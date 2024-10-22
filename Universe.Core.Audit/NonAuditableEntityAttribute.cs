namespace Universe.Core.Audit;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class NonAuditableEntityAttribute : Attribute
{
}
