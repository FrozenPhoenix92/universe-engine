using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Universe.Core.ModuleBinding.Bindings;

/// <summary>
/// ��������� �������� ������ � �������, ������ ����������� ��������� ������������ ����������.
/// </summary>
/// <remarks>
/// ���� �� �����������, ������������ �������� ��� ������ �������� � �������. � ������ ������ ���������� ������, ����������� ����� ���������,
/// ����� ����������� ����� ��������� ��� ��������� ������. ��� ���� � ���� ��������� ������� ��� ������� ������������� ������� ������� �,
/// ��� ���������, ������ �� ������������ ���, ��� ��������� ��������/��������� ������ �� ������� ������� ���������/����������� � �������.
/// </remarks>
public interface IApplicationBuilderModuleBinding
{
	/// <summary>
	/// ����������� ������������ ���������� ��� ������.
	/// </summary>
	/// <param name="app">������ ��� ��������� ������������ ����������.</param>
	/// <param name="configuration">������, ���������� ������������ �������.</param>
	void ConfigureModule(IApplicationBuilder app, IConfiguration configuration);
}
