import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { HttpErrorResponse } from "@angular/common/http";

import { Message } from "primeng/api";

import { AccountService } from "../account.service";
import { matchValidator, notEmptyValidator } from "../../../validators";
import { ISignUpRequest, ISignUpResponse } from "../account-models";
import { HttpResponseHandler } from "../../../infrasructure/http";


@Component({
	templateUrl: "./sign-up.component.html",
	styleUrls: [ "./sign-up.component.scss" ]
})
export class SignUpComponent implements OnInit {
	errorMessage: Message[];
	form: FormGroup;
	formRequestPending = false;
	initialized = false;
	response: ISignUpResponse;
	resultMessage: Message[];
	selfSignUpAllowed: boolean;
	editDirectionDialogVisible = false;


	constructor(public accountService: AccountService) {}


	ngOnInit(): void {
		this.init();
	}

	signUp(): void {
		this.formRequestPending = true;
		this.errorMessage = null;
		this.accountService.signUp(this.form.value as ISignUpRequest)
			.then(result => {
				const successMessage = <Message>{
					severity: "success",
					summary: "Регистрация прошла успешно"
				};

				if (result.emailConfirmationRequired) {
					successMessage.detail = "Для завершения регистрации необходимо подтвердить адрес электронной почты";
				}
				if (result.phoneNumberConfirmationRequired) {
					successMessage.detail = "Для завершения регистрации необходимо подтвердить номер телефона";
				}
				if (result.approvalRequired) {
					successMessage.detail = "Для активации аккаунта требуется подтверждение администратора";
				}

				this.resultMessage = [successMessage];
				this.response = result;
			})
			.catch((errorResponse: HttpErrorResponse) =>
				this.errorMessage = [{
					severity: "error",
					summary: HttpResponseHandler.getErrorMessageFromResponse(errorResponse)
				}])
			.finally(() => this.formRequestPending = false);
	}


	private async init(): Promise<void> {
		this.selfSignUpAllowed = await this.accountService.selfSignUpAllowed();

		if (this.selfSignUpAllowed) {
			const formModel = {
				userName: new FormControl(
					"",
					this.accountService.isSameUserNameEmail ? Validators.email : notEmptyValidator()),
				password: new FormControl("", notEmptyValidator()),
				passwordConfirmation: new FormControl("", [notEmptyValidator(), matchValidator("password")]),
				firstName: new FormControl("", notEmptyValidator()),
				lastName: new FormControl("", notEmptyValidator())
			};

			if (!this.accountService.isSameUserNameEmail) {
				formModel["email"] = new FormControl("", notEmptyValidator());
			}

			this.form = new FormGroup(formModel);
		}

		this.initialized = true;
	}
}