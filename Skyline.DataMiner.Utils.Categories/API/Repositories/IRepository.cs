namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;

	public interface IRepository<T> : IReadableRepository<T>, ICreatableRepository<T>, IBulkDeletableRepository<T>, IUpdatableRepository<T>
		where T : ApiObject<T>
	{
		IConnection Connection { get; }

		IEnumerable<T> CreateOrUpdate(IEnumerable<T> instances);

		long Count(FilterElement<T> filter);

		long CountAll();

		IQueryable<T> Query();

		T Read(Guid id);

		T Read(string name);

		IDictionary<string, T> Read(IEnumerable<string> names);

		IEnumerable<T> ReadAll();

		RepositorySubscription<T> Subscribe();

		RepositorySubscription<T> Subscribe(FilterElement<T> filter);
	}
}