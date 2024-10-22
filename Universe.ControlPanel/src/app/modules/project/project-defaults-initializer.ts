import { MessageService } from "primeng/api";
import * as moment from "moment";

import { HttpDefaults, IHttpResultHandler, PrimeNgMessageHttpResultHandler } from "../../infrasructure/http";


/** Инициализатор, в котором можно переопределить значения, используемые по умолчанию различными механизмами. */
export class ProjectDefaults {
	static AuthorizedRoute = "customer-orders";


	static Init(messageService: MessageService): void {
		moment.locale("en");

		ProjectDefaults.SetHttpDefaults(messageService);
		ProjectDefaults.SetPrimeNgDefaults();
		ProjectDefaults.SetAccountDefaults();
	}


	private static SetAccountDefaults(): void {
		// AccountDefaults.SessionExpiredMessage = "";
	}

	private static SetHttpDefaults(messageService: MessageService): void {
		HttpDefaults.ResultHandler = <T = any>(): IHttpResultHandler<T> =>
			new PrimeNgMessageHttpResultHandler(messageService);

		// HttpDefaults.ErrorBadRequestMessage = "";
		// HttpDefaults.ErrorForbiddenMessage = "";
		// HttpDefaults.ErrorMessage = "";
		// HttpDefaults.SuccessMessage = "";
	}

	private static SetPrimeNgDefaults(): void {
		// PrimeNgDefaults.MessageErrorSummary = "";
		// PrimeNgDefaults.MessageInfoSummary = "";
		// PrimeNgDefaults.MessageSuccessSummary = "";
		// PrimeNgDefaults.MessageWarningSummary = "";
	}
}