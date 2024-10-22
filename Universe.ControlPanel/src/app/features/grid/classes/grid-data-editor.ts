import { AbstractControl, FormControl, FormGroup } from "@angular/forms";

import { SelectItem } from "primeng/api";

import { GridComponent } from "../grid.component";
import { CellEditInputType } from "../enums/cell-edit-input-type";
import { CreateMode } from "../enums/create-mode";
import { UpdateMode } from "../enums/update-mode";
import { IGridColumn } from "../interfaces/grid-column";
import { GridHelper } from "./grid-helper";


export class GridDataEditor {
    readonly defaultCalendarYearRange = `1900:${new Date().getFullYear()}`;
    readonly defaultCalendarDateFormat = "mm/dd/yy";
    editingDialogVisible = false;

    private _creationState = false;
    private _editableRowFormGroup: FormGroup;
    private _editableRow: any;
    private _rowIndex: number;
    private _startEditingRowFormData: any;
    private _pending = false;
    private _updatingState = false;


    constructor(private _grid: GridComponent) {}


    get cancelButtonText(): string {
        return this._grid.gridSettings.cancelButtonText; 
    }

    get cellEditingInputTypeEnum(): any {
        return CellEditInputType; 
    }

    get columns(): IGridColumn[] {
        return this._grid._table.columns; 
    }

    get createModeEnum(): any {
        return CreateMode; 
    }

    get creationState(): boolean {
        return this._creationState; 
    }

    get dataChanged(): boolean {
        return JSON.stringify(this._editableRowFormGroup.value) != JSON.stringify(this._startEditingRowFormData);
    }

    get dataKey(): string {
        return this._grid._table.dataKey; 
    }

    get dialogRequired(): boolean {
        return this._grid.gridSettings.createMode == CreateMode.Dialog ||
            this._grid.gridSettings.updateMode == UpdateMode.Dialog;
    }

    get editableRow(): any {
        return this._editableRow; 
    }

    get editableRowIndex(): any {
        return this._rowIndex; 
    }

    get formGroup(): FormGroup {
        return this._editableRowFormGroup; 
    }

    get pending(): boolean {
        return this._pending; 
    }

    get saveButtonText(): string {
        return this._grid.gridSettings.saveButtonText; 
    }

    get updateModeEnum(): any {
        return UpdateMode; 
    }

    get updatingState(): boolean {
        return this._updatingState; 
    }


    getDropDownOptions(column: IGridColumn): SelectItem[] {
        if (column.cellEditingInputType != CellEditInputType.Dropdown &&
            column.cellEditingInputType != CellEditInputType.Multiselect ) return null;

        if (column.dropdownOptions) return column.dropdownOptions;

        if (column.dropdownOptionsGenerator) return column.dropdownOptionsGenerator(this._editableRowFormGroup.value);

        return null;
    }

    getErrorMessagesForControl(column: IGridColumn, control: AbstractControl): string[] {
        return Object.keys(control.errors).map(errorKeyItem => {
            const gridValidator = this._grid._getColumnValidators(column)?.find(validatorItem =>
                validatorItem?.errorKey == errorKeyItem);
            if (gridValidator?.errorMessage) return gridValidator.errorMessage;

            switch (errorKeyItem) {
                case "required": return "Обязательное поле";
                case "pattern": return "Значение не соответствует формату";
                case "min": return "Значение меньше допустимого";
                case "max": return "Значение больше допустимого";
                case "email": return "Значение не соответствует формату адреса электронной почты";
                case "minLength": return "Длина строки меньше допустимой";
                case "maxLength": return "Длина строки больше допустимой";
                default: return "";
            }
        }).filter(item => item);
    }

    isEditableColumn(column: IGridColumn): boolean {
        return column.editable !== false &&
            (column.field != this._grid._table.dataKey || this.creationState && this._grid.gridSettings.manuallyCreatableDataKey) &&
            (!this.updatingState || column.editableOnUpdate !== false) &&
            (!this.creationState || column.editableOnCreate !== false);
    }

    isRequiredColumn(column: IGridColumn): boolean {
        return this._grid._isRequiredColumn(column);
    }

    startCreation(): void {
        if (this._creationState || this._updatingState) {
            this.cancelEditing();
        }

        if (this._grid.gridSettings.createFunc) {
            this._grid.gridSettings.createFunc();
            return;
        }

        switch (this._grid.gridSettings.createMode) {
            case CreateMode.Dialog:
                this._creationState = true;
                this.initEditing();
                this.editingDialogVisible = true;
                break;
            case CreateMode.Redirect:
                this._grid._router.navigateByUrl(this._grid.gridSettings.createLink);
                break;
        }
    }

    startDeleting(rowData: any, rowIndex?: number): void {
        if (this._creationState || this._updatingState) {
            this.cancelEditing();
        }

        if (this._grid.gridSettings.deleteFunc) {
            this._grid.gridSettings.deleteFunc(rowData, rowIndex);
            return;
        }

        this._grid._confirmationService.confirm({
            header: "Подтверждение удаления",
            message: "Вы уверены, что хотите удалить элемент?",
            accept: () => {
                this._pending = true;
                this._grid._crudManager.delete(rowData[this._grid._table.dataKey], rowIndex)
                    .then(result => {
                        this._grid.delete.emit(result);
                        return result;
                    })
                    .finally(() => {
                        this._pending = false;
                        this._grid._cd.detectChanges();
                    });
            }
        });
    }

    startDeletingAll(): void {
        if (this._creationState || this._updatingState) {
            this.cancelEditing();
        }

        if (this._grid.gridSettings.deleteAllFunc) {
            this._grid.gridSettings.deleteAllFunc();
            return;
        }

        this._grid._confirmationService.confirm({
            message: "Are you sure that you want to delete all items?",
            accept: () => {
                this._pending = true;
                this._grid._crudManager.deleteAll()
                    .then(() => this._grid.deleteAll.emit())
                    .finally(() => {
                        this._pending = false;
                        this._grid._cd.detectChanges();
                    });
            }
        });
    }

    startUpdating(rowData: any, rowIndex?: number): void {
        if (this._creationState || this._updatingState) {
            this.cancelEditing();
        }

        if (this._grid.gridSettings.updateFunc) {
            this._grid.gridSettings.updateFunc(rowData, rowIndex);
            return;
        }

        switch (this._grid.gridSettings.updateMode) {
            case UpdateMode.Dialog:
                this._updatingState = true;
                this.initEditing(rowData);
                this.editingDialogVisible = true;
                break;
            case UpdateMode.Inline:
                this._updatingState = true;
                this.initEditing(rowData);
                break;
            case UpdateMode.Redirect:
                const parameterName = this._grid.gridSettings.updateLink.match(/:\w+/)[0].substring(1);
                const parameterValue = this.getParameterValueCaseInsensitive(rowData, parameterName);
                this._grid._router.navigateByUrl(this._grid._gridSettings.updateLink.replace(/:\w+/, parameterValue));
                break;
        }
    }

    cancelEditing(): void {
        if (this.dialogRequired) {
            if (this.editingDialogVisible) {
                this.editingDialogVisible = false;
                this._grid._cd.detectChanges();
            }
        }

        if (this._editableRow) {
            this._grid._table.cancelRowEdit(this._editableRow);
        }

        this._editableRow = null;
        this._rowIndex = null;
        this._startEditingRowFormData = null;
        this._editableRowFormGroup = null;
        this._updatingState = false;
        this._creationState = false;
    }

    performCreate(data: any): Promise<any> {
        this._pending = true;
        return this._grid._crudManager.create(data).then(result => {
            this.cancelEditing();
            this._grid.create.emit(result);
            return result;
        }).finally(() => {
            this._pending = false;
            this._grid._cd.detectChanges();
        });
    }

    performUpdate(data: any): Promise<any> {
        this._pending = true;
        return this._grid._crudManager.update(
            this._grid._table.dataKey ? data[this._grid._table.dataKey] : null,
            data,
            this._rowIndex)
            .then(result => {
                this.cancelEditing();
                this._grid.update.emit(result);
                return result;
            }).finally(() => {
                this._pending = false;
                this._grid._cd.detectChanges();
            });
    }

    saveEditing(): Promise<void> {
        if (this._pending) return Promise.reject("Operation already performing.");
        if (this._editableRowFormGroup.invalid) return Promise.reject("The form is invalid.");

        if (this._creationState) {
            return this.createEditing();
        }
        if (this._updatingState) {
            return this.updateEditing();
        }

        return Promise.reject("Editing is not in process.");
    }


    private createEditing(): Promise<any> {
        if (!this._editableRowFormGroup) return Promise.reject("Related form group not found.");

        let savingData = { ...this._editableRow };
        Object.keys(this._editableRowFormGroup.value)
            .forEach(keyItem => savingData[keyItem] = this._editableRowFormGroup.value[keyItem]);
        if (this._grid.gridSettings.transformBeforeCreate) {
            savingData = this._grid.gridSettings.transformBeforeCreate(savingData);
        }

        return this.performCreate(savingData);
    }

    private getDefaultValueForColumn(column: IGridColumn): any {
        if (column.defaultValue) return column.defaultValue;

        switch (column.cellEditingInputType) {
            case CellEditInputType.Checkbox: return false;
            case CellEditInputType.Number:
                return column.numericInputMin ? column.numericInputMin : 0;
            case CellEditInputType.Calendar:
            case CellEditInputType.Dropdown:
            case CellEditInputType.Multiselect:
                return null;
            default: return "";
        }
    }

    private initEditing(rowData?: any, rowIndex?: number): void {
        if (!rowData) rowData = {};
        this._rowIndex = rowIndex;
        this._editableRow = { ...rowData };
        const formGroupModel = {};
        this._grid._table.columns.filter(columnItem => this.isEditableColumn(columnItem))
            .forEach(columnItem => {
                const columnValue = GridHelper.getColumnFieldValue(rowData, columnItem);
                formGroupModel[columnItem.field] =
                    new FormControl(
                        columnValue != null
                            ? columnItem.cellEditingInputType == CellEditInputType.Calendar ? new Date(columnValue) : columnValue
                            : this.getDefaultValueForColumn(columnItem),
                        this._grid._getColumnValidators(columnItem)?.map(x => x.validator)
                    );
            });
        this._editableRowFormGroup = this._grid._fb.group(formGroupModel);
        this._startEditingRowFormData = this._editableRowFormGroup.value;
        this._grid._cd.markForCheck();
    }

    private updateEditing(): Promise<any> {
        if (!this._editableRowFormGroup) return Promise.reject("Related form group not found.");

        let savingData = { ...this._editableRow };
        Object.keys(this._editableRowFormGroup.value)
            .forEach(keyItem => savingData[keyItem] = this._editableRowFormGroup.value[keyItem]);
        if (this._grid.gridSettings.transformBeforeUpdate) {
            savingData = this._grid.gridSettings.transformBeforeUpdate(savingData);
        }

        return this.performUpdate(savingData);
    }

    private getParameterValueCaseInsensitive(object, key) {
        const asLowercase = key.toLowerCase();
        return object[Object.keys(object).find(k => k.toLowerCase() === asLowercase)];
    }
}