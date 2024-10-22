using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Core.ModuleBinding.Bindings;

/// <summary>
/// ��������� �������� ������ � �������, ������ ����������� ���������� ������������.
/// </summary>
/// <remarks>
/// ���� �� �����������, ������������ �������� ��� ������ �������� � �������. � ������ ������ ���������� ������, ����������� ����� ���������,
/// ����� ����������� ����� ��������� ��� ��������� ������. ��� ���� � ���� ��������� ������� ��� ������� ������������� ������� ������� �,
/// ��� ���������, ������ �� ������������ ���, ��� ��������� ��������/��������� ������ �� ������� ������� ���������/����������� � �������.
/// </remarks>
public interface IDependenciesModuleBinding
{
    /// <summary>
    /// ��������� ����������� ������.
    /// </summary>
    void AddDependencies(IServiceCollection services, IConfiguration configuration);
}