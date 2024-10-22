import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { CrudService } from "../../../../../infrasructure/http";
import { environment } from "../../../../../../environments/environment";
import { IOrder, IProduct } from "../store-overview-models";
import { IQueryCommand } from "@features/filter";


@Injectable()
export class OrderService extends CrudService<IOrder> {
	protected readonly urlPrefix = `${environment.apiUrl}order`;


	constructor(httpClient: HttpClient) {
		super(httpClient);
	}


	getOrdersSum(queryCommand?: IQueryCommand): Promise<number> {
		return this.handleRequest<number>(
			this.http.get<number>(`${this.urlPrefix}/orders-sum`, { params: this.constructHttpParams(queryCommand) })
		);
	}

	getMostFrequentProduct(queryCommand?: IQueryCommand): Promise<{product: IProduct, count: number}> {
		return this.handleRequest<{product: IProduct, count: number}>(
			this.http.get<{product: IProduct, count: number}>(
				`${this.urlPrefix}/most-frequent-product`,
				{ params: this.constructHttpParams(queryCommand) })
		);
	}
}