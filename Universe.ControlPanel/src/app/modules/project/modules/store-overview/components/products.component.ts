import { Component } from "@angular/core";
import { ProductService } from "../services";
import {
	DisplayMode,
	GridValidator,
	IGridColumn,
	IGridColumnFilterSettings,
	IGridSettings,
	ITableSettings,
	UpdateMode
} from "@features/grid";
import { minValidator, notEmptyValidator } from "../../../../../validators";
import { FilterMatchMode } from "primeng/api";
import { FilterInputType, FilterType } from "@features/filter";


@Component({
	selector: "products",
	templateUrl: "./products.component.html"
})
export class ProductsComponent {
	_gridSettings = <IGridSettings> {
		updateMode: UpdateMode.Inline,
		filtersRow: true
	};

	_tableSettings = <ITableSettings> {
		columns: <IGridColumn[]> [
			{
				field: "name",
				header: "Name",
				validators: [ new GridValidator(notEmptyValidator(), "required") ],
				filterSettings: <IGridColumnFilterSettings> {
					matchMode: FilterMatchMode.STARTS_WITH,
					matchModeSelectorVisible: false
				}
			},
			{
				field: "price",
				header: "Price",
				validators: [ new GridValidator(minValidator(0)) ],
				displayMode: DisplayMode.Number,
				decimalPlaces: 2,
				numericInputMin: 0,
				numericInputStep: 1,
				filterSettings: <IGridColumnFilterSettings> {
					matchMode: FilterMatchMode.LESS_THAN_OR_EQUAL_TO,
					numericInputMaxFractionDigits: 2,
					filterType: FilterType.Numeric,
					inputType: FilterInputType.Number
				}
			}
		]
	};


	public constructor(productService: ProductService) {
		this._gridSettings.dataService = productService;
	}
}