import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { CrudService } from "../../../../../infrasructure/http";
import { environment } from "../../../../../../environments/environment";
import { IProduct } from "../store-overview-models";


@Injectable()
export class ProductService extends CrudService<IProduct> {
	protected readonly urlPrefix = `${environment.apiUrl}product`;


	constructor(httpClient: HttpClient) {
		super(httpClient);
	}
}