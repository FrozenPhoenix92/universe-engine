import { Component, OnInit } from "@angular/core";

import { IProfilePersonal } from "../profile-models";
import { FormControl, FormGroup } from "@angular/forms";
import { notEmptyValidator } from "../../../../../validators";


@Component({
	templateUrl: "./personal.component.html"
})
export class PersonalComponent implements OnInit {
	form: FormGroup;
	profilePersonal: IProfilePersonal;


	ngOnInit(): void {
		this.form = new FormGroup({
			firstName: new FormControl(this.profilePersonal.firstName, notEmptyValidator()),
			lastName: new FormControl(this.profilePersonal.lastName, notEmptyValidator()),
			phoneNumber: new FormControl(this.profilePersonal.phoneNumber)
		});
	}
}