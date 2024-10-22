import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { CrudService } from "../../../../infrasructure/http";
import { IPermission } from "./permission-models";
import { environment } from "../../../../../environments/environment";


@Injectable({ providedIn: "root" })
export class PermissionService extends CrudService<IPermission> {
	protected readonly urlPrefix = `${environment.apiUrl}permission`;

	constructor(httpClient: HttpClient) {
		super(httpClient);
	}
}