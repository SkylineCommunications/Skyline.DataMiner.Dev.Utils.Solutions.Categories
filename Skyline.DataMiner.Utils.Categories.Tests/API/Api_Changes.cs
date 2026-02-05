namespace Skyline.DataMiner.Solutions.Categories.Tests.API
{
	using System.Reflection;

	using PublicApiGenerator;

	[TestClass]
	[UsesVerify]
	public sealed partial class Api_Changes
	{
		[TestMethod]
		public Task Api_NoPublicApiChanges_Common()
		{
			var assemblyName = "Skyline.DataMiner.Dev.Utils.Solutions.Categories";
			var publicApi = Assembly.Load(assemblyName).GeneratePublicApi();

			return Verifier.Verify(publicApi)
				.UseFileName($"{assemblyName}_PublicApi")
				.AutoVerify(includeBuildServer: false);
		}

		[TestMethod]
		public Task Api_NoPublicApiChanges_Automation()
		{
			var assemblyName = "Skyline.DataMiner.Dev.Utils.Solutions.Categories.Automation";
			var publicApi = Assembly.Load(assemblyName).GeneratePublicApi();

			return Verifier.Verify(publicApi)
				.UseFileName($"{assemblyName}_PublicApi")
				.AutoVerify(includeBuildServer: false);
		}

		[TestMethod]
		public Task Api_NoPublicApiChanges_Protocol()
		{
			var assemblyName = "Skyline.DataMiner.Dev.Utils.Solutions.Categories.Protocol";
			var publicApi = Assembly.Load(assemblyName).GeneratePublicApi();

			return Verifier.Verify(publicApi)
				.UseFileName($"{assemblyName}_PublicApi")
				.AutoVerify(includeBuildServer: false);
		}

		[TestMethod]
		public Task Api_NoPublicApiChanges_GQI()
		{
			var assemblyName = "Skyline.DataMiner.Dev.Utils.Solutions.Categories.GQI";
			var publicApi = Assembly.Load(assemblyName).GeneratePublicApi();

			return Verifier.Verify(publicApi)
				.UseFileName($"{assemblyName}_PublicApi")
				.AutoVerify(includeBuildServer: false);
		}
	}
}
