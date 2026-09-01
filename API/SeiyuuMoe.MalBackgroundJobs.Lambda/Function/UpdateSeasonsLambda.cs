using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seasons;
using SeiyuuMoe.Infrastructure.Tenrai;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class UpdateSeasonsLambda : BaseLambda
	{
		private static readonly TenraiService TenraiService;

		static UpdateSeasonsLambda()
		{
			var serverKey = ConfigurationReader.TenraiServerKey;
			TenraiService = new TenraiService(TenraiClientFactory.Create(serverKey));
		}

		protected override async Task HandleAsync()
		{
			Console.WriteLine("UpdateSeasonsLambda was invoked");

			var dbConfig = ConfigurationReader.DatabaseConfiguration;
			using var dbContext = new SeiyuuMoeContext(dbConfig);

			var handler = CreateHandler(dbContext);
			await handler.HandleAsync();
		}

		private UpdateSeasonsHandler CreateHandler(SeiyuuMoeContext dbContext)
		{
			var seasonRepository = new SeasonRepository(dbContext);

			return new UpdateSeasonsHandler(seasonRepository, TenraiService);
		}
	}
}
