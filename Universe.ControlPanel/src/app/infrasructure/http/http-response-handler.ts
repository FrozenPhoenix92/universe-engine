import { HttpErrorResponse, HttpStatusCode } from "@angular/common/http";

import { Message as PrimeNgMessage, MessageService } from "primeng/api";

import { HttpDefaults } from "./http-static";
import { Message } from "../../utils";


export interface IHttpResponseHandler<T = any> {
    resultHandler?: IHttpResultHandler<T>;
    settings?: IHttpResponseHandlerSettings | any;

    handleError?(errorResponse: HttpErrorResponse): void;
    handleSuccess?(data: T): void;
}

export interface IHttpResponseHandlerSettings {
    showSuccessMessage?: boolean; // Default false
    successMessage?: string;
    showErrorMessage?: boolean; // Default true
    errorMessage?: string;
    errorStatusesMessages?: {[key: number]: string};
}

export interface IHttpResultHandler<T = any> {
    handleError?(
        errorResponse: HttpErrorResponse,
        errorMessage: string,
        settings?: IHttpResponseHandlerSettings | any): void;
    handleSuccess?(
        data: T,
        successMessage: string,
        settings?: IHttpResponseHandlerSettings | any): void;
}


export class HttpResponseHandler<T = any> implements IHttpResponseHandler<T> {
    constructor(
        public settings?: IHttpResponseHandlerSettings | any,
        public resultHandler?: IHttpResultHandler) {
        if (!resultHandler) {
            this.resultHandler = HttpDefaults.ResultHandler();
        }
    }


    static getErrorMessageFromResponse(errorResponse: HttpErrorResponse): string {
        let errorMessage = errorResponse.message ?? HttpDefaults.ErrorBadRequestMessage;

        if (typeof errorResponse.error === "object") {
            // OData errors
            if (errorResponse.error.error && errorResponse.error.error.message) {
                errorMessage = errorResponse.error.error.message;
            }

            // Validation errors
            if (errorResponse.error?.errors) {
                errorMessage = Object
                    .keys(errorResponse.error.errors)
                    .map(key =>
                        `${key}: ${Array.isArray(errorResponse.error.errors[key]) ? (<string[]>errorResponse.error.errors[key]).join(", ") : errorResponse.error.errors[key]}`)
                    .join("\n");
            }
        } else {
            errorMessage = errorResponse.error;
        }

        return errorMessage;
    }

    handleError(errorResponse: HttpErrorResponse): void {
        if (this.settings && this.settings.showErrorMessage == false) return;

        if (this.settings && this.settings.errorStatusesMessages && this.settings.errorStatusesMessages[errorResponse.status] != null) {
            this.resultHandler.handleError(
                errorResponse,
                this.settings.errorStatusesMessages[errorResponse.status],
                this.settings);
            return;
        }

        this.handleErrorHttpStatusCode(errorResponse);
    }

    handleSuccess(data: T): void {
        if (this.settings && this.settings.showSuccessMessage) {
            this.resultHandler.handleSuccess(
                data,
                this.settings.successMessage
                    ? this.settings.successMessage
                    : HttpDefaults.SuccessMessage,
                this.settings);
        }
    }


    private handleErrorHttpStatusCode(errorResponse: HttpErrorResponse): void {
        switch (errorResponse.status) {
            case HttpStatusCode.BadRequest:
                this.handleStatus400BadRequest(errorResponse);
                break;
            case HttpStatusCode.Forbidden:
                this.resultHandler.handleError(errorResponse, HttpDefaults.ErrorForbiddenMessage, this.settings);
                break;
            default:
                this.handleOtherError(errorResponse);
        }
    }

    private handleOtherError(errorResponse: HttpErrorResponse): void {
        this.resultHandler.handleError(
            errorResponse,
            errorResponse.error ?? HttpDefaults.ErrorMessage,
            this.settings);
    }

    private handleStatus400BadRequest(errorResponse: HttpErrorResponse): void {
        this.resultHandler.handleError(
            errorResponse,
            HttpResponseHandler.getErrorMessageFromResponse(errorResponse),
            this.settings);
    }
}

export class PrimeNgMessageHttpResultHandler<T = any> implements IHttpResultHandler<T> {
    constructor(private messageService: MessageService) {}


    handleError(errorResponse: HttpErrorResponse, errorMessage: string): void {
        this.showMessage(Message.Error(errorMessage));
    }

    handleSuccess(data: T, successMessage: string): void {
        this.showMessage(Message.Success(successMessage));
    }


    private showMessage(message: PrimeNgMessage) {
        this.messageService.add(message);
    }
}

export class ConsoleHttpResultHandler<T = any> implements IHttpResultHandler<T> {
    handleError(errorResponse: HttpErrorResponse, errorMessage: string): void {
        console.error(errorMessage);
    }

    handleSuccess(data: T, successMessage: string): void {
        console.info(successMessage);
    }
}