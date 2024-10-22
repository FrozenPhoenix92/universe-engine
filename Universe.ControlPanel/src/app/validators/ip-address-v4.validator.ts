import {
	AbstractControl,
	NG_VALIDATORS,
	ValidationErrors,
	Validator,
	ValidatorFn
} from "@angular/forms";
import { Directive } from "@angular/core";


export function ipAddressV4Validator(): ValidatorFn {
	return (control: AbstractControl): ValidationErrors | null => {
		if (!control.value) return null;

		const segments = control.value.split(".");

		if (segments?.length !== 4) {
			return { ipAddressV4: true };
		}

		for (let index = 0; index < 4; index++) {
			const numValue = parseInt(segments[index]);
			if (isNaN(numValue) || numValue < 0 || numValue > 255) {
				return { ipAddressV4: true };
			}
		}

		return null;
	};
}

@Directive({
	selector: "[ipAddressV4][formControlName],[ipAddressV4][formControl],[ipAddressV4][ngModel]",
	providers: [{ provide: NG_VALIDATORS, useExisting: IpAddressV4ValidatorDirective, multi: true }]
})
export class IpAddressV4ValidatorDirective implements Validator {
	validate(control: AbstractControl): ValidationErrors | null {
		return ipAddressV4Validator()(control);
	}
}