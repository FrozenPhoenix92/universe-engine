import { APP_INITIALIZER, CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { RouterModule } from "@angular/router";
import { HTTP_INTERCEPTORS, HttpClientModule } from "@angular/common/http";
import { CommonModule } from "@angular/common";

import { ConfirmationService, MessageService } from "primeng/api";
import { ToastModule } from "primeng/toast";
import { MessageModule } from "primeng/message";
import { ConfirmDialogModule } from "primeng/confirmdialog";

import { AppComponent } from './app.component';
import { DatesConversionInterceptor, HttpStatusesInterceptor } from "./interceptors";
import { ProjectDefaults } from "./modules/project";
import { AccountService } from "./modules/account";
import { AppConfigurationService } from "./modules/control-panel/modules/app-configuration";
import { MenuService } from "./modules/control-panel/modules/menu";
import { UserService } from "./modules/control-panel/modules/users";


@NgModule({
	declarations: [ AppComponent ],
	imports: [
		CommonModule,
		BrowserModule,
		BrowserAnimationsModule,
		HttpClientModule,

		ToastModule,
		ConfirmDialogModule,
		MessageModule,

		RouterModule.forRoot([
			{
				path: "",
				loadChildren: () => import("./modules/account/account.module").then(m => m.AccountModule),
			},
			{
				path: "control-panel",
				loadChildren: () => import("./modules/control-panel/control-panel.module").then(m => m.ControlPanelModule),
			}
		])
	],
	providers: [
		{ provide: HTTP_INTERCEPTORS, useClass: HttpStatusesInterceptor, multi: true },
		{ provide: HTTP_INTERCEPTORS, useClass: DatesConversionInterceptor, multi: true },
		{
			provide: APP_INITIALIZER,
			useFactory: (messageService: MessageService) => () => ProjectDefaults.Init(messageService),
			deps: [ MessageService ],
			multi: true
		},
		{
			provide: APP_INITIALIZER,
			useFactory: (appConfigurationService: AppConfigurationService) => () => appConfigurationService.init(),
			deps: [ AppConfigurationService ],
			multi: true
		},
		{
			provide: APP_INITIALIZER,
			useFactory: (userService: UserService) => () => userService.init(),
			deps: [ UserService ],
			multi: true
		},
		{
			provide: APP_INITIALIZER,
			useFactory: (accountService: AccountService) => () => accountService.init(),
			deps: [ AccountService ],
			multi: true
		},
		{
			provide: APP_INITIALIZER,
			useFactory: (menuService: MenuService) => () => menuService.init(),
			deps: [ MenuService ],
			multi: true
		},

		MessageService,
		ConfirmationService,
	],
	bootstrap: [ AppComponent ],
	schemas: [ CUSTOM_ELEMENTS_SCHEMA ]
})
export class AppModule {}