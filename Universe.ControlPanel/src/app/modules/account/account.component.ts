import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { FormControl, FormGroup, Validators } from "@angular/forms";

import { Message } from "primeng/api";
import { Subscription } from "rxjs";

import { EventBroadcastingService } from "../../utils";
import { AccountBroadcastEvents } from "./account-static";
import { UserService } from "../control-panel/modules/users";
import { matchValidator, notEmptyValidator } from "../../validators";
import { ISignUpResponse } from "./account-models";
import { AccountService } from "./account.service";
import { HttpErrorResponse } from "@angular/common/http";
import { HttpResponseHandler } from "../../infrasructure/http";


@Component({
	templateUrl: "./account.component.html",
	styleUrls: [ "./account.component.scss" ]
})
export class AccountComponent implements OnInit, OnDestroy {
	errorMessage: Message[];
	form: FormGroup;
	formRequestPending = false;
	response: ISignUpResponse;
	resultMessage: Message[];
	private _logInEventSubscription: Subscription;


	constructor(public userService: UserService,
	            public accountService: AccountService,
	            private cd: ChangeDetectorRef,
	            private router: Router,
	            private eventBroadcastingService: EventBroadcastingService,
	            ) {
		this._logInEventSubscription = eventBroadcastingService
			.on(AccountBroadcastEvents.SignIn)
			.subscribe(() => this.router.navigateByUrl("/control-panel"));
	}


	ngOnInit(): void {
		this.init();
	}

	ngOnDestroy(): void {
		this._logInEventSubscription.unsubscribe();
	}


	initSuperAdmin(): void {
		this.formRequestPending = true;
		this.errorMessage = null;
		this.userService.initSuperAdmin(this.form.value)
			.then(() => this.cd.detectChanges())
			.catch((errorResponse: HttpErrorResponse) =>
				this.errorMessage = [{
					severity: "error",
					summary: HttpResponseHandler.getErrorMessageFromResponse(errorResponse)
				}])
			.finally(() => this.formRequestPending = false);
	}


	private init(): void {
		const formModel = {
			userName: new FormControl(
				"boyko_igor92@mail.ru",
				this.accountService.isSameUserNameEmail ? Validators.email : notEmptyValidator()),
			password: new FormControl("Carbon1992####", notEmptyValidator()),
			passwordConfirmation: new FormControl("Carbon1992####", [notEmptyValidator(), matchValidator("password")]),
			firstName: new FormControl("Игорь", notEmptyValidator()),
			lastName: new FormControl("Бойко", notEmptyValidator())
		};

		if (!this.accountService.isSameUserNameEmail) {
			formModel["email"] = new FormControl("", Validators.email);
		}

		this.form = new FormGroup(formModel);
	}
}