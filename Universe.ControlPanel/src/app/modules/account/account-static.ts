import { CoreRoles } from "../control-panel";

export const AccountStorageKeys = {
	Session: "session-data",
	LogoutReasonMessage: "logout-reason-message",
	LastVisitedPageUrl: "last-visited-page-url"
};

export const AccountBroadcastEvents = {
	IdentityChanged: "IdentityChangedEvent",
	SignIn: "SignInEvent",
	SignOut: "SignOutEvent"
};

export class AccountDefaults {
	static SuperRole = CoreRoles.SuperAdmin;
	static SessionExpiredMessage = "Время сессии истекло. Необходима повторная авторизация.";
}