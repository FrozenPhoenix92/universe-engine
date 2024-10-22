import { Component } from "@angular/core";

import { IpAddressConstraintService } from "../ip-address-constraint.service";
import {
	CellEditInputType,
	GridValidator,
	IGridColumn,
	IGridSettings,
	ITableSettings,
	UpdateMode
} from "@features/grid";
import { getIpAddressConstraintRuleOptions } from "../app-configuration-models";
import { ipAddressV4Validator, notEmptyValidator } from "../../../../../validators";
import { FilterInputType, FilterType } from "@features/filter";


@Component({
	selector: "ip-address-constraints",
	templateUrl: "./ip-address-constraints.component.html"
})
export class IpAddressConstraintsComponent {
	gridSettings: IGridSettings = {
		updateMode: UpdateMode.Inline,
		filtersRow: true,
	};
	tableSettings: ITableSettings = {
		columns: <IGridColumn[]>[
			{
				field: "containingUrlPart",
				header: "Использование ограничения, если адрес запроса содержит подстроку"
			},
			{
				field: "rule",
				header: "Действие",
				defaultValue: 0,
				cellEditingInputType: CellEditInputType.Dropdown,
				dropdownOptions: getIpAddressConstraintRuleOptions(),
				filterSettings: {
					dropdownOptions: getIpAddressConstraintRuleOptions(),
					inputType: FilterInputType.Dropdown,
					filterType: FilterType.Numeric
				}
			},
			{
				field: "addressesRangeStart",
				header: "Начало диапазона адресов",
				validators: [
					new GridValidator(notEmptyValidator(), "required"),
					new GridValidator(ipAddressV4Validator())
				]
			},
			{
				field: "addressesRangeEnd",
				header: "Конец диапазона адресов",
				validators: [
					new GridValidator(ipAddressV4Validator())
				]
			}
		]
	};


	constructor(private ipAddressConstraintService: IpAddressConstraintService) {
		this.gridSettings.dataService = ipAddressConstraintService;
	}
}