using Universe.Core.Data;
using Universe.Core.Exceptions;
using Universe.Core.Infrastructure;
using Universe.Core.Utils;
using Universe.Messaging.Dto;
using Universe.Messaging.Model;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

namespace Universe.Messaging.Services;

public class EmailTemplateService : IEmailTemplateService
{
	private readonly IDbContext _context;
	private readonly IDataService<EmailTemplate> _emailTemplateDataService;


	public EmailTemplateService(IDbContext context, IDataService<EmailTemplate> emailTemplateDataService)
	{
		VariablesChecker.CheckIsNotNull(context, nameof(context));
		VariablesChecker.CheckIsNotNull(emailTemplateDataService, nameof(emailTemplateDataService));

		_context = context;
		_emailTemplateDataService = emailTemplateDataService;
	}

	public async Task Delete(int id, CancellationToken ct = default) => await Delete(id, null, ct);

	public async Task Delete(int id, Func<EmailTemplate, IDbContext, CancellationToken, Task>? beforeSave, CancellationToken ct = default)
	{
		var entity = await _context.Set<EmailTemplate>().FirstOrDefaultAsync(x => x.Id != default && x.Id.Equals(id), ct);

		if (entity is not null && entity.System)
			throw new ForbiddenException("Шаблон, отмеченный, как используемый в системных операциях нельзя удалять.");

		await _emailTemplateDataService.Delete(id, beforeSave, ct);
	}
	public async Task DeleteAll(CancellationToken ct = default) => await DeleteAll(null, ct);

	public Task DeleteAll(Func<IQueryable<EmailTemplate>, IQueryable<EmailTemplate>>? queryHandler, CancellationToken ct = default) =>
		throw new ForbiddenException("Операция запрещена.");

	public async Task<EmailTemplate?> GetByCode(string code, CancellationToken ct = default)
	{
		VariablesChecker.CheckIsNotNullOrEmpty(code, nameof(code));

		return await _context.Set<EmailTemplate>().SingleOrDefaultAsync(x => x.Code == code, ct);
	}

	public async Task<IEnumerable<TemplateVariable>> GetGlobalVariables(CancellationToken ct = default) =>
		await _context.Set<TemplateVariable>().Where(x => x.Global).ToListAsync(ct);

	public async Task Validate(EmailTemplateDto dto, EmailTemplate? entity = null, CancellationToken ct = default)
	{
		if (string.IsNullOrWhiteSpace(dto.Code))
			throw new ValidationException("Значение поля 'Code' не допускает пустого значения.");

		if (await _context.Set<EmailTemplate>().AnyAsync(x => x.Code == dto.Code && x.Id != dto.Id, ct))
			throw new ValidationException($"Шаблон со значением поля 'Code', равным '{dto.Code}', уже существует.");

		if (entity is not null && entity.System ^ dto.System)
			throw new ValidationException("Изменение значения поля 'IsSystem' запрещено.");
	}
}
