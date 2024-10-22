import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { firstValueFrom } from "rxjs";

import { environment } from "../../../environments/environment";
import { AccountStorageKeys, AccountBroadcastEvents, AccountDefaults } from "./account-static";
import {
	ISignInRequest,
	ISignInResponse,
	ISessionData,
	ISignUpRequest,
	ISignUpResponse,
	IEmailConfirmation, IPasswordRecovery, IPasswordReset
} from "./account-models";
import { HttpServiceBase } from "../../infrasructure/http";
import { AppStorage, EventBroadcastingService } from "../../utils";
import { IProfilePersonal } from "../control-panel/modules/profile";


@Injectable({ providedIn: "root" })
export class AccountService extends HttpServiceBase {
	private static _sessionData = AppStorage.getItem<ISessionData>(AccountStorageKeys.Session);
	private readonly _urlPrefix = `${environment.apiUrl}account`;
	private _isSameUserNameEmail: boolean;


	constructor(private eventBroadcastingService: EventBroadcastingService,
	            http: HttpClient) {
		super(http);
	}


	get isAuthenticated(): boolean {
		return AccountService._sessionData != null;
	}

	get isSameUserNameEmail(): boolean {
		return this._isSameUserNameEmail;
	}

	get sessionData(): ISessionData {
		return { ...AccountService._sessionData };
	}


	confirmEmail(data: IEmailConfirmation): Promise<void> {
		return this.handleRequest(this.http.post<void>(`${this._urlPrefix}/confirm-email`, data));
	}

	getProfilePersonal(): Promise<IProfilePersonal> {
		return this.handleRequest(this.http.get<IProfilePersonal>(`${this._urlPrefix}/profile-personal`));
	}

	getSessionData(): Promise<ISessionData> {
		return this.handleRequest(this.http.get<ISessionData>(`${this._urlPrefix}/session-data`));
	}

	init(): Promise<[void, void]> {
		return Promise.all([
			this.getSessionData().then(sessionData => this.setSessionData(sessionData)),
			this.handleRequest(this.http.get<boolean>(`${this._urlPrefix}/is-same-user-name-email`))
				.then(result => { this._isSameUserNameEmail = result; })
		]);
	}

	matchRights(allowedRoles: string | string[], allowedPermissions?: string | string[], allowSuperRole: boolean = true): boolean {
		return allowSuperRole && this.isInRole(AccountDefaults.SuperRole) ||
			allowedRoles?.length && this.isInRole(allowedRoles) ||
			allowedPermissions?.length && this.hasPermission(allowedPermissions);
	}

	recoverPassword(data: IPasswordRecovery): Promise<void> {
		return this.handleRequest(this.http.post<void>(`${this._urlPrefix}/recover-password`, data));
	}

	refreshAntiForgeryToken(): Promise<void> {
		return this.handleRequest(this.http.get<void>(`${this._urlPrefix}/refresh-anti-forgery-token`));
	}

	resetPassword(data: IPasswordReset): Promise<void> {
		return this.handleRequest(this.http.post<void>(`${this._urlPrefix}/reset-password`, data));
	}

	selfSignUpAllowed(): Promise<boolean> {
		return this.handleRequest(this.http.get<boolean>(`${this._urlPrefix}/self-sign-up-allowed`));
	}

	signIn(data: ISignInRequest): Promise<ISignInResponse> {
		return this.handleRequest(this.http.post<ISignInResponse>(`${this._urlPrefix}/sign-in`, data))
			.then(loginResponse => {
				this.handleSessionData(loginResponse.sessionData);
				return loginResponse;
			});
	}

	signOut(): Promise<void> {
		return this.handleRequest(this.http.post<void>(`${this._urlPrefix}/sign-out`, {}))
			.finally(() => this.signOutLocally());
	}

	async signOutLocally(): Promise<void> {
		this.clearSessionData();

		await this.refreshAntiForgeryToken();

		this.eventBroadcastingService.broadcast(AccountBroadcastEvents.SignOut);
		this.eventBroadcastingService.broadcast(AccountBroadcastEvents.IdentityChanged, null);
	}

	signUp(data: ISignUpRequest): Promise<ISignUpResponse> {
		return this.handleRequest(this.http.post<ISignUpResponse>(`${this._urlPrefix}/sign-up`, data))
			.then(loginResponse => {
				this.handleSessionData(loginResponse.signInResponse?.sessionData);
				return loginResponse;
			});
	}


	private clearSessionData(): void {
		this.setSessionData(null);
	}

	private handleSessionData(sessionData: ISessionData): void {
		if (!sessionData) return;

		this.setSessionData(sessionData);

		this.refreshAntiForgeryToken().finally(() => {
			this.eventBroadcastingService.broadcast(AccountBroadcastEvents.SignIn, sessionData);
			this.eventBroadcastingService.broadcast(AccountBroadcastEvents.IdentityChanged, sessionData);
		});
	}

	private hasPermission(permissions: string | string[]): boolean {
		if (!permissions) return false;

		const sessionData = AccountService._sessionData;

		if (!sessionData) return false;

		const permissionsArray = Array.isArray(permissions) ? <string[]>permissions : [permissions];

		return permissionsArray.some(x => sessionData.permissions.some(y => x === y));
	}

	private isInRole(roles: string | string[]): boolean {
		if (!roles) return false;

		const sessionData = AccountService._sessionData;

		if (!sessionData) return false;

		const rolesArray = Array.isArray(roles) ? <string[]>roles : [roles];

		return rolesArray.some(x => sessionData.roles.some(y => x === y));
	}

	private setSessionData(sessionData: ISessionData): void {
		AccountService._sessionData = sessionData;
		AppStorage.setItem(AccountStorageKeys.Session, sessionData);
	}
}