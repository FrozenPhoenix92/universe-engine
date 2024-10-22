import { Injectable } from "@angular/core";

import { Subject, Observable } from "rxjs";
import { filter, map } from "rxjs/operators";

import { IBroadcastEvent } from "../infrasructure";


@Injectable({ providedIn: "root" })
export class EventBroadcastingService {
	private _eventBus = new Subject<IBroadcastEvent>();


	broadcast(key: string, data?: any) {
		this._eventBus.next(<IBroadcastEvent> { key, data });
	}

	on<T = any>(key: any): Observable<T> {
		return this._eventBus.asObservable().pipe(
			filter(event => event.key === key),
			map(event => <T>event.data));
	}
}