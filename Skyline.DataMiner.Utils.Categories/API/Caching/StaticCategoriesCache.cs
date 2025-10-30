namespace Skyline.DataMiner.Utils.Categories.API.Caching
{
	using System;
	using System.Threading;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Utils.Categories.API.Subscriptions;
	using Skyline.DataMiner.Utils.Categories.Tools;

	public sealed class StaticCategoriesCache : IDisposable
	{
		private static readonly object _lock = new();
		private static volatile StaticCategoriesCache _instance;

		private bool _disposed;

		public StaticCategoriesCache(IConnection connection)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));

			Api = new CategoriesApi(Connection);
			Observer = CreateObserver();
		}

		private IConnection Connection { get; }

		private CategoriesApi Api { get; }

		private CategoriesObserver Observer { get; }

		public CategoriesCache Cache => Observer.Cache;

		public static StaticCategoriesCache GetOrCreate(Func<IConnection> connectionFactory)
		{
			if (connectionFactory == null)
			{
				throw new ArgumentNullException(nameof(connectionFactory));
			}

			if (_instance == null)
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						var connection = connectionFactory();
						_instance = GetOrCreate(connection);
					}
				}
			}

			return _instance;
		}

		public static StaticCategoriesCache GetOrCreate(IConnection baseConnection)
		{
			if (baseConnection is null)
			{
				throw new ArgumentNullException(nameof(baseConnection));
			}

			if (_instance == null)
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						// Always clone the connection to ensure that the cache has its own dedicated connection.
						// This prevents potential conflicts when the base connection would be closed or unsubscribed elsewhere.
						var connection = CloneConnection(baseConnection);

						_instance = new StaticCategoriesCache(connection);
					}
				}
			}

			return _instance;
		}

		public static StaticCategoriesCache Get()
		{
			lock (_lock)
			{
				if (_instance == null)
				{
					throw new InvalidOperationException($"The instance has not been created yet. Please call {nameof(GetOrCreate)} first.");
				}

				return _instance;
			}
		}

		/// <summary>
		/// Resets the singleton instance, disposing of the existing instance if necessary.
		/// For testing purposes only.
		/// </summary>
		public static void Reset()
		{
			// Replace the instance with null in a thread-safe manner
			var oldInstance = Interlocked.Exchange(ref _instance, null);

			// Dispose the old instance if it exists
			oldInstance?.Dispose();
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			Observer?.Dispose();

			_disposed = true;
		}

		private CategoriesObserver CreateObserver()
		{
			var observer = new CategoriesObserver(Api);

			observer.Subscribe();
			observer.LoadInitialData();

			return observer;
		}

		private static IConnection CloneConnection(IConnection baseConnection)
		{
			if (baseConnection.GetType().FullName.Contains("SLNetConnectionMock"))
			{
				// If the connection is a mock connection used for unit testing, use the existing connection directly.
				// Such connection cannot be cloned.
				return baseConnection;
			}

			if (ConnectionHelper.IsManagedDataMinerModule(baseConnection))
			{
				// If the connection is a managed DataMiner module (e.g. Engine.SLNetRaw), use the existing connection directly.
				return baseConnection;
			}

			if (ConnectionHelper.TryCloneConnection(baseConnection, "Categories - Connection", out var clonedConnection))
			{
				return clonedConnection;
			}

			throw new InvalidOperationException("Failed to clone the provided connection.");
		}
	}
}
