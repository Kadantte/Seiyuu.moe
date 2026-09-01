using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seasons;
using SeiyuuMoe.Infrastructure.Tenrai;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class UpdateAnimeLambda : BaseSqsLambda<UpdateAnimeMessage>
	{
		private static readonly TenraiService TenraiService;

		static UpdateAnimeLambda()
		{
			var serverKey = ConfigurationReader.TenraiServerKey;
			TenraiService = new TenraiService(TenraiClientFactory.Create(serverKey));
		}

		protected async override Task HandleAsync(UpdateAnimeMessage message)
		{
			Console.WriteLine($"UpdateAnimeLambda was invoked for anime {message.Id}");

			var dbConfig = ConfigurationReader.DatabaseConfiguration;
			using var dbContext = new SeiyuuMoeContext(dbConfig);

			var handler = CreateHandler(dbContext);
			await handler.HandleAsync(message);
		}

		private static UpdateAnimeHandler CreateHandler(SeiyuuMoeContext dbContext)
		{
			var animeRepository = new AnimeRepository(dbContext);
			var seasonRepository = new SeasonRepository(dbContext);

			return new UpdateAnimeHandler(animeRepository, seasonRepository, TenraiService);
		}
	}
}
