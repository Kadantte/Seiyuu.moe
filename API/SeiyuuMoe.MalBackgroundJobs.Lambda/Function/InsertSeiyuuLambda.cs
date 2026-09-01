using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Infrastructure.Database.Characters;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seasons;
using SeiyuuMoe.Infrastructure.Database.Seiyuus;
using SeiyuuMoe.Infrastructure.S3;
using SeiyuuMoe.Infrastructure.Tenrai;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class InsertSeiyuuLambda : BaseLambda
	{
		private static readonly TenraiService TenraiService;

		static InsertSeiyuuLambda()
		{
			var serverKey = ConfigurationReader.TenraiServerKey;
			TenraiService = new TenraiService(TenraiClientFactory.Create(serverKey));
		}

		protected async override Task HandleAsync()
		{
			Console.WriteLine($"InsertSeiyuuLambda was invoked.");

			var dbConfig = ConfigurationReader.DatabaseConfiguration;
			using var dbContext = new SeiyuuMoeContext(dbConfig);

			var handler = CreateHandler(dbContext);
			await handler.HandleAsync();
		}

		private static InsertSeiyuuHandler CreateHandler(SeiyuuMoeContext dbContext)
		{
			var scheduleConfiguration = ConfigurationReader.MalBgJobsScheduleConfiguration;

			var animeRepository = new AnimeRepository(dbContext);
			var seiyuuRepository = new SeiyuuRepository(dbContext);
			var characterRepository = new CharacterRepository(dbContext);
			var animeRoleRepository = new AnimeRoleRepository(dbContext);
			var seasonRepository = new SeasonRepository(dbContext);

			var s3Client = new S3Service();

			return new InsertSeiyuuHandler(
				scheduleConfiguration.InsertSeiyuuBatchSize,
				scheduleConfiguration.DelayBetweenCallsInSeconds,
				seiyuuRepository,
				seasonRepository,
				characterRepository,
				animeRepository,
				animeRoleRepository,
				TenraiService,
				s3Client
			);
		}
	}
}
