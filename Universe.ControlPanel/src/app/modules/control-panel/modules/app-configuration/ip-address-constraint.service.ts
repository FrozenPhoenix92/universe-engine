import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { CrudService } from "../../../../infrasructure/http";
import { IIpAddressConstraint } from "./app-configuration-models";
import { environment } from "../../../../../environments/environment";


@Injectable()
export class IpAddressConstraintService extends CrudService<IIpAddressConstraint>{
	protected readonly urlPrefix = `${environment.apiUrl}ip-address-constraint`;

	constructor(httpClient: HttpClient) {
		super(httpClient);
	}
}