import { IEntity } from "../../../../infrasructure";


export interface IPermission extends IEntity<string> {
	id: string;
	name: string;
}