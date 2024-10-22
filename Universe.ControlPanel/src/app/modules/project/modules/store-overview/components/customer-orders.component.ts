import { Component, ViewChild } from "@angular/core";
import { Router } from "@angular/router";

import { CustomerService, OrderService } from "../services";
import {
	DisplayMode, GridComponent,
	IGridColumn,
	IGridColumnFilterSettings,
	IGridSettings,
	ITableSettings,
	SortOrder
} from "../../../../../features/grid";
import { FilterInputType, FilterType, INumberFilter } from "@features/filter";
import { getOrderStatusDisplayingName, getOrderStatusOptions, ICustomer, IProduct } from "../store-overview-models";
import { FilterMatchMode } from "primeng/api";


@Component({
	selector: "customer-orders",
	templateUrl: "./customer-orders.component.html",
	styleUrls: ["./customer-orders.component.scss"]
})
export class CustomerOrdersComponent {
	customersGridSettings = <IGridSettings> {
		readonly: true,
		selectColumn: true
	};
	customersTableSettings = <ITableSettings> {
		selectionMode: "single",
		rows: 5,
		rowsPerPageOptions: null,
		columns: <IGridColumn[]> [
			{
				field: "name",
				header: "Name"
			},
			{
				field: "lastName",
				header: "Last Name"
			},
			{
				field: "address",
				header: "Address"
			},
			{
				field: "photo",
				header: "Photo",
				sortable: false,
				filterCellEnabled: false
			}
		]
	};
	ordersGridSettings = <IGridSettings> {
		filtersRow: true,
		readonly: true,
		rowExpansionEnabled: true,
		queryCommandTransform: queryCommand => {
			let customerIdFilter = <INumberFilter> queryCommand.filters.find(x => x.propertyName === "customerId");

			if (!customerIdFilter) {
				customerIdFilter = <INumberFilter> {
					propertyName: "customerId",
					$type: "number"
				};

				queryCommand.filters.push(customerIdFilter);
			}

			customerIdFilter.value = <number> (<ICustomer><any>this.customersGrid.selection).id;

			return queryCommand;
		},
		beforeDataRequest: queryCommand => {
			if (queryCommand.filters.every(x => x.propertyName !== "customerId")) {
				this.ordersSum = null;
				this.mostFrequentProduct = null;

				return;
			}

			this.orderService.getOrdersSum(queryCommand)
				.then(result => this.ordersSum = result)
				.catch(() => this.ordersSum = null);

			this.orderService.getMostFrequentProduct(queryCommand)
				.then(result => this.mostFrequentProduct = result)
				.catch(() => this.mostFrequentProduct = null);
		}
	};
	mostFrequentProduct: { product: IProduct, count: number };
	ordersSum: number;
	ordersTableSettings = <ITableSettings> {
		sortOrder: SortOrder.Desc,
		sortField: "created",
		columns: <IGridColumn[]> [
			{
				field: "created",
				header: "Created",
				displayMode: DisplayMode.Date,
				displayDateMomentFormat: "L LTS",
				filterSettings: <IGridColumnFilterSettings> {
					filterType: FilterType.Date,
					inputType: FilterInputType.Calendar,
					matchMode: FilterMatchMode.AFTER
				}
			},
			{
				field: "lastModified",
				header: "Last Modified",
				displayMode: DisplayMode.Date,
				displayDateMomentFormat: "L LTS",
				filterSettings: <IGridColumnFilterSettings> {
					filterType: FilterType.Date,
					inputType: FilterInputType.Calendar,
					matchMode: FilterMatchMode.AFTER
				}
			},
			{
				field: "status",
				header: "Status",
				displayHandler: cellValue => getOrderStatusDisplayingName(cellValue),
				filterSettings: <IGridColumnFilterSettings> {
					filterType: FilterType.Text,
					inputType: FilterInputType.Multiselect,
					dropdownOptions: getOrderStatusOptions(),
					matchMode: FilterMatchMode.IN,
					ignoreIfConvertibleToFalse: true
				}
			}
		]
	};

	@ViewChild("customersGrid", { static: true }) customersGrid: GridComponent;
	@ViewChild("ordersGrid", { static: false }) ordersGrid: GridComponent;


	constructor(private router: Router,
	            private customerService: CustomerService,
	            private orderService: OrderService) {
		this.customersGridSettings.dataService = customerService;
		this.ordersGridSettings.dataService = orderService;
	}


	onCustomerSelect(): void {
		this.ordersGrid?.reload();
	}
}