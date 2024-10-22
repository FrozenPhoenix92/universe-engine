import { Component } from "@angular/core";

import {
    DisplayMode,
    GridColumnViewSettings,
    IGridColumn,
    IGridSettings,
    ITableSettings,
    SortOrder
} from "@features/grid";
import { FilterInputType, FilterType } from "@features/filter";
import { SignInAuditService } from "../services";


@Component({
    selector: "sign-in-audit",
    templateUrl: "./sign-in-audit.component.html"
})
export class SignInAuditComponent {
    tableSettings: ITableSettings = {
        columns: <IGridColumn[]>[
            {
                field: "ip", header: "IP адрес",
                viewSettings: new GridColumnViewSettings({ width: "200px" })
            },
            {
                field: "datetime",
                header: "Время",
                displayMode: DisplayMode.Date,
                displayDateMomentFormat: "L LTS",
                filterSettings: {
                    filterType: FilterType.Date,
                    inputType: FilterInputType.Calendar
                }
            },
            { field: "email", header: "Email" },
            { field: "browser", header: "Браузер" },
            {
                field: "result",
                header: "Результат",
                filterCellEnabled: false
            },
            { field: "fingerprint", header: "Finger Print" }
        ],
        autoLayout: true,
        sortField: "datetime",
        sortOrder: SortOrder.Desc
    };
    gridSettings: IGridSettings = {
        readonly: true,
        exportEnabled: true,
        filtersRow: true
    };

    constructor(auditService: SignInAuditService) {
        this.gridSettings.dataService = auditService;
    }
}