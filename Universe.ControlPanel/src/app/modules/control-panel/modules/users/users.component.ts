import { Component, OnInit } from "@angular/core";
import { Validators } from "@angular/forms";

import { FilterMatchMode, SelectItem } from "primeng/api";

import { UserService } from "./user.service";
import {
	CellEditInputType,
	GridValidator,
	IGridColumn,
	IGridColumnFilterSettings,
	IGridSettings,
	ITableSettings
} from "@features/grid";
import { notEmptyValidator } from "../../../../validators";
import { IRole, RoleService } from "../roles";
import { IUser, IUserPermission, IUserRole } from "./user-models";
import { PermissionService } from "../permissions";
import { CoreRoles } from "../../static-data";


@Component({
	templateUrl: "./users.component.html"
})
export class UsersComponent implements OnInit {
	_gridSettings: IGridSettings;
	_tableSettings: ITableSettings;

	private _roles: IRole[];


	constructor(private userService: UserService,
	            private roleService: RoleService,
	            private permissionService: PermissionService) {
	}


	ngOnInit(): void {
		this._init().then();
	}


	private async _init(): Promise<void> {
		this._roles = await this.roleService.getAll();
		const permissionOptions = (await this.permissionService.getAll())
			.map(x => <SelectItem> { label: x.name, value: <IUserPermission> { permission: x, permissionId: x.id } });

		this._gridSettings = <IGridSettings> {
			filtersRow: true,
			deletingEnabled: false,
			dataService: this.userService,
			transformBeforeCreate: (rowData: IUser) => {
				if (!rowData.userRoles) {
					rowData.userRoles = [];
				}

				rowData.userRoles.forEach(x => {
					x.role = null;
				});

				if (!rowData.userPermissions) {
					rowData.userPermissions = [];
				}

				rowData.userPermissions.forEach(x => {
					x.permission = null;
				});

				return rowData;
			},
			transformBeforeUpdate: (rowData: IUser) => {
				rowData.userRoles.forEach(x => {
					x.userId = rowData.id;
					x.role = null;
				});

				rowData.userPermissions.forEach(x => {
					x.userId = rowData.id;
					x.permission = null;
				});

				return rowData;
			}
		};

		this._tableSettings = <ITableSettings> {
			columns: <IGridColumn[]> [
				{
					field: "email",
					header: "Email",
					validators: [
						new GridValidator(Validators.email, "email"),
						new GridValidator(notEmptyValidator(), "required")
					],
					filterSettings: <IGridColumnFilterSettings> {
						matchMode: FilterMatchMode.STARTS_WITH,
						matchModeSelectorVisible: false
					}
				},
				{
					field: "firstName",
					header: "Имя",
					validators: [ new GridValidator(notEmptyValidator(), "required") ],
					filterSettings: <IGridColumnFilterSettings> {
						matchMode: FilterMatchMode.STARTS_WITH,
						matchModeSelectorVisible: false
					}
				},
				{
					field: "lastName",
					header: "Фамилия",
					validators: [ new GridValidator(notEmptyValidator(), "required") ],
					filterSettings: <IGridColumnFilterSettings> {
						matchMode: FilterMatchMode.STARTS_WITH,
						matchModeSelectorVisible: false
					}
				},
				{
					field: "userRoles",
					header: "Роли",
					dropdownOptionsGenerator: (rowData: IUser) => this._roles.map(x => <SelectItem> {
						label: x.name,
						value: <IUserRole> { role: x, roleId: x.id },
						disabled: x.name === CoreRoles.SuperAdmin && rowData.rootAdmin
					}),
					dropdownOptionsDataKey: "roleId",
					cellEditingInputType: CellEditInputType.Multiselect,
					filterCellEnabled: false,
					displayHandler: (cellValue: IUserRole[]) => cellValue.map(x => x.role?.name).join(", ")
				},
				{
					field: "userPermissions",
					header: "Разрешения",
					dropdownOptions: permissionOptions,
					dropdownOptionsDataKey: "permissionId",
					cellEditingInputType: CellEditInputType.Multiselect,
					filterCellEnabled: false,
					displayHandler: (cellValue: IUserPermission[]) => cellValue.map(x => x.permission?.name).join(", ")
				}
			]
		};
	}
}