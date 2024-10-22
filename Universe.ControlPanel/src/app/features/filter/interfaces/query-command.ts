import { IFilterInfoBase } from "./filter-info-base";

export interface IQueryCommand {
    expandedFields?: string[];
    skip?: number;
    take?: number;
    sortingDirection?: number;
    sortingField?: string;
    filters?: IFilterInfoBase[];
}
