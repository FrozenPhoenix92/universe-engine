import { Directive, Input } from "@angular/core";
import {
	AbstractControl,
	NG_VALIDATORS,
	NgModel,
	ValidationErrors,
	Validator,
	ValidatorFn
} from "@angular/forms";


export function matchValidator(compareWith: NgModel | AbstractControl | string): ValidatorFn {
	return (control: AbstractControl): ValidationErrors | null => {
		if (!compareWith) return null;

		const compareWithControl = (compareWith["control"] ||
			control.parent?.get(compareWith as string) ||
			compareWith) as AbstractControl;

		if (compareWithControl == null || control.value == null) return null;

		if (control.value !== compareWithControl.value) {
			return { "match": true };
		}

		return null;
	};
}

@Directive({
	selector: "[match][formControlName],[match][formControl],[match][ngModel]",
	providers: [{ provide: NG_VALIDATORS, useExisting: MatchValidatorDirective, multi: true }]
})
export class MatchValidatorDirective implements Validator {
	@Input() match: NgModel | AbstractControl | string;

	validate(control: AbstractControl): ValidationErrors | null {
		return matchValidator(this.match)(control);
	}
}