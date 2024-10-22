import { SelectItem } from "primeng/api";

import { IEntity } from "../../../../infrasructure";


export enum IpAddressConstraintRule {
	Allow = 0,
	Forbid = 1
}
export function getIpAddressConstraintRuleDisplayingName(value: IpAddressConstraintRule): string {
	switch (value) {
		case IpAddressConstraintRule.Allow: return "Разрешить";
		case IpAddressConstraintRule.Forbid: return "Запретить";
	}

	throw new Error("Указанное правило не существует.");
}
export function getIpAddressConstraintRuleOptions(): SelectItem[] {
	return <SelectItem[]> [
		{ label: getIpAddressConstraintRuleDisplayingName(IpAddressConstraintRule.Allow), value: IpAddressConstraintRule.Allow },
		{ label: getIpAddressConstraintRuleDisplayingName(IpAddressConstraintRule.Forbid), value: IpAddressConstraintRule.Forbid }
	];
}


export interface IAppSettingsSet<T = any> {
	name: string,
	value: T;
	readOnly: boolean;
}

export interface IIpAddressConstraint extends IEntity {
	addressesRangeEnd?: string;
	addressesRangeStart: string;
	containingUrlPart?: string;
	rule: IpAddressConstraintRule;
}