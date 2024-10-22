using Universe.Core.Infrastructure;
using Universe.Messaging.Dto;
using Universe.Messaging.Model;

namespace Universe.Messaging.Services;

public interface IEmailTemplateService : IDeleteDataOperation, IDeleteAllDataOperation, IValidateDataOperation<EmailTemplateDto, EmailTemplate>
{
	Task<EmailTemplate?> GetByCode(string code, CancellationToken ct = default);

	Task<IEnumerable<TemplateVariable>> GetGlobalVariables(CancellationToken ct = default);
}
