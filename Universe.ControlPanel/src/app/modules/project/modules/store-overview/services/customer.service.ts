import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { CrudService } from "../../../../../infrasructure/http";
import { environment } from "../../../../../../environments/environment";
import { ICustomer } from "../store-overview-models";


@Injectable()
export class CustomerService extends CrudService<ICustomer> {
	protected readonly urlPrefix = `${environment.apiUrl}customer`;


	constructor(httpClient: HttpClient) {
		super(httpClient);
	}
}