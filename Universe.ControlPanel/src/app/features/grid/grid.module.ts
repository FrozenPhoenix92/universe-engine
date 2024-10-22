import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { CheckboxModule } from "primeng/checkbox";
import { DropdownModule } from "primeng/dropdown";
import { MultiSelectModule } from "primeng/multiselect";
import { InputNumberModule } from "primeng/inputnumber";
import { CalendarModule } from "primeng/calendar";
import { TableModule } from "primeng/table";
import { SplitButtonModule } from "primeng/splitbutton";
import { ProgressBarModule } from "primeng/progressbar";
import { FileUploadModule } from "primeng/fileupload";
import { DialogModule } from "primeng/dialog";
import { ConfirmDialogModule } from "primeng/confirmdialog";

import { GridComponent } from "./grid.component";
import { GridColumnTemplate } from "./grid-template.directive";
import { GridEditingDialogComponent } from "./grid-editing-dialog.component";
import { GridImportDialogComponent } from "./grid-import-dialog.component";
import { InputNumberRangeModule } from "../input-number-range";
import { InputSwitchModule } from "primeng/inputswitch";


@NgModule({
    declarations: [ GridComponent, GridColumnTemplate, GridEditingDialogComponent, GridImportDialogComponent ],
    exports: [ GridComponent, GridColumnTemplate ],
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        ReactiveFormsModule,
	    InputNumberRangeModule,
        ButtonModule,
        CalendarModule,
	    InputNumberModule,
        InputTextModule,
        ButtonModule,
        CheckboxModule,
        InputSwitchModule,
        DropdownModule,
        MultiSelectModule,
        TableModule,
        SplitButtonModule,
        ProgressBarModule,
        FileUploadModule,
        DialogModule,
        ConfirmDialogModule
    ]
})
export class GridModule {}
