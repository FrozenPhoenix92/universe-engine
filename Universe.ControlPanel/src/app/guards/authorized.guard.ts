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

import { AccountService } from "../modules/account";


@Injectable({ providedIn: "root" })
export class AuthorizedGuard implements CanLoad, CanActivate, CanActivateChild {
	constructor(private router: Router, private accountService: AccountService) {}


	canLoad(route: Route, segments: UrlSegment[]): boolean | UrlTree {
		return this.isAuthorizedCheck();
	}

	canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
		return this.isAuthorizedCheck();
	}

	canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
		return this.canActivate(childRoute, state);
	}


	private isAuthorizedCheck(): boolean | UrlTree {
		if (this.accountService.isAuthenticated) {
			return true;
		} else {
			return this.router.parseUrl("/sign-in");
		}
	}
}