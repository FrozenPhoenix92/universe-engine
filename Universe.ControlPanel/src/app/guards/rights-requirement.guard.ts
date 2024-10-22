import { Injectable } from "@angular/core";
import {
	ActivatedRouteSnapshot,
	CanActivate, CanActivateChild,
	CanLoad,
	Route,
	Router,
	RouterStateSnapshot,
	UrlSegment,
	UrlTree
} from "@angular/router";

import { MessageService } from "primeng/api";

import { AccountService } from "../modules/account";
import { Message } from "../utils";


@Injectable({ providedIn: "root" })
export class RightsRequirementGuard implements CanLoad, CanActivate, CanActivateChild {
	constructor(private router: Router, private messageService: MessageService, private accountService: AccountService) {}


	canLoad(route: Route, segments: UrlSegment[]): boolean | UrlTree {
		return this.hasAccess(route.data.roles, route.data.permissions);
	}

	canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
		return this.hasAccess(route.data.roles, route.data.permissions);
	}

	canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
		return this.canActivate(childRoute, state);
	}


	private hasAccess(allowedRoles: string[], allowedPermissions?: string[]): boolean | UrlTree {
		if (this.accountService.matchRights(allowedRoles, allowedPermissions)) return true;

		this.messageService.add(Message.Error("Доступ запрещён."));
		return false;
	}
}