export interface IBroadcastEvent {
	key: string;
	data?: any;
}

export interface ICollectionHandleResult {
	failed: IHash<string>;
	succeed: string[];
}

export interface IEntity<TKey = number> {
	id: TKey;
}

export interface IHash<T = any> {
	[key: string]: T;
}

export interface IFileDetails extends IEntity {
	created: Date;
	extension: string;
	latestUpdated: Date;
	name: string;
	relatedDataKey: string;
	relatedOperationKey: string;
	relatedResponsibleKey: string;
}