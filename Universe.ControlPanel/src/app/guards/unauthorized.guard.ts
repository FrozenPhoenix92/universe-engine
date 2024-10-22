import { Injectable } from "@angular/core";
import {
	ActivatedRouteSnapshot,
	CanActivate, CanActivateChild,
	Router,
	RouterStateSnapshot,
	UrlTree
} from "@angular/router";
import { AccountService } from "../modules/account";


@Injectable({ providedIn: "root" })
export class UnauthorizedGuard implements CanActivate, CanActivateChild {
	constructor(private router: Router, private accountService: AccountService) {}

	canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
		if (this.accountService.isAuthenticated) {
			return this.router.parseUrl("/control-panel/store-overview");
		} else {
			return true;
		}
	}

	canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
		return this.canActivate(childRoute, state);
	}
}