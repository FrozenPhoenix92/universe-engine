export function truncate(value: number, decimalPlaces: number): number {
	const regExp = new RegExp("(\\d+\\.\\d{" + decimalPlaces + "})(\\d)"),
		m = value.toString().match(regExp);
	return m ? parseFloat(m[1]) : value;
}