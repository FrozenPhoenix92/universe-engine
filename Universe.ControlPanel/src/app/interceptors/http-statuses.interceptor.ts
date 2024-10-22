import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";

import { AppStorage } from "../utils";
import { AccountDefaults, AccountService, AccountStorageKeys } from "../modules/account";


@Injectable()
export class HttpStatusesInterceptor implements HttpInterceptor {
    constructor(private accountService: AccountService) {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError((err: HttpErrorResponse) => {
                if (err.status == 401) {
                    if (this.accountService) {
                        AppStorage.setItemForSession(
                            AccountStorageKeys.LogoutReasonMessage,
                            AccountDefaults.SessionExpiredMessage
                        );
                        AppStorage.setItemForSession(AccountStorageKeys.LastVisitedPageUrl, location.pathname);

                        if (this.accountService.isAuthenticated) {
                            this.accountService.signOutLocally();
                        }
                    }
                }

                return throwError(() => err);
            })
        );
    }
}
