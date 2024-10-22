import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Injectable } from "@angular/core";

import { IProfilePersonal } from "./profile-models";
import { AccountService } from "../../../account";


@Injectable()
export class ProfileResolver implements Resolve<IProfilePersonal>{
	constructor(private service: AccountService) { }

	resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<IProfilePersonal> {
		return this.service.getProfilePersonal();
	}
}