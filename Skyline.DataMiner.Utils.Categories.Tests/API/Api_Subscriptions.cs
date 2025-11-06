namespace Skyline.DataMiner.Utils.Categories.Tests.API
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.Categories.API.Objects;
	using Skyline.DataMiner.Utils.Categories.API.Subscriptions;

	[TestClass]
	public sealed class Api_Subscriptions : TestBase
	{
		[TestMethod]
		public void Api_Subscriptions_CreateWithFilter()
		{
			// Arrange
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.Create(scope);

			var categoryX = new Category { Name = "Category X", Scope = scope };
			var categoryY = new Category { Name = "Category Y", Scope = scope };

			var receivedEvents = new List<ApiObjectsChangedEvent<Category>>();

			var filter = CategoryExposers.Name.Equal(categoryX.Name);
			using var subscription = api.Categories.Subscribe(filter);
			subscription.Changed += (s, e) => receivedEvents.Add(e);

			// Act
			api.Categories.Create(categoryX); // matches filter
			api.Categories.Create(categoryY); // does not match filter

			// Assert
			Assert.HasCount(1, receivedEvents);

			var receivedEvent = receivedEvents[0];
			CollectionAssert.AreEquivalent(new[] { categoryX }, receivedEvent.Created.ToArray());
			CollectionAssert.AreEquivalent(Array.Empty<Category>(), receivedEvent.Updated.ToArray());
			CollectionAssert.AreEquivalent(Array.Empty<Category>(), receivedEvent.Deleted.ToArray());
		}

		[TestMethod]
		public void Api_Subscriptions_CreateWithoutFilter()
		{
			// Arrange
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.Create(scope);

			var categoryX = new Category { Name = "Category X", Scope = scope };
			var categoryY = new Category { Name = "Category Y", Scope = scope };

			var receivedEvents = new List<ApiObjectsChangedEvent<Category>>();

			using var subscription = api.Categories.Subscribe();
			subscription.Changed += (s, e) => receivedEvents.Add(e);

			// Act
			api.Categories.Create(categoryX);
			api.Categories.Create(categoryY);

			// Assert
			Assert.HasCount(2, receivedEvents);

			var receivedEvent1 = receivedEvents[0];
			CollectionAssert.AreEquivalent(new[] { categoryX }, receivedEvent1.Created.ToArray());
			CollectionAssert.AreEquivalent(Array.Empty<Category>(), receivedEvent1.Updated.ToArray());
			CollectionAssert.AreEquivalent(Array.Empty<Category>(), receivedEvent1.Deleted.ToArray());

			var receivedEvent2 = receivedEvents[1];
			CollectionAssert.AreEquivalent(new[] { categoryY }, receivedEvent2.Created.ToArray());
			CollectionAssert.AreEquivalent(Array.Empty<Category>(), receivedEvent2.Updated.ToArray());
			CollectionAssert.AreEquivalent(Array.Empty<Category>(), receivedEvent2.Deleted.ToArray());
		}

		[TestMethod]
		public void Api_Subscriptions_Update()
		{
			// Arrange
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.Create(scope);

			var category = new Category { Name = "Category X", Scope = scope };
			api.Categories.Create(category);

			var receivedEvents = new List<ApiObjectsChangedEvent<Category>>();

			using var subscription = api.Categories.Subscribe();
			subscription.Changed += (s, e) => receivedEvents.Add(e);

			// Act
			api.Categories.Update(category);

			// Assert
			Assert.HasCount(1, receivedEvents);

			var receivedEvent = receivedEvents[0];
			CollectionAssert.AreEquivalent(Array.Empty<Category>(), receivedEvent.Created.ToArray());
			CollectionAssert.AreEquivalent(new[] { category }, receivedEvent.Updated.ToArray());
			CollectionAssert.AreEquivalent(Array.Empty<Category>(), receivedEvent.Deleted.ToArray());
		}

		[TestMethod]
		public void Api_Subscriptions_Delete()
		{
			// Arrange
			var api = new CategoriesApiMock();

			var scope = new Scope { Name = "Scope 1" };
			api.Scopes.Create(scope);

			var category = new Category { Name = "Category X", Scope = scope };
			api.Categories.Create(category);

			var receivedEvents = new List<ApiObjectsChangedEvent<Category>>();

			using var subscription = api.Categories.Subscribe();
			subscription.Changed += (s, e) => receivedEvents.Add(e);

			// Act
			api.Categories.Delete(category);

			// Assert
			Assert.HasCount(1, receivedEvents);

			var receivedEvent = receivedEvents[0];
			CollectionAssert.AreEquivalent(Array.Empty<Category>(), receivedEvent.Created.ToArray());
			CollectionAssert.AreEquivalent(Array.Empty<Category>(), receivedEvent.Updated.ToArray());
			CollectionAssert.AreEquivalent(new[] { category }, receivedEvent.Deleted.ToArray());
		}
	}
}
