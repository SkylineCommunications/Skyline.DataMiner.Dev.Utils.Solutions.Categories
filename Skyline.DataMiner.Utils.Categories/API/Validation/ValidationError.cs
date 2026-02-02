namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System;

	public class ValidationError
	{
		public ValidationError(string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));
			}

			Message = message;
		}

		public ValidationError(string message, object instance, string propertyName)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));
			}

			if (instance is null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or whitespace.", nameof(propertyName));
			}

			Message = message;
			Instance = instance;
			PropertyName = propertyName;
		}

		public string Message { get; }

		public object Instance { get; }

		public string PropertyName { get; }
	}
}
