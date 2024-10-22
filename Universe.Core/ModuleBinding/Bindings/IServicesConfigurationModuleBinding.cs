using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Core.ModuleBinding.Bindings;

/// <summary>
/// ��������� �������� ������ � �������, ������ ����������� ��������� ������������ ��������.
/// </summary>
/// <remarks>
/// ���� �� �����������, ������������ �������� ��� ������ �������� � �������. � ������ ������ ���������� ������, ����������� ����� ���������,
/// ����� ����������� ����� ��������� ��� ��������� ������. ��� ���� � ���� ��������� ������� ��� ������� ������������� ������� ������� �,
/// ��� ���������, ������ �� ������������ ���, ��� ��������� ��������/��������� ������ �� ������� ������� ���������/����������� � �������.
/// </remarks>
public interface IServicesConfigurationModuleBinding
{
    /// <summary>
    /// ���������� ������������ ��������.
    /// </summary>
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}
