import { IEntity } from "../../../../infrasructure";


export interface IDataAudit extends IEntity {
	dateTime: Date;
	stateString: string;
	entityName: string;
	tableName: string;
	userName: string;
	entityId: string;
}

export interface ISignInAudit extends IEntity {
	dateTime: string;
	ip: string;
	location: string;
	fingerprint: string;
	email: string;
	browserInformation: string;
	loginResult: string;
}