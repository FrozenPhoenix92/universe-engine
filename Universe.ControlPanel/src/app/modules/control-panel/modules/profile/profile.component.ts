import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";

import { MenuItem } from "primeng/api";

import { IProfilePersonal } from "./profile-models";
import { PersonalComponent } from "./pages/personal.component";
import { AccessComponent } from "./pages/access.component";


@Component({
	templateUrl: "./profile.component.html",
	styleUrls: ["./profile.component.scss"]
})
export class ProfileComponent {
	activeItem: MenuItem;
	profilePersonal: IProfilePersonal;
	profileMenuModel: MenuItem[] = [
		{ label: "Персональные данные", routerLink: "/control-panel/profile/personal" },
		{ label: "Настройки доступа", routerLink: "/control-panel/profile/access" }
	];


	constructor(private activatedRoute: ActivatedRoute) {
		this.activeItem = this.profileMenuModel[0];
		this.profilePersonal = activatedRoute.snapshot.data.profilePersonal as IProfilePersonal;
	}


	onProfilePartActivated(profilePartComponent: any): void {
		if (profilePartComponent instanceof PersonalComponent ||
			profilePartComponent instanceof AccessComponent) {
			profilePartComponent.profilePersonal = this.profilePersonal;
		}
	}
}