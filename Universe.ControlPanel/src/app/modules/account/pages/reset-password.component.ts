import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { HttpErrorResponse } from "@angular/common/http";
import { ActivatedRoute } from "@angular/router";

import { firstValueFrom } from "rxjs";
import { Message } from "primeng/api";

import { IPasswordReset, ISignInRequest } from "../account-models";
import { AccountService } from "../account.service";
import { matchValidator, notEmptyValidator } from "../../../validators";
import { HttpResponseHandler } from "../../../infrasructure/http";


@Component({
	templateUrl: "./reset-password.component.html",
	styleUrls: ["./reset-password.component.scss"]
})
export class ResetPasswordComponent implements OnInit {
	badLinkMessage: Message[];
	form = new FormGroup({
		password: new FormControl("", notEmptyValidator()),
		passwordConfirmation: new FormControl("", [notEmptyValidator(), matchValidator("password")]),
	});
	formRequestPending = false;
	passwordReset: IPasswordReset;
	resultMessage: Message[];


	constructor(private activatedRoute: ActivatedRoute, private accountService: AccountService) {}


	ngOnInit(): void {
		firstValueFrom(this.activatedRoute.queryParams).then(queryParams => {
			this.passwordReset = {
				userId: queryParams["userId"],
				token: queryParams["token"],
				password: ""
			};

			if (!this.passwordReset.userId || !this.passwordReset.token) {
				this.badLinkMessage = <Message[]> [{ severity: "error", summary: "Ссылка содержит недостаточно данных" }];
			}
		});
	}

	resetPassword(): void {
		this.formRequestPending = true;
		this.resultMessage = null;
		this.passwordReset.password = this.form.value["password"];
		this.accountService.resetPassword(this.passwordReset)
			.then(() =>
				this.resultMessage = [{
					severity: "success",
					summary: "Пароль успешно изменён"
				}])
			.catch((errorResponse: HttpErrorResponse) =>
				this.resultMessage = [{
					severity: "error",
					summary: HttpResponseHandler.getErrorMessageFromResponse(errorResponse)
				}])
			.finally(() => this.formRequestPending = false);
	}
}