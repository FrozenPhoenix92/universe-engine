using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Core.ModuleBinding.Bindings;

/// <summary>
/// ��������� �������� ������ � �������, ������ ����������� ���������� ����������� ���������� ������.
/// </summary>
/// <remarks>
/// ���� �� �����������, ������������ �������� ��� ������ �������� � �������. � ������ ������ ���������� ������, ����������� ����� ���������,
/// ����� ����������� ����� ��������� ��� ��������� ������. ��� ���� � ���� ��������� ������� ��� ������� ������������� ������� ������� �,
/// ��� ���������, ������ �� ������������ ���, ��� ��������� ��������/��������� ������ �� ������� ������� ���������/����������� � �������.
/// </remarks>
public interface IDataContextModuleBinding
{
    /// <summary>
    /// ��������� �������� ������ ������.
    /// </summary>
    void AddDataContext(IServiceCollection services, IConfiguration configuration);
}
