import { IEntity } from "../../../../infrasructure";


export interface IRole extends IEntity<string> {
	id: string;
	name: string;
}