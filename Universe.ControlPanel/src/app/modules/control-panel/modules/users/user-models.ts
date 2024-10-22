import { IEntity } from "../../../../infrasructure";
import { IRole } from "../roles";
import { IPermission } from "../permissions";


export interface IUser extends IEntity<string> {
	email: string;
	firstName: string;
	id: string;
	lastName: string;
	password?: string;
	phoneNumber: string;
	rootAdmin: boolean;
	userName: string;
	userPermissions: IUserPermission[];
	userRoles: IUserRole[]
}

export interface IUserRole {
	userId: string;
	user: IUser;
	roleId: string;
	role: IRole;
}

export interface IUserPermission {
	userId: string;
	user: IUser;
	permissionId: string;
	permission: IPermission;
}