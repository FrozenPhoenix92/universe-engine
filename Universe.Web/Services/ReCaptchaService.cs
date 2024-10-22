using Universe.Core.Exceptions;
using Universe.Core.Utils;
using Universe.Web.Configuration;

using Microsoft.Extensions.Options;

using System.Net;
using System.Text.Json;

namespace Universe.Web.Services;

public class ReCaptchaService : IReCaptchaService
{
	private const string ReCaptchaUrl = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";

	private readonly List<ClientApplicationSettings> _clientApplicationSettings;
	private readonly IHttpClientFactory _httpClientFactory;


	public ReCaptchaService(IHttpClientFactory httpClientFactory, IOptions<List<ClientApplicationSettings>> clientApplicationSettingsOptions)
	{
		VariablesChecker.CheckIsNotNull(httpClientFactory, nameof(httpClientFactory));
		VariablesChecker.CheckIsNotNull(clientApplicationSettingsOptions, nameof(clientApplicationSettingsOptions));
		VariablesChecker.CheckIsNotNull(clientApplicationSettingsOptions.Value, nameof(clientApplicationSettingsOptions.Value));

		_clientApplicationSettings = clientApplicationSettingsOptions.Value;
		_httpClientFactory = httpClientFactory;
	}


	public async Task<bool> ValidateResponse(string userResponse, string appCode, CancellationToken ct = default)
	{
		var clientApplicationSettingsItem = _clientApplicationSettings.SingleOrDefault(x => x.ApplicationCode == appCode);

		if (clientApplicationSettingsItem == null)
		{
			throw new BusinessException("Несуществующий код приложения.");
		}

		var httpRequestMessage = new HttpRequestMessage(
			HttpMethod.Get,
			string.Format(ReCaptchaUrl, clientApplicationSettingsItem.ReCaptchaSecret, userResponse));
		var httpClient = _httpClientFactory.CreateClient();

		try
		{
			var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, ct);

			if (httpResponseMessage.IsSuccessStatusCode)
			{
				using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync(ct);
				using (var readStream = new StreamReader(contentStream))
				{
					var jsonResponse = readStream.ReadToEnd();
					var data = JsonSerializer.Deserialize<ReCaptchaResponse>(
						jsonResponse,
						new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

					return data?.Success is true;
				}
			}

			return false;
		}
		catch (WebException)
		{
			return false;
		}
	}


	private class ReCaptchaResponse
	{
		public bool Success { get; set; }
	}
}
