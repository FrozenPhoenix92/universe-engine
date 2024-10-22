export function setUtcTimeZone(date: Date): Date {
	return date ? new Date(date.getTime() - date.getTimezoneOffset() * 60000) : date;
}

export function setLocalTimeZone(date: Date): Date {
	return date ? new Date(date.getTime() + date.getTimezoneOffset() * 60000) : date;
}