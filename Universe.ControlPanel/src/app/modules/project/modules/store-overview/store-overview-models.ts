import { IEntity } from "../../../../infrasructure";

import { SelectItem } from "primeng/api";


export enum OrderStatus {
	Created = "created",
	InProgress = "inProgress",
	Completed = "completed",
	Canceled = "canceled"
}
export function getOrderStatusDisplayingName(value: OrderStatus): string {
	switch (value) {
		case OrderStatus.Created: return "Created";
		case OrderStatus.InProgress: return "In Progress";
		case OrderStatus.Completed: return "Completed";
		case OrderStatus.Canceled: return "Canceled";
	}

	throw new Error("The specified order status doesn't exist.");
}
export function getOrderStatusOptions(): SelectItem[] {
	return <SelectItem[]> [
		{ label: getOrderStatusDisplayingName(OrderStatus.Created), value: OrderStatus.Created },
		{ label: getOrderStatusDisplayingName(OrderStatus.InProgress), value: OrderStatus.InProgress },
		{ label: getOrderStatusDisplayingName(OrderStatus.Completed), value: OrderStatus.Completed },
		{ label: getOrderStatusDisplayingName(OrderStatus.Canceled), value: OrderStatus.Canceled },
	];
}


export interface ICustomer extends IEntity {
	address: string;
	lastName: string;
	name: string;
	photo?: string;
}

export interface IOrder extends IEntity {
	created: Date;
	customer: ICustomer;
	customerId: string;
	lastModified: Date;
	orderLines: IOrderLine[];
	status: OrderStatus;
}

export interface IOrderLine extends IEntity {
	count: number;
	order: IOrder;
	orderId: number;
	price: number;
	product: IProduct;
	productId: number;
}

export interface IProduct extends IEntity {
	name: string;
	price: number;
}