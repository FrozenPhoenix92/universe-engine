import { HttpClient } from "@angular/common/http";

import {
    HttpResponseHandler,
    IHttpResponseHandler,
    IHttpResponseHandlerSettings,
    IHttpResultHandler
} from "./http-response-handler";
import { setLocalTimeZone, setUtcTimeZone } from "../../utils";
import { HttpServiceBase } from "./http-service-base";
import { HttpDefaults } from "./http-static";
import { IDateFilter, IQueryCommand } from "@features/filter";


export abstract class CrudService<TEntity> extends HttpServiceBase {
    protected readonly abstract urlPrefix: string;


    protected constructor(http: HttpClient) {
        super(http);
    }


    /* Поля, которые в независимости от часового пояса всегда должны иметь одно значение. */
    protected get modelUtcDateFields(): string[] {
        return []; 
    }


    extendQueryCommand(queryCommand: IQueryCommand): void {
    }


    get(
        id: number | string,
        responseHandlerSettings?: IHttpResponseHandlerSettings | any,
        resultHandler?: IHttpResultHandler<TEntity>,
        responseHandler?: IHttpResponseHandler<TEntity>): Promise<TEntity> {
        const rhs = responseHandlerSettings ?? <IHttpResponseHandlerSettings>{};
        if (!rhs.errorStatusesMessages) rhs.errorStatusesMessages = {};
        if (!rhs.errorStatusesMessages[404]) rhs.errorStatusesMessages[404] = HttpDefaults.ErrorNotFoundMessage;

        return this.handleRequest<TEntity>(
            this.http.get<TEntity>(`${this.urlPrefix}/${id}`),
            this.setResponseHandler(rhs, resultHandler, responseHandler)
        ).then(result => {
            if (result) {
                this.modelUtcDateFields.forEach(utcField =>
                    result[utcField] = setLocalTimeZone(result[utcField]));
            }
            return result;
        });
    }

    getAll(
        queryCommand?: IQueryCommand,
        responseHandlerSettings?: IHttpResponseHandlerSettings | any,
        resultHandler?: IHttpResultHandler<TEntity[]>,
        responseHandler?: IHttpResponseHandler<TEntity[]>): Promise<TEntity[]> {
        if (this.extendQueryCommand) {
            this.extendQueryCommand(queryCommand);
        }

        if (queryCommand) {
            this.prepareQueryCommandDates(queryCommand);
        }

        return this.handleRequest<TEntity[]>(
            this.http.get<TEntity[]>(`${this.urlPrefix}`, { params: this.constructHttpParams(queryCommand) }),
            this.setResponseHandler(responseHandlerSettings, resultHandler, responseHandler)
        ).then(result => {
            if (result) {
                result.forEach(item =>
                    this.modelUtcDateFields.forEach(utcField => item[utcField] = setLocalTimeZone(item[utcField])));
            }
            return result;
        });
    }

    getTotal(
        queryCommand?: IQueryCommand,
        responseHandlerSettings?: IHttpResponseHandlerSettings | any,
        resultHandler?: IHttpResultHandler<number>,
        responseHandler?: IHttpResponseHandler<number>): Promise<number> {
        if (this.extendQueryCommand) {
            this.extendQueryCommand(queryCommand);
        }

        if (queryCommand) {
            this.prepareQueryCommandDates(queryCommand);
        }

        return this.handleRequest<number>(
            this.http.get<number>(`${this.urlPrefix}/total`, { params: this.constructHttpParams(queryCommand) }),
            this.setResponseHandler(responseHandlerSettings, resultHandler, responseHandler)
        );
    }

    create(
        item: TEntity,
        responseHandlerSettings?: IHttpResponseHandlerSettings | any,
        resultHandler?: IHttpResultHandler<TEntity>,
        responseHandler?: IHttpResponseHandler<TEntity>): Promise<TEntity> {
        this.modelUtcDateFields.forEach(utcField => item[utcField] = setUtcTimeZone(item[utcField]));

        const rhs = responseHandlerSettings ?? <IHttpResponseHandlerSettings>{};
        if (rhs.showSuccessMessage == null) rhs.showSuccessMessage = true;
        if (!rhs.successMessage) rhs.successMessage = HttpDefaults.SuccessCreatedMessage;

        return this.handleRequest<TEntity>(
            this.http.post<TEntity>(`${this.urlPrefix}`, item),
            this.setResponseHandler(rhs, resultHandler, responseHandler)
        );
    }

    update(
        id: number | string, item: TEntity,
        responseHandlerSettings?: IHttpResponseHandlerSettings | any,
        resultHandler?: IHttpResultHandler<TEntity>,
        responseHandler?: IHttpResponseHandler<TEntity>): Promise<TEntity> {
        this.modelUtcDateFields.forEach(utcField => item[utcField] = setUtcTimeZone(item[utcField]));

        const rhs = responseHandlerSettings ?? <IHttpResponseHandlerSettings>{};
        if (rhs.showSuccessMessage == null) rhs.showSuccessMessage = true;
        if (!rhs.successMessage) rhs.successMessage = HttpDefaults.SuccessUpdatedMessage;

        return this.handleRequest<TEntity>(
            this.http.put<TEntity>(`${this.urlPrefix}/${id}`, item),
            this.setResponseHandler(rhs, resultHandler, responseHandler)
        );
    }

    delete(
        id: number | string,
        responseHandlerSettings?: IHttpResponseHandlerSettings | any,
        resultHandler?: IHttpResultHandler<TEntity>,
        responseHandler?: IHttpResponseHandler<TEntity>): Promise<any> {
        const rhs = responseHandlerSettings ?? <IHttpResponseHandlerSettings>{};
        if (rhs.showSuccessMessage == null) rhs.showSuccessMessage = true;
        if (!rhs.successMessage) rhs.successMessage = HttpDefaults.SuccessDeletedMessage;

        return this.handleRequest<any>(
            this.http.delete<any>(`${this.urlPrefix}/${id}`),
            this.setResponseHandler(rhs, resultHandler, responseHandler)
        );
    }

    deleteAll(
        responseHandlerSettings?: IHttpResponseHandlerSettings | any,
        resultHandler?: IHttpResultHandler<TEntity>,
        responseHandler?: IHttpResponseHandler<TEntity>): Promise<any> {
        const rhs = responseHandlerSettings ?? <IHttpResponseHandlerSettings>{};
        if (rhs.showSuccessMessage == null) rhs.showSuccessMessage = true;
        if (!rhs.successMessage) rhs.successMessage = HttpDefaults.SuccessDeletedAllMessage;

        return this.handleRequest<any>(
            this.http.delete<any>(`${this.urlPrefix}`),
            this.setResponseHandler(rhs, resultHandler, responseHandler)
        );
    }


    private prepareQueryCommandDates(queryCommand: IQueryCommand): void {
        this.modelUtcDateFields.forEach(utcField => {
            const filter = queryCommand.filters.find(filterItem => filterItem.propertyName == utcField) as IDateFilter;
            if (filter && filter.value) {
                filter.value = setUtcTimeZone(filter.value);
            }
        });
    }

    private setResponseHandler<T>(
        responseHandlerSettings?: IHttpResponseHandlerSettings | any,
        resultHandler?: IHttpResultHandler<T>,
        responseHandler?: IHttpResponseHandler<T>): IHttpResponseHandler<T> {
        const rh = responseHandler ?? new HttpResponseHandler();

        if (resultHandler) {
            rh.resultHandler = resultHandler;
        }
        if (!rh.resultHandler) {
            rh.resultHandler = HttpDefaults.ResultHandler();
        }

        if (!rh.settings) {
            rh.settings = responseHandlerSettings;
        }

        return rh;
    }
}