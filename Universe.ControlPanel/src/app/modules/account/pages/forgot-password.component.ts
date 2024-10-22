import { Component } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { HttpErrorResponse } from "@angular/common/http";

import { Message } from "primeng/api";

import { IPasswordRecovery } from "../account-models";
import { AccountService } from "../account.service";
import { notEmptyValidator } from "../../../validators";
import { HttpResponseHandler } from "../../../infrasructure/http";


@Component({
	templateUrl: "./forgot-password.component.html",
	styleUrls: ["./forgot-password.component.scss"]
})
export class ForgotPasswordComponent {
	form = new FormGroup({
		email: new FormControl("", notEmptyValidator)
	});
	formRequestPending = false;
	resultMessage: Message[];


	constructor(private accountService: AccountService) {}


	recoverPassword(): void {
		this.formRequestPending = true;
		this.resultMessage = null;
		this.accountService.recoverPassword(this.form.value as IPasswordRecovery)
			.then(() =>
				this.resultMessage = [{
					severity: "success",
					summary: "На указанную почту было отправлено письмо со ссылкой для сброса пароля"
				}])
			.catch((errorResponse: HttpErrorResponse) =>
				this.resultMessage = [{
					severity: "error",
					summary: HttpResponseHandler.getErrorMessageFromResponse(errorResponse)
				}])
			.finally(() => this.formRequestPending = false);
	}
}