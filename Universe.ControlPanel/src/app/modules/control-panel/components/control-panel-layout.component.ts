import { Component, OnDestroy, OnInit } from "@angular/core";
import { Router } from "@angular/router";

import { Subscription } from "rxjs";

import { EventBroadcastingService } from "../../../utils";
import { AccountBroadcastEvents } from "../../account";


@Component({
	templateUrl: "./control-panel-layout.component.html"
})
export class ControlPanelLayoutComponent implements OnInit, OnDestroy {
	private _logOutEventSubscription: Subscription;


	constructor(private eventBroadcastingService: EventBroadcastingService, private router: Router) {}


	ngOnInit(): void {
		this._logOutEventSubscription = this.eventBroadcastingService
			.on(AccountBroadcastEvents.SignOut)
			.subscribe(() => this.router.navigateByUrl("/sign-in"));
	}

	ngOnDestroy(): void {
		this._logOutEventSubscription.unsubscribe();
	}
}