import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { IQueryCommand } from "@features/filter";
import { HttpServiceBase } from "../../../../../infrasructure/http";
import { environment } from "../../../../../../environments/environment";
import { ISignInAudit } from "../audit-models";


@Injectable()
export class SignInAuditService extends HttpServiceBase {
    protected readonly urlPrefix = `${environment.apiUrl}sign-in-audit`;


    constructor(http: HttpClient) {
        super(http);
    }


    getAll(queryCommand?: IQueryCommand): Promise<ISignInAudit[]> {
        return this.handleRequest<ISignInAudit[]>(
            this.http.get<ISignInAudit[]>(`${this.urlPrefix}`, { params: this.constructHttpParams(queryCommand) })
        );
    }

    getTotal(queryCommand?: IQueryCommand): Promise<number> {
        return this.handleRequest<number>(
            this.http.get<number>(`${this.urlPrefix}/total`, { params: this.constructHttpParams(queryCommand) })
        );
    }
}