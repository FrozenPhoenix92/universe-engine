import { ConsoleHttpResultHandler, IHttpResultHandler } from "./http-response-handler";

export class HttpDefaults {
	static ErrorBadRequestMessage = "Операция некорректна.";
	static ErrorForbiddenMessage = "Операция успешно выполнена.";
	static ErrorMessage = "Операция завершилась ошибкой.";
	static ErrorNotFoundMessage = "Запрашиваемый объект не найден.";
	static SuccessCreatedMessage = "Данные успешно добавлены.";
	static SuccessDeletedMessage = "Удаление прошло успешно.";
	static SuccessDeletedAllMessage = "Список очищен.";
	static SuccessMessage = "Операция успешно выполнена.";
	static SuccessUpdatedMessage = "Изменения сохранены.";

	static ResultHandler = <T = any>(): IHttpResultHandler<T> => {
		return new ConsoleHttpResultHandler<T>();
	}
}