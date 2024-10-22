import {
	AbstractControl,
	NG_VALIDATORS,
	ValidationErrors,
	Validator,
	ValidatorFn
} from "@angular/forms";
import { Directive, Input } from "@angular/core";


export function maxValidator(maxValue?: number): ValidatorFn {
	return (control: AbstractControl): ValidationErrors | null => {
		if (!control.value || maxValue == null) return null;

		const numValue = parseFloat(control.value);

		if (isNaN(numValue) || numValue > maxValue) {
			return { max: true };
		}

		return null;
	};
}

@Directive({
	selector: "[max][formControlName],[max][formControl],[max][ngModel]",
	providers: [{ provide: NG_VALIDATORS, useExisting: MaxValidatorDirective, multi: true }]
})
export class MaxValidatorDirective implements Validator {
	@Input() max?: number;

	validate(control: AbstractControl): ValidationErrors | null {
		return maxValidator(this.max)(control);
	}
}