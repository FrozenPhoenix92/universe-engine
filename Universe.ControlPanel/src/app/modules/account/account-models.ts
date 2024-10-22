export interface IEmailConfirmation {
	userId: string;
	token: string;
}

export interface IPasswordReset {
	password: string;
	token: string;
	userId: string;
}

export interface IPasswordRecovery {
	email: string;
}

export interface ISignInRequest {
	captcha?: string;
	login: string;
	password: string;
	twoFactorAuthenticationCode?: string;
}

export interface ISignInResponse {
	sessionData: ISessionData;
	twoFactorCodeRequired: boolean;
}

export interface ISignUpRequest {
	email: string;
	firstName: string;
	lastName: string;
	userName: string;
	password: string;
}

export interface ISignUpResponse {
	approvalRequired: boolean;
	emailConfirmationRequired: boolean;
	phoneNumberConfirmationRequired: boolean;
	signInResponse: ISignInResponse;
}

export interface ISessionData {
	email: string;
	firstName: string;
	lastName: string;
	permissions: string[];
	phoneNumber: string;
	roles: string[];
	userId: string;
	userName: string;
}