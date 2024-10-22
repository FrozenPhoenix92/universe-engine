import { Message as PrimeNgMessage } from "primeng/api";

export class Message implements PrimeNgMessage {
	constructor(public severity: string, public summary: string, public detail: string) {}


	static Error(detail: string, summary?: string): PrimeNgMessage {
		return new Message("error", summary || PrimeNgDefaults.MessageErrorSummary, detail);
	}

	static Info(detail: string, summary?: string): PrimeNgMessage {
		return new Message("info", summary || PrimeNgDefaults.MessageInfoSummary, detail);
	}

	static Success(detail: string, summary?: string): PrimeNgMessage {
		return new Message("success", summary || PrimeNgDefaults.MessageSuccessSummary, detail);
	}

	static Warning(detail: string, summary?: string): PrimeNgMessage {
		return new Message("warn", summary || PrimeNgDefaults.MessageWarningSummary, detail);
	}
}

export class PrimeNgDefaults {
	static MessageErrorSummary = "Ошибка";
	static MessageInfoSummary = "Уведомление";
	static MessageSuccessSummary = "Выполнено";
	static MessageWarningSummary = "Внимание";
}