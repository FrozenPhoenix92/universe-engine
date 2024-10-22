import { Directive, Input, TemplateRef, ViewContainerRef } from "@angular/core";

import { AccountService } from "../modules/account";


@Directive({
	selector: "[rightsRequirement]"
})
export class RightsRequirementDirective {
	constructor(private templateRef: TemplateRef<any>,
	            private viewContainer: ViewContainerRef,
	            private accountService: AccountService
	) {}

	@Input() set rightsRequirement(value: { roles?: string[], permissions?: string[], authenticated?: boolean }) {
		if (value.authenticated && this.accountService.isAuthenticated ||
			this.accountService.matchRights(value.roles, value.permissions)) {
			this.viewContainer.createEmbeddedView(this.templateRef);
		} else {
			this.viewContainer.clear();
		}
	}
}