import { Injectable } from "@angular/core";

import { AccountBroadcastEvents, AccountService } from "../../../account";
import { CorePermissions } from "../../static-data";
import { EventBroadcastingService } from "../../../../utils";


@Injectable({ providedIn: "root" })
export class MenuService {
	private _menuModel: any[];


	constructor(private eventBroadcastingService: EventBroadcastingService, private accountService: AccountService) {
		this.eventBroadcastingService
			.on(AccountBroadcastEvents.IdentityChanged)
			.subscribe(() => this.loadMenu());
	}


	init(): Promise<void> {
		return this.loadMenu();
	}


	get menu(): any[] { return this._menuModel; }


	private loadMenu(): Promise<void> {
		this._menuModel = this.filterMenu(this.getDefaultMenu());
		return Promise.resolve();
	}


	private filterMenu(items: any[]): any[] {
		items.filter(x => x.items).forEach(x => x.items = this.filterMenu(x.items));
		return items.filter(x => (!x.items || x.items.length) && this.hasAccess(x));
	}

	private getDefaultMenu(): any[] {
		return [
			{
				label: "Main Menu",
				items: [
					{
						label: "Store Overview",
						icon: "pi pi-fw pi-file",
						routerLink: "/control-panel/store-overview"
					}
				]
			},
			{
				label: "Service Settings",
				items: [
					{
						label: "App Configuration",
						icon: "pi pi-fw pi-cog",
						routerLink: "/control-panel/app-configuration",
						permissions: CorePermissions.ManageAppConfiguration
					},
					{
						label: "Users",
						icon: "pi pi-fw pi-user",
						routerLink: "/control-panel/users",
						permissions: CorePermissions.ManageUser
					},
					{
						label: "Roles",
						icon: "pi pi-fw pi-sitemap",
						routerLink: "/control-panel/roles",
						permissions: CorePermissions.ManageRole
					},
					{
						label: "Permissions",
						icon: "pi pi-fw pi-table",
						routerLink: "/control-panel/permissions",
						permissions: CorePermissions.ManagePermission
					},
					{
						label: "Audit",
						icon: "pi pi-fw pi-pen-to-square",
						routerLink: "/control-panel/audit",
						permissions: CorePermissions.ReadAuditData
					}
				]
			}
		];
	}

	private hasAccess(menuItem: any): boolean {
		return !menuItem.roles?.length && !menuItem.permissions?.length ||
			this.accountService.matchRights(menuItem.roles, menuItem.permissions);
	}
}