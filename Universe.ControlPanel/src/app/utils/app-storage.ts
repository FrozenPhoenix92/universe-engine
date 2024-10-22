import { environment } from "../../environments/environment";

/**
 * Use this class for storing application data instead of direct localStorage or sessionStorage.
 * */
export class AppStorage {
    /**
     * Uses a "localStorage" to store a data or delete item if "null" specified.
     * @param key The ksy of stored object.
     * @param item Stored value.
     * */
    static setItem(key: string, item: any): void {
        if (item == null) {
            localStorage.removeItem(`${environment.applicationName}.${key}`);
        } else {
            localStorage.setItem(`${environment.applicationName}.${key}`, JSON.stringify(item));
        }
    }

    /**
     * Uses a "sessionStorage" to store a data or delete item if "null" specified.
     * @param key The ksy of stored object.
     * @param item Stored value.
     * */
    static setItemForSession(key: string, item: any): void {
        if (item == null) {
            sessionStorage.removeItem(key);
        } else {
            sessionStorage.setItem(key, JSON.stringify(item));
        }
    }

    /**
     * Uses a "localStorage" to get a stored data or "null" if nothing found or fetched data could not be parsed.
     * @param key The ksy of stored object.
     * */
    static getItem<T = string>(key: string): T | null {
        const storedString = localStorage.getItem(`${environment.applicationName}.${key}`);

        if (storedString == null) return null;

        try {
            return JSON.parse(storedString) as T;
        } catch (error) {
            return null;
        }
    }

    /**
     * Uses a "sessionStorage" to get a stored data or "null" if nothing found or fetched data could not be parsed.
     * @param key The ksy of stored object.
     * */
    static getItemFromSession<T>(key: string): T | null {
        const storedSessionString = sessionStorage.getItem(key);

        if (storedSessionString == null) return null;

        try {
            return JSON.parse(storedSessionString) as T;
        } catch (error) {
            return null;
        }
    }
}