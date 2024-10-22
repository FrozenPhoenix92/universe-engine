import * as _ from "lodash";

import { IHash } from "../infrasructure";


export function applyModifiedEntityChange<TEntity = any, TEntityKey = any>(entityChange: IEntityChange<TEntity, TEntityKey>,
                                                                           entity: TEntity): TEntity {
	Object.keys(entityChange.fieldChanges).forEach(x =>
		entity[_.camelCase(x)] = entityChange.fieldChanges[x] === "[[null]]" ? null : entityChange.fieldChanges[x]);
	return entity;
}

export function applyEntityChange<TEntity = any, TEntityKey = any>(entityChange: IEntityChange<TEntity, TEntityKey>,
                                                                   entities: TEntity[]): TEntity {
	switch (entityChange.changeType) {
		case EntityChangeType.Created:
			entities.push(entityChange.fullNewValue);
			return entityChange.fullNewValue;
		case EntityChangeType.Deleted:
			const pKDeletedFieldName = _.camelCase(entityChange.primaryKeyFieldName);
			if (!pKDeletedFieldName) return null;

			const index = entities.findIndex(x => x[pKDeletedFieldName] === entityChange.primaryKeyValue);
			if (index < 0) return null;

			const deletedEntity = entities[index];

			entities.splice(index, 1);

			return deletedEntity;
		case EntityChangeType.Modified:
			const pKModifiedFieldName = _.camelCase(entityChange.primaryKeyFieldName);
			if (!pKModifiedFieldName) return null;

			const changedEntity = entities.find(x => x[pKModifiedFieldName] === entityChange.primaryKeyValue);
			if (!changedEntity) return null;

			return applyModifiedEntityChange(entityChange, changedEntity);
	}

	return null;
}

export enum EntityChangeType {
	Created = 0,
	Deleted = 1,
	Modified = 2
}

export interface IEntityChange<TEntity = any, TEntityKey = any> {
	changeType: EntityChangeType;
	entityTypeName: string;
	fullNewValue?: TEntity;
	primaryKeyFieldName?: string;
	primaryKeyValue?: TEntityKey;
	fieldChanges?: IHash;
}