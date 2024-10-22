import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { HttpErrorResponse } from "@angular/common/http";

import { firstValueFrom } from "rxjs";
import { Message } from "primeng/api";

import { AccountService } from "../account.service";
import { IEmailConfirmation } from "../account-models";
import { HttpResponseHandler } from "../../../infrasructure/http";


@Component({
	templateUrl: "./email-confirmation.component.html",
	styleUrls: ["./email-confirmation.component.scss"]
})
export class EmailConfirmationComponent implements OnInit {
	resultMessage: Message[];

	constructor(private activatedRoute: ActivatedRoute, private accountService: AccountService) {}

	ngOnInit(): void {
		firstValueFrom(this.activatedRoute.queryParams).then(queryParams => {
			const emailConfirmation = <IEmailConfirmation> {
				userId: queryParams["userId"],
				token: queryParams["token"]
			};

			if (!emailConfirmation.userId || !emailConfirmation.token) {
				this.resultMessage = <Message[]> [{ severity: "error", summary: "Ссылка содержит недостаточно данных" }];
			} else {
				this.accountService.confirmEmail(emailConfirmation)
					.then(() => this.resultMessage = <Message[]> [{ severity: "success", summary: "Адрес электронной почты подтверждён" }])
					.catch((errorResponse: HttpErrorResponse) =>
						this.resultMessage = [{
							severity: "error",
							summary: HttpResponseHandler.getErrorMessageFromResponse(errorResponse)
						}])
			}
		});
	}
}