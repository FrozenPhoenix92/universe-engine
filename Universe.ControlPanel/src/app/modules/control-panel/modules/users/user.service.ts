import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { CrudService } from "../../../../infrasructure/http";
import { IUser } from "./user-models";
import { environment } from "../../../../../environments/environment";


@Injectable({ providedIn: "root" })
export class UserService extends CrudService<IUser> {
	protected readonly urlPrefix = `${environment.apiUrl}user`;
	private _superAdminExists: boolean;


	constructor(httpClient: HttpClient) {
		super(httpClient);
	}


	get superAdminExists(): boolean { return this._superAdminExists; }


	initSuperAdmin(data: IUser): Promise<void> {
		return this.handleRequest(this.http.post<void>(`${this.urlPrefix}/init-super-admin`, data))
			.then(() => {
				this._superAdminExists = true;
			});
	}

	init(): Promise<void> {
		return this.handleRequest<boolean>(this.http.get<boolean>(`${this.urlPrefix}/super-admin-exists`))
			.then(result => { this._superAdminExists = result; });
	}
}