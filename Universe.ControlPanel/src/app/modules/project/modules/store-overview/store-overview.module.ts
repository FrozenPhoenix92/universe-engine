import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { InputTextModule } from "primeng/inputtext";
import { InputNumberModule } from "primeng/inputnumber";
import { ButtonModule } from "primeng/button";
import { ConfirmDialogModule } from "primeng/confirmdialog";
import { TooltipModule } from "primeng/tooltip";
import { MenuModule } from "primeng/menu";
import { CheckboxModule } from "primeng/checkbox";
import { DialogModule } from "primeng/dialog";
import { TabViewModule } from "primeng/tabview";
import { DropdownModule } from "primeng/dropdown";
import { FieldsetModule } from "primeng/fieldset";
import { InputMaskModule } from "primeng/inputmask";
import { StepsModule } from "primeng/steps";
import { InputTextareaModule } from "primeng/inputtextarea";
import { MessagesModule } from "primeng/messages";
import { SelectButtonModule } from "primeng/selectbutton";
import { RadioButtonModule } from "primeng/radiobutton";
import { AccordionModule } from "primeng/accordion";
import { FileUploadModule } from "primeng/fileupload";
import { PanelModule } from "primeng/panel";
import { PaginatorModule } from "primeng/paginator";

import { GridModule } from "@features/grid";
import { FilterModule } from "@features/filter";
import { CustomerOrdersComponent, ProductsComponent } from "./components";
import { SharedModule } from "../../../../shared.module";
import { CustomerService, OrderService, ProductService } from "./services";
import { StoreOverviewPageComponent } from "./store-overview.page.component";


@NgModule({
	declarations: [
		CustomerOrdersComponent,
		StoreOverviewPageComponent,
		ProductsComponent
	],
	imports: [
		CommonModule,
		FormsModule,
		ReactiveFormsModule,

		InputTextModule,
		InputTextareaModule,
		InputNumberModule,
		ButtonModule,
		ConfirmDialogModule,
		TooltipModule,
		MenuModule,
		CheckboxModule,
		DialogModule,
		TabViewModule,
		DropdownModule,
		FieldsetModule,
		InputMaskModule,
		StepsModule,
		MessagesModule,
		SelectButtonModule,
		RadioButtonModule,
		AccordionModule,
		FileUploadModule,
		PanelModule,
		PaginatorModule,

		SharedModule,
		GridModule,
		FilterModule,
		RouterModule.forChild([
			{
				path: "",
				component: StoreOverviewPageComponent
			}
		])
	],
	providers: [
		CustomerService,
		OrderService,
		ProductService
	]
})
export class StoreOverviewModule {}