﻿using Universe.Web.Dto.Client;
using Universe.Web.Model;

namespace Universe.Web.Services;

public interface IClientAccountService
{
	Task ChangePassword(ClientChangePasswordRequest request, CancellationToken ct = default);

	Task<ClientSignInResult> ConfirmEmail(ClientConfirmEmailRequest request, CancellationToken ct = default);

	Task ForgotPassword(ClientForgotPasswordRequest request, CancellationToken ct = default);

	Task<string> GetClientAuthToken(Guid clientId, CancellationToken ct = default);

	string GetClientAuthToken(Client client);

	Task<ClientSessionData> GetClientSessionData(Guid clientId, CancellationToken ct = default);

	Task<ClientSignInResult> ResetPassword(ClientResetPasswordRequest request, CancellationToken ct = default);

	string SetAutoGeneratedPasswordForClient(Client client);

	Task<ClientSignInResult> SignIn(ClientSignInRequest request, CancellationToken ct = default);

	Task<ClientSignInResult> SignUp(ClientSignUpRequest request, CancellationToken ct = default);
}