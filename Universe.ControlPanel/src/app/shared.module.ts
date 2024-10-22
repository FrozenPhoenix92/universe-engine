import { NgModule } from "@angular/core";
import {
	GreaterThanValidatorDirective, IpAddressV4ValidatorDirective,
	LessThanValidatorDirective,
	MatchValidatorDirective, MaxValidatorDirective, MinValidatorDirective,
	NotEmptyValidatorDirective
} from "./validators";
import { SafeHtmlPipe } from "./pipes";
import { RightsRequirementDirective } from "./directives";


@NgModule({
	declarations: [
		RightsRequirementDirective,
		GreaterThanValidatorDirective,
		LessThanValidatorDirective,
		MatchValidatorDirective,
		MaxValidatorDirective,
		MinValidatorDirective,
		NotEmptyValidatorDirective,
		IpAddressV4ValidatorDirective,
		SafeHtmlPipe
	],
	exports: [
		RightsRequirementDirective,
		GreaterThanValidatorDirective,
		LessThanValidatorDirective,
		MatchValidatorDirective,
		MaxValidatorDirective,
		MinValidatorDirective,
		NotEmptyValidatorDirective,
		SafeHtmlPipe
	]
})
export class SharedModule {}