import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { MessageService } from "primeng/api";

import { HttpServiceBase } from "../../../../infrasructure/http";
import { environment } from "../../../../../environments/environment";
import { AppStorage, EventBroadcastingService, Message } from "../../../../utils";
import { IAppSettingsSet } from "./app-configuration-models";
import {
	AppConfigurationBroadcastEvents,
	AppConfigurationDefaults,
	AppConfigurationStorageKeys
} from "./app-configuration-static";
import { AccountBroadcastEvents } from "../../../account";


@Injectable({ providedIn: "root" })
export class AppConfigurationService extends HttpServiceBase {
	private readonly _urlPrefix = `${environment.apiUrl}app-configuration`;
	private _settings = AppStorage.getItem<IAppSettingsSet[]>(AppConfigurationStorageKeys.AppConfiguration);


	constructor(
		private eventBroadcastingService: EventBroadcastingService,
		private messageService: MessageService,
		http: HttpClient) {
		super(http);

		this.initSubscriptions();
	}


	getSettingsSet<T>(sectionName: string): T {
		const settingsSet = this.getSettingsSetByName(sectionName);
		return settingsSet ? settingsSet.value as T : null;
	}

	init(): Promise<void> {
		return this.loadSettings();
	}

	saveSettings(settings: IAppSettingsSet): Promise<IAppSettingsSet> {
		return this.handleRequest(this.http.post<IAppSettingsSet>(this._urlPrefix, settings))
			.then(settingsResult => {
				const oldSettingsSectionIndex = this._settings.findIndex(x =>
					x.name == settingsResult.name);
				if (oldSettingsSectionIndex >= 0) {
					this._settings.splice(oldSettingsSectionIndex, 1, settingsResult);
				}
				AppStorage.setItem(AppConfigurationStorageKeys.AppConfiguration, this._settings);

				this.eventBroadcastingService.broadcast(
					AppConfigurationBroadcastEvents.AppSettingsSetChanged,
					settingsResult);

				this.messageService.add(Message.Success(AppConfigurationDefaults.AppSettingsSetSavedMessage));

				return settingsResult;
			});
	}


	private getSettingsSetByName(sectionName: string): IAppSettingsSet {
		return this._settings.find(settingsSectionItem => settingsSectionItem.name == sectionName);
	}

	private initSubscriptions(): void {
		this.eventBroadcastingService
			.on(AccountBroadcastEvents.IdentityChanged)
			.subscribe(() => this.loadSettings());
	}

	private loadSettings(): Promise<void> {
		return this.handleRequest<IAppSettingsSet[]>(this.http.get<IAppSettingsSet[]>(this._urlPrefix))
			.then(settings => {
				this._settings = settings;
				AppStorage.setItem(AppConfigurationStorageKeys.AppConfiguration, this._settings);
			});
	}
}