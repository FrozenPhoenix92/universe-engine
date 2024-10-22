using System.Collections;
using System.Globalization;
using System.Reflection;

using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.Internal;

using Universe.Core.Data;
using Universe.Core.Extensions;
using Universe.Core.Infrastructure;
using Universe.Core.ModuleBinding;

namespace Universe.Core.Mapping;

public class DefaultBehaviourProvider
{
	public static void AddDefaultRules(IMapperConfigurationExpression mapperConfiguration)
	{
		var typeMapCopnfigurations = mapperConfiguration.Internal().Profiles.SelectMany(x => x.TypeMapConfigs);

		var existingMapperConfigurations = new List<TypeMapConfiguration>(typeMapCopnfigurations);

		existingMapperConfigurations.AddRange(typeMapCopnfigurations
			.Where(x => x.ReverseTypeMap is not null)
			.Select(x => x.ReverseTypeMap));

		var mapsHashSet = existingMapperConfigurations
			.Select(x => (x.SourceType, x.DestinationType))
			.ToHashSet();

		var projectAssemblies = AssembliesManager.GetProjectAssemblies();

		var dtoTypes = projectAssemblies
			.SelectMany(x => x.GetTypes()
				.Where(y => y.Name.EndsWith("DTO", true, CultureInfo.InvariantCulture) &&
					typeof(IDto<>).IsAssignableFromGenericType(y) &&
					y.IsClass &&
					!y.IsAbstract &&
					y.IsPublic));

		var entityTypes = projectAssemblies
			.SelectMany(x => x.GetTypes()
				.Where(y => typeof(IEntity<>).IsAssignableFromGenericType(y) &&
					y.IsClass &&
					!y.IsAbstract &&
					y.IsPublic));

		CreateDefaultMaps(mapperConfiguration, dtoTypes, entityTypes, mapsHashSet);
		ApplyDefaultComplexTypesIgnoring(mapperConfiguration);
	}


	private static void CreateDefaultMaps(IMapperConfigurationExpression cfg,
										  IEnumerable<Type> dtoTypes,
										  IEnumerable<Type> entityTypes,
										  HashSet<(Type, Type)> existingMaps)
	{
		foreach (var dtoType in dtoTypes)
		{
			var entityType = entityTypes.FirstOrDefault(x =>
				x.Name == dtoType.Name.Substring(0, dtoType.Name.Length - 3));

			if (entityType is null) continue;

			if (existingMaps.All(x => x.Item1 != entityType || x.Item2 != dtoType))
			{
				cfg.CreateMap(entityType, dtoType);
				existingMaps.Add((entityType, dtoType));
			}

			if (existingMaps.All(x => x.Item1 != dtoType || x.Item2 != entityType))
			{
				cfg.CreateMap(dtoType, entityType);
				existingMaps.Add((dtoType, entityType));
			}
		}
	}

	private static void ApplyDefaultComplexTypesIgnoring(IMapperConfigurationExpression mapperConfiguration)
	{
		mapperConfiguration.Internal().ForAllMaps((typeMap, expression) =>
		{
			if (expression is not MappingExpression mappingExpression) return;

			if (!typeof(IEntity<>).IsAssignableFromGenericType(mappingExpression.DestinationType)) return;

			mappingExpression.ForAllMembers(memberConfigurationExpression =>
			{
				var propertyInfo = memberConfigurationExpression.DestinationMember as PropertyInfo;

				if (propertyInfo is null || propertyInfo.PropertyType == typeof(string)) return;

				if (typeof(IEntity<>).IsAssignableFromGenericType(propertyInfo.PropertyType) ||
					typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
				{
					if (!Attribute.IsDefined(propertyInfo, typeof(DisableAutoIgnoreDefaultBehaviourAttribute)))
					{
						memberConfigurationExpression.Ignore();
					}
				}
			});
		});
	}
}
