import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { InputNumberModule } from "primeng/inputnumber";

import { InputNumberRangeComponent } from "./input-number-range.component";


@NgModule({
    imports: [ CommonModule, FormsModule, ReactiveFormsModule, InputNumberModule ],
    declarations: [ InputNumberRangeComponent ],
    exports: [ InputNumberRangeComponent ]
})
export class InputNumberRangeModule {}