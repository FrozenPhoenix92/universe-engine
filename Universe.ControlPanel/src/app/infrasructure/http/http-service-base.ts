import { HttpClient, HttpErrorResponse, HttpParams } from "@angular/common/http";

import { firstValueFrom, Observable } from "rxjs";

import { HttpResponseHandler, IHttpResponseHandler } from "./http-response-handler";
import { flatten } from "../../utils";
import { HttpDefaults } from "./http-static";


export abstract class HttpServiceBase {
	protected constructor(protected http: HttpClient) {}


	protected handleRequest<TResult>(
		request: Observable<TResult>,
		responseHandler?: IHttpResponseHandler<TResult>
	): Promise<TResult> {
		if (typeof responseHandler === "undefined") {
			responseHandler = new HttpResponseHandler<TResult>();
		}

		const promise = firstValueFrom(request);
		if (responseHandler) {
			return promise
				.then((response: TResult) => {
					if (responseHandler.handleSuccess) {
						responseHandler.handleSuccess(response);
					}
					return Promise.resolve(response);
				})
				.catch((errorResponse: HttpErrorResponse) => {
					if (responseHandler.handleError) {
						responseHandler.handleError(errorResponse);
					}
					return Promise.reject(errorResponse);
				});
		} else {
			return promise;
		}
	}

	protected constructHttpParams(obj: any): HttpParams {
		if (!obj) return null;

		let params = new HttpParams();
		const flattenBody = flatten(obj);

		Object.keys(flattenBody).forEach(prop => {
			params = params.set(prop, flattenBody[prop]);
		});

		return params;
	}
}