import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { CrudService } from "../../../../infrasructure/http";
import { IRole } from "./role-models";
import { environment } from "../../../../../environments/environment";


@Injectable({ providedIn: "root" })
export class RoleService extends CrudService<IRole> {
    protected readonly urlPrefix = `${environment.apiUrl}role`;

    constructor(httpClient: HttpClient) {
        super(httpClient);
    }
}