import { ChangeDetectorRef } from "@angular/core";

import { GridInternalBase } from "./grid-internal-base";
import { ExportFormat } from "../enums/export-format";


export abstract class GridPublicBase extends GridInternalBase {
    protected constructor(_cd: ChangeDetectorRef) {
        super(_cd);
    }


    get id(): any {
        return this._id;
    }

    /** Shows whether the table performs a background process. */
    get pending(): boolean {
        return this._crudManager?.pending || this._dataEditor?.pending;
    }

    /** Shows whether the table is in a printing state now. */
    get printing(): boolean {
        return this._printing;
    }


    /** Clears stored table state. It might be useful if "stateStorage" and "stateKey" were defined. */
    clearState(): void {
        this._table.clearState();
    }

    /** Performs exporting in specified format. */
    export(format: ExportFormat): void {
        this._exporter.export(format);
    }

    /** Returns a string representation of the specified row value at the specified column. */
    getCellHandledStringValue(rowData: any, columnName: string): string {
        const column = this._table?.columns.find(x => x.field == columnName);
        return column ? this._dataViewer.getCellDisplayValue(rowData, column) : "";
    }

    /** Extracts a value from the wrapped Table component. */
    getTableProperty(propertyName: string): any {
        return this._table[propertyName];
    }

    performCreate(data: any): Promise<any> {
        return this._dataEditor.performCreate(data);
    }

    performUpdate(data: any): Promise<any> {
        return this._dataEditor.performUpdate(data);
    }

    refreshView(): void {
        this._cd.detectChanges();
    }

    /** Reloads the data if table is lazy-loaded. */
    reload(): void {
        if (this._table.lazy && this._gridSettings.dataService) {
            this._crudManager.loadTable(this._queryManager.getLazyLoadMetadata());
        }
    }

    /** Resets sort, filter and paginator state. */
    reset(): void {
        this._table.reset();
    }

    /** Resets the scrollable table scroll position to the beginning. */
    resetScrollTop(): void {
        this._table.resetScrollTop();
    }

    /** Scrolls to the row with the given index when virtual scrolling is enabled. */
    scrollToVirtualIndex(index: number): void {
        this._table.scrollToVirtualIndex(index);
    }

    /** Scrolls to a position of a scrollable table viewport. */
    scrollTo(options: any): void {
        this._table.scrollTo(options);
    }

    /** Sets a value in the wrapped Table component. */
    setTableProperty(propertyName: string, data: any): void {
        this._setTableProperty(propertyName, data);
        this._cd.detectChanges();
    }

    /** Stores table state if "stateStorage" and "stateKey" were defined.
     * Overrides the Table's "saveState" to avoid redundant data storings. */
    trySaveState(): void {
        if (this._table.stateStorage && this._table.stateKey) {
            this._table.saveState();
        }
    }
}