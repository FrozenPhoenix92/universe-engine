using Universe.Core.Common.Dto;
using Universe.Core.Common.Model;
using Universe.Core.Data;
using Universe.Core.Utils;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Universe.Core.Common.Services;

public class IpAddressConstraintService : IIpAddressConstraintService
{
	private readonly Regex _ipRegex = new(@"^(\d{1,3}\.){3}\d{1,3}$");
	private readonly IDbContext _context;


	public IpAddressConstraintService(IDbContext context) => _context = context;


	public async Task<bool> IsAllowedRequest(string ipAddress, string url, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNullOrEmpty(ipAddress, nameof(ipAddress));

		if (!_ipRegex.IsMatch(ipAddress))
			throw new ValidationException("Указанная строка не соотвествует формату адреса IP v4.");

		if (ipAddress.Split('.').Select(x => int.Parse(x)).Any(x => x > 255))
			throw new ValidationException("Значение адреса содержит сегмент, больший 255.");

		var result = true;

		foreach (var ipAddressConstraint in await _context.Set<IpAddressConstraint>()
			.Where(x => x.ContainingUrlPart == null || x.ContainingUrlPart == "" || EF.Functions.Like(url, "%" + x.ContainingUrlPart + "%"))
			.ToListAsync(ct))
		{
			var isIpAddressInRange = IsIpAddressInRange(
					ipAddress,
					ipAddressConstraint.AddressesRangeStart,
					ipAddressConstraint.AddressesRangeEnd ?? ipAddressConstraint.AddressesRangeStart);

			result = result && ipAddressConstraint.Rule == IpAddressConstraintRule.Allow
				? isIpAddressInRange
				: !isIpAddressInRange;
		}

		return result;
	}

	public Task Validate(IpAddressConstraintDto dto, IpAddressConstraint? entity = null, CancellationToken ct = default)
	{
		if (string.IsNullOrWhiteSpace(dto.AddressesRangeStart))
			throw new ValidationException("Стартовое значение диапазона адресов не должно быть пустым.");

		if (!_ipRegex.IsMatch(dto.AddressesRangeStart))
			throw new ValidationException("Стартовое значение диапазона не соответствует формату адреса IP v4.");

		if (dto.AddressesRangeStart.Split('.').Select(x => int.Parse(x)).Any(x => x > 255))
			throw new ValidationException("Стартовое значение содержит сегмент, больший 255.");

		if (!string.IsNullOrWhiteSpace(dto.AddressesRangeEnd))
		{
			if (!_ipRegex.IsMatch(dto.AddressesRangeEnd))
				throw new ValidationException("Конечное значение диапазона не соответствует формату адреса IP v4.");

			if (dto.AddressesRangeEnd.Split('.').Select(x => int.Parse(x)).Any(x => x > 255))
				throw new ValidationException("Конечное значение содержит сегмент, больший 255.");
		}

		return Task.CompletedTask;
	}


	private static long ConvertIpAddressToLong(string ip)
	{
		double num = 0;
		if (string.IsNullOrEmpty(ip)) return (long) num;

		var ipBytes = ip.Split('.');
		for (var i = ipBytes.Length - 1; i >= 0; i--)
		{
			num += int.Parse(ipBytes[i]) % 256 * Math.Pow(256, 3 - i);
		}
		return (long) num;
	}

	private static bool IsIpAddressInRange(string checkingIpAddress, string rangeStartIpAddress, string rangeEndIpAddress)
	{
		var rangeStartIpAddressNumber = ConvertIpAddressToLong(rangeStartIpAddress);
		var rangeEndIpAddressNumber = ConvertIpAddressToLong(rangeEndIpAddress);
		var checkingIpAddressNumber = ConvertIpAddressToLong(checkingIpAddress);

		return checkingIpAddressNumber >= Math.Min(rangeStartIpAddressNumber, rangeEndIpAddressNumber) &&
			checkingIpAddressNumber <= Math.Max(rangeStartIpAddressNumber, rangeEndIpAddressNumber);
	}
}
