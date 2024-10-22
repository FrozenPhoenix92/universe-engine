import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { InputTextModule } from "primeng/inputtext";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { DropdownModule } from "primeng/dropdown";
import { MultiSelectModule } from "primeng/multiselect";
import { InputNumberModule } from "primeng/inputnumber";
import { CalendarModule } from "primeng/calendar";

import { FilterComponent } from "./filter.component";
import { FilterCustomTemplateDirective } from "./filter-custom-template.directive";
import { InputNumberRangeModule } from "../input-number-range";

@NgModule({
    declarations: [ FilterComponent, FilterCustomTemplateDirective ],
    exports: [ FilterComponent, FilterCustomTemplateDirective ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        InputNumberRangeModule,
        CalendarModule,
        InputNumberModule,
        InputTextModule,
        ButtonModule,
        CheckboxModule,
        DropdownModule,
        MultiSelectModule
    ]
})
export class FilterModule {}
