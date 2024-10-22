import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { IQueryCommand } from "@features/filter";
import { HttpServiceBase } from "../../../../../infrasructure/http";
import { environment } from "../../../../../../environments/environment";
import { IDataAudit } from "../audit-models";



@Injectable()
export class DataAuditService extends HttpServiceBase {
    protected readonly urlPrefix = `${environment.apiUrl}data-audit`;


    constructor(http: HttpClient) {
        super(http);
    }


    getAll(queryCommand?: IQueryCommand): Promise<IDataAudit[]> {
        return this.handleRequest<IDataAudit[]>(
            this.http.get<IDataAudit[]>(`${this.urlPrefix}`, { params: this.constructHttpParams(queryCommand) })
        );
    }

    getTotal(queryCommand?: IQueryCommand): Promise<number> {
        return this.handleRequest<number>(
            this.http.get<number>(`${this.urlPrefix}/total`, { params: this.constructHttpParams(queryCommand) })
        );
    }
}