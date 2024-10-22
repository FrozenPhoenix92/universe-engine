import { LazyLoadEvent } from "primeng/api";
import buildQuery from "odata-query";

import { FilterType, IFilterItem, IQueryCommand, QueryCommand } from "@features/filter";
import { IGridColumn } from "../interfaces/grid-column";
import { GridComponent } from "../grid.component";
import { TableLazyLoadEvent } from "primeng/table";


type DataServiceGetFunc = (queryCommand?: IQueryCommand) => Promise<any[]> | Promise<any> | any[] | any;
type DataServiceGetTotalFunc = (queryCommand?: IQueryCommand) => Promise<number> | number;
type DataServiceCreateFunc = (newRowValue: any) => Promise<any>;
type DataServiceDeleteFunc = (dataKey: any) => Promise<void>;
type DataServiceDeleteAllFunc = () => Promise<void>;
type DataServiceODataGetFunc = (oDataQueryString: string) => Promise<any>;
type DataServiceUpdateFunc = (dataKey: any, changedRowValue: any) => Promise<any>;

export class GridCrudManager {
    private _pending = false;
    private _totalLoading = false;


    constructor(private _grid: GridComponent) {}


    get pending(): boolean {
        return this._pending;
    }

    get totalLoading(): boolean {
        return this._totalLoading;
    }


    create(rowData: any): Promise<any> {
        if (this._grid._table?.lazy && this._grid.gridSettings.dataService) {
            if (!this._grid.gridSettings.dataServiceCreateMethodName || !this._grid.gridSettings.dataService[this._grid.gridSettings.dataServiceCreateMethodName]) {
                return Promise.reject("Can not create the new record. Wrong grid configuration.");
            }

            this.setPendingState(true);
            return (this._grid.gridSettings.dataService[this._grid.gridSettings.dataServiceCreateMethodName] as DataServiceCreateFunc)(rowData)
                .then(response => {
                    this.loadTable(this._grid._queryManager.getLazyLoadMetadata());
                    return response;
                })
                .catch(error => {
                    this.setPendingState(false);
                    return Promise.reject(error);
                });
        } else {
            if (this._grid._table?.dataKey) {
                if (!rowData[this._grid._table.dataKey]) {
                    if (this._grid.gridSettings.dataKeyGenerator) {
                        rowData[this._grid._table.dataKey] = this._grid.gridSettings.dataKeyGenerator();
                    } else {
                        return Promise.reject("Impossible to add a new record. Either specify the record's dataKey directly or define the dataKeyGenerator function.");
                    }
                }
            }
            this._grid._table.value.push(rowData);
            this._grid._table.totalRecords++;
            this._grid._cd.detectChanges();
            return Promise.resolve(rowData);
        }
    }

    delete(dataKey: any, rowIndex?: number): Promise<void> {
        if (this._grid._table?.lazy && this._grid.gridSettings.dataService) {
            if (dataKey == null) {
                return Promise.reject("Unable to delete row. The 'dataKey' is undefined.");
            }

            this.setPendingState(true);
            return (this._grid.gridSettings.dataService[this._grid.gridSettings.dataServiceDeleteMethodName] as DataServiceDeleteFunc)(dataKey)
                .then(() => {
                    if (this._grid._table.selection) {
                        if (this._grid._table.selectionMode == "single") {
                            if (this._grid._table.selection[this._grid._table.dataKey] == dataKey) {
                                this._grid._table.selection = null;
                            }
                        }
                        if (this._grid._table.selectionMode == "multiple") {
                            this._grid._table.selection = this._grid._table.selection.filter(x => x[this._grid._table.dataKey] != dataKey);
                        }
                    }
                    this._grid._table.selectionChange.emit(this._grid._table.selection);
                    this._grid.trySaveState();
                    this.loadTable(this._grid._queryManager.getLazyLoadMetadata());
                })
                .catch(error => {
                    this.setPendingState(false);
                    return Promise.reject(error);
                });
        } else {
            const index = this._grid._table.dataKey
                ? this._grid._table.value.findIndex(value => value[this._grid._table.dataKey] == dataKey)
                : rowIndex;

            if (index == null) {
                return Promise.reject("Unable to find deleting row.");
            }

            const deletingRecord = this._grid._table.value[rowIndex];

            this._grid._table.value.splice(rowIndex, 1);
            this._grid._table.totalRecords--;

            if (this._grid._table.selectionMode == "single") {
                if (this._grid._table.selection == deletingRecord) {
                    this._grid._table.selection = null;
                }
            }
            if (this._grid._table.selectionMode == "multiple") {
                this._grid._table.selection = this._grid._table.selection.filter(x => x != deletingRecord);
            }
            this._grid._table.selectionChange.emit(this._grid._table.selection);
            this._grid.trySaveState();

            this._grid._cd.detectChanges();

            return Promise.resolve();
        }
    }

    deleteAll(): Promise<void> {
        if (this._grid._table?.lazy && this._grid.gridSettings.dataService) {
            this.setPendingState(true);
            return (this._grid.gridSettings.dataService[this._grid.gridSettings.dataServiceDeleteAllMethodName] as DataServiceDeleteAllFunc)()
                .then(() => {
                    if (this._grid._table.selectionMode != null) {
                        this._grid._table.selection = this._grid._table.selectionMode == "single" ? null : [];
                    }
                    this._grid._table.selectionChange.emit(this._grid._table.selection);
                    this._grid.trySaveState();
                    this.loadTable(this._grid._queryManager.getLazyLoadMetadata());
                })
                .catch(error => {
                    this.setPendingState(false);
                    return Promise.reject(error);
                });
        } else {
            this._grid._table.value = [];
            this._grid._table.totalRecords = 0;
            if (this._grid._table.selectionMode != null) {
                this._grid._table.selection = this._grid._table.selectionMode == "single" ? null : [];
            }
            this._grid._table.selectionChange.emit(this._grid._table.selection);
            this._grid.trySaveState();

            this._grid._cd.detectChanges();

            return Promise.resolve();
        }
    }

    getData(lazyLoadEvent: LazyLoadEvent): Promise<any[]> {
        if (!this._grid._table?.lazy || !this._grid.gridSettings.dataService) return null;

        if (this._grid.gridSettings.isODataGetRequest) {
            return this.requestDataViaOData(lazyLoadEvent).then(result => result.rows);
        } else {
            return this.requestData(
                new QueryCommand(
                    lazyLoadEvent,
                    this.getFilterDataTypesMap(Object.keys(lazyLoadEvent.filters))));
        }
    }

    async loadTable(lazyLoadEvent: TableLazyLoadEvent): Promise<void> {
        if (!this._grid._table?.lazy || !this._grid.gridSettings.dataService) return;

        if (this._grid.gridSettings.isODataGetRequest) {
            this.setPendingState(true);
            this.setTotalLoadingState(true);
            this.requestDataViaOData(lazyLoadEvent).then(result => {
                this._grid.setTableProperty("value", result.rows);
                this._grid.setTableProperty("totalRecords", result.total);
            }).finally(() => {
                this.setPendingState(false);
                this.setTotalLoadingState(false);
            });
        } else {
            let queryCommand = new QueryCommand(lazyLoadEvent, this.getFilterDataTypesMap(Object.keys(lazyLoadEvent.filters)));

            if (this._grid.gridSettings.queryCommandTransform) {
                queryCommand = this._grid.gridSettings.queryCommandTransform(queryCommand)
            }

            if (this._grid.gridSettings.beforeDataRequest) {
                this._grid.gridSettings.beforeDataRequest(queryCommand);
            }

            this.setPendingState(true);
            await this.requestData(queryCommand)
                .then(result => this._grid.setTableProperty("value", result))
                .finally(() => this.setPendingState(false));

            this.setTotalLoadingState(true);
            await this.requestTotal(queryCommand)
                .then(result => this._grid.setTableProperty("totalRecords", result))
                .finally(() => this.setTotalLoadingState(false));
        }
    }

    setPendingState(value: boolean): void {
        this._pending = value;
    }

    setTotalLoadingState(value: boolean): void {
        this._totalLoading = value;
    }

    update(dataKey: any, rowData: any, rowIndex?: number): Promise<any> {
        if (this._grid.gridSettings.dataService) {
            if (!this._grid.gridSettings.dataServiceUpdateMethodName || !this._grid.gridSettings.dataService[this._grid.gridSettings.dataServiceUpdateMethodName]) {
                return Promise.reject("Can not update the record. Wrong grid configuration.");
            }

            this.setPendingState(true);
            return (this._grid.gridSettings.dataService[this._grid.gridSettings.dataServiceUpdateMethodName] as DataServiceUpdateFunc)(dataKey, rowData)
                .then(response => {
                    this.loadTable(this._grid._queryManager.getLazyLoadMetadata());
                    return response;
                })
                .catch(error => {
                    this.setPendingState(false);
                    return Promise.reject(error);
                });
        } else {
            const index = this._grid._table.dataKey
                ? this._grid._table.value.findIndex(value => value[this._grid._table.dataKey] == rowData[this._grid._table.dataKey])
                : rowIndex;

            if (index == null) {
                return Promise.reject("Unable to find the edited row.");
            }

            Object.assign(this._grid._table.value[index], rowData);
            this._grid._cd.detectChanges();
            return Promise.resolve(this._grid._table.value[index]);
        }
    }


    private requestDataViaOData(lazyLoadEvent: TableLazyLoadEvent): Promise<{rows: any[], total: number}> {
        let oDataQuery = this._grid._queryManager.passLazyLoadMetadataToODataQuery(lazyLoadEvent);
        oDataQuery = this._grid._queryManager.passGridColumnsToODataSelectExpandQuery(oDataQuery);
        oDataQuery.count = true;
        if (this._grid.gridSettings.oDataQueryTransform) {
            oDataQuery = this._grid.gridSettings.oDataQueryTransform(oDataQuery);
        }

        return (this._grid.gridSettings.dataService[
            this._grid.gridSettings.dataServiceGetPageMethodName] as DataServiceODataGetFunc)(
            `${this._grid.gridSettings.oDataUrl}${buildQuery(oDataQuery)}`)
            .then(result => {
                return {
                    rows: this.handleLoadedData ? this.handleLoadedData(result) : result,
                    total: result["@odata.count"]
                };
            });
    }

    private requestData(queryCommand: IQueryCommand): Promise<any[]> {
        const dataResult = (this._grid.gridSettings.dataService[
            this._grid.gridSettings.dataServiceGetPageMethodName] as DataServiceGetFunc)(queryCommand);

        if (dataResult["then"] != null) {
            return (dataResult as Promise<any[]> | Promise<any>)
                .then((promiseResult: any[] | any) => this.handleLoadedData(promiseResult));
        } else {
            return Promise.resolve(this.handleLoadedData(dataResult));
        }
    }

    private requestTotal(queryCommand: IQueryCommand): Promise<number> {
        const totalResult = (this._grid.gridSettings.dataService[
            this._grid.gridSettings.dataServiceGetTotalMethodName] as DataServiceGetTotalFunc)(queryCommand);

        if (totalResult["then"] != null) {
            return (totalResult as Promise<number>);
        } else {
            return Promise.resolve(totalResult as number);
        }
    }

    private getFilterDataTypesMap(filtersKeys: string[]): {[key: string]: FilterType} {
        const result = {};

        if (this._grid.gridSettings.filtersRow) {
            filtersKeys.forEach(filterKey => {
                const column = <IGridColumn> this._grid._table.columns.find(x => x.field === filterKey);
                result[filterKey] = column?.filterSettings?.filterType;
            });
        } else {
            filtersKeys.forEach(filterKey => {
                const filter = <IFilterItem> this._grid._filterComponent?.filters[filterKey];
                result[filterKey] = filter?.settings.filterType;
            });
        }

        return result;
    }

    private handleLoadedData(data: any): any[] {
        if (this._grid.gridSettings.loadedDataHandler) {
            return this._grid.gridSettings.loadedDataHandler(data);
        } else {
            return data;
        }
    }
}