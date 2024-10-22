using System.Runtime.Serialization;

namespace Universe.Core.Data
{
	[Serializable]
	internal class MissedConfigurationSectionException : Exception
	{
		private object mainDatabaseConnectionSettings;

		public MissedConfigurationSectionException()
		{
		}

		public MissedConfigurationSectionException(object mainDatabaseConnectionSettings) => this.mainDatabaseConnectionSettings = mainDatabaseConnectionSettings;

		public MissedConfigurationSectionException(string? message) : base(message)
		{
		}

		public MissedConfigurationSectionException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected MissedConfigurationSectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}