namespace Skyline.DataMiner.Solutions.Categories.API
{
	using System;
	using System.Linq;

	public class CategoriesObserver : IDisposable
	{
		private readonly object _lock = new();

		private RepositorySubscription<Scope> _subscriptionScopes;
		private RepositorySubscription<Category> _subscriptionCategories;
		private RepositorySubscription<CategoryItem> _subscriptionCategoryItems;

		/// <summary>
		/// Initializes a new instance of the <see cref="CategoriesObserver"/> class.
		/// This observer can be used to monitor changes in categories and scopes.
		/// It uses the provided API to subscribe to changes and updates the provided cache accordingly.
		/// It raises events when scopes or categories change.
		/// </summary>
		/// <param name="api">The categories API to use for subscriptions.</param>
		/// <param name="cache">The cache to update when changes occur.</param>
		public CategoriesObserver(ICategoriesApi api, CategoriesCache cache)
		{
			Api = api ?? throw new ArgumentNullException(nameof(api));
			Cache = cache ?? throw new ArgumentNullException(nameof(cache));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CategoriesObserver"/> class.
		/// This observer can be used to monitor changes in categories and scopes.
		/// It uses the provided API to subscribe to changes and updates the provided cache accordingly.
		/// It raises events when scopes or categories change.
		/// </summary>
		/// <param name="api">The categories API to use for subscriptions.</param>
		public CategoriesObserver(ICategoriesApi api)
			: this(api, new CategoriesCache())
		{
		}

		public event EventHandler<ApiObjectsChangedEvent<Scope>> ScopesChanged;

		public event EventHandler<ApiObjectsChangedEvent<Category>> CategoriesChanged;

		public event EventHandler<ApiObjectsChangedEvent<CategoryItem>> CategoryItemsChanged;

		internal ICategoriesApi Api { get; }

		public CategoriesCache Cache { get; }

		public bool IsSubscribed { get; private set; }

		public void Subscribe()
		{
			lock (_lock)
			{
				if (IsSubscribed)
				{
					return;
				}

				_subscriptionScopes = Api.Scopes.Subscribe();
				_subscriptionScopes.Changed += Scopes_Changed;

				_subscriptionCategories = Api.Categories.Subscribe();
				_subscriptionCategories.Changed += Categories_Changed;

				_subscriptionCategoryItems = Api.CategoryItems.Subscribe();
				_subscriptionCategoryItems.Changed += CategoryItems_Changed;

				IsSubscribed = true;
			}
		}

		public void Unsubscribe()
		{
			lock (_lock)
			{
				if (!IsSubscribed)
				{
					return;
				}

				_subscriptionScopes.Changed -= Scopes_Changed;
				_subscriptionScopes.Dispose();

				_subscriptionCategories.Changed -= Categories_Changed;
				_subscriptionCategories.Dispose();

				_subscriptionCategoryItems.Changed -= CategoryItems_Changed;
				_subscriptionCategoryItems.Dispose();

				IsSubscribed = false;
			}
		}

		public void LoadInitialData()
		{
			lock (_lock)
			{
				Cache.LoadInitialData(Api);
			}
		}

		private void Scopes_Changed(object sender, ApiObjectsChangedEvent<Scope> e)
		{
			lock (_lock)
			{
				Cache.UpdateScopes(e.Created.Concat(e.Updated), e.Deleted);
			}

			ScopesChanged?.Invoke(this, e);
		}

		private void Categories_Changed(object sender, ApiObjectsChangedEvent<Category> e)
		{
			lock (_lock)
			{
				Cache.UpdateCategories(e.Created.Concat(e.Updated), e.Deleted);
			}

			CategoriesChanged?.Invoke(this, e);
		}

		private void CategoryItems_Changed(object sender, ApiObjectsChangedEvent<CategoryItem> e)
		{
			lock (_lock)
			{
				Cache.UpdateCategoryItems(e.Created.Concat(e.Updated), e.Deleted);
			}

			CategoryItemsChanged?.Invoke(this, e);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			Unsubscribe();
		}
	}
}