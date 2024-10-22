import {
	AbstractControl,
	NG_VALIDATORS,
	ValidationErrors,
	Validator,
	ValidatorFn
} from "@angular/forms";
import { Directive, Input } from "@angular/core";


export function minValidator(minValue?: number): ValidatorFn {
	return (control: AbstractControl): ValidationErrors | null => {
		if (!control.value || minValue == null) return null;

		const numValue = parseFloat(control.value);

		if (isNaN(numValue) || numValue < minValue) {
			return { min: true };
		}

		return null;
	};
}

@Directive({
	selector: "[min][formControlName],[min][formControl],[min][ngModel]",
	providers: [{ provide: NG_VALIDATORS, useExisting: MinValidatorDirective, multi: true }]
})
export class MinValidatorDirective implements Validator {
	@Input() min?: number;

	validate(control: AbstractControl): ValidationErrors | null {
		return minValidator(this.min)(control);
	}
}