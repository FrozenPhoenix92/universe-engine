namespace Universe.Core.Exceptions
{
    /// <summary>
    /// Представляет класс для исключений, возникающих в период запуска приложения, когда обязательная переменная не задана.
    /// </summary>
    public sealed class StartupRequiredVariableIsEmptyException : StartupCriticalException
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Universe.Core.Exceptions.StartupRequiredVariableIsEmptyException"></see>
        /// с именем загружаемой сборки и именем типа незаданной переменной.
        /// </summary>
        /// <param name="asseblyName">Имя загружаемой сборки.</param>
        /// <param name="valiableTypeName">Тип незаданной переменной.</param>
        public StartupRequiredVariableIsEmptyException(string? asseblyName, string? valiableTypeName)
            : base($"Не задана обязательная переменная типа '{valiableTypeName}' в ходе инициализации модуля '{asseblyName}'.")
        {
        }
    }
}
