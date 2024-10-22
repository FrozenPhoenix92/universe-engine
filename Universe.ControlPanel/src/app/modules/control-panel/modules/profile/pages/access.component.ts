import { Component } from "@angular/core";

import { IProfilePersonal } from "../profile-models";


@Component({
	templateUrl: "./access.component.html"
})
export class AccessComponent {
	profilePersonal: IProfilePersonal;
}