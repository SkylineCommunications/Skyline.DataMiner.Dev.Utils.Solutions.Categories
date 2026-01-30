namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;

	public interface IRepository<T> : IReadableRepository<T>, ICreatableRepository<T>, IBulkDeletableRepository<T>, IUpdatableRepository<T>
		where T : ApiObject<T>
	{
		RepositorySubscription<T> Subscribe();

		IEnumerable<T> ReadAll();

		DomHelper Helper { get; } // TODO: Remove from public API

		IConnection Connection { get; } // TODO: Remove from public API

		T CreateInstance(DomInstance domInstance); // TODO: Remove from public API

		IEnumerable<T> CreateOrUpdate(IEnumerable<T> instances);

		IQueryable<T> Query();

		RepositorySubscription<T> Subscribe(FilterElement<T> filter);
	}
}
