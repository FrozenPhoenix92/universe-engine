import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";

import { Message } from "primeng/api";

import { ISignInRequest } from "../account-models";
import { AccountService } from "../account.service";
import { notEmptyValidator } from "../../../validators";
import { HttpErrorResponse } from "@angular/common/http";
import { HttpResponseHandler } from "../../../infrasructure/http";


@Component({
	templateUrl: "./sign-in.component.html",
	styleUrls: ["./sign-in.component.scss"]
})
export class SignInComponent implements OnInit {
	errorMessage: Message[];
	form = new FormGroup({
		login: new FormControl("", notEmptyValidator),
		password: new FormControl("", notEmptyValidator)
	});
	formRequestPending = false;
	selfSignUpAllowed: boolean;


	constructor(private accountService: AccountService) {}


	ngOnInit(): void {
		this.init();
	}

	signIn(): void {
		this.formRequestPending = true;
		this.errorMessage = null;
		this.accountService.signIn(this.form.value as ISignInRequest)
			.catch((errorResponse: HttpErrorResponse) =>
				this.errorMessage = [{
					severity: "error",
					summary: HttpResponseHandler.getErrorMessageFromResponse(errorResponse)
				}])
			.finally(() => this.formRequestPending = false);
	}


	private async init(): Promise<void> {
		this.selfSignUpAllowed = await this.accountService.selfSignUpAllowed();
	}
}