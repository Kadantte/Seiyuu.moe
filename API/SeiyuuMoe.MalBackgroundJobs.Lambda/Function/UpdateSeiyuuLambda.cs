using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Infrastructure.Database.Characters;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seasons;
using SeiyuuMoe.Infrastructure.Database.Seiyuus;
using SeiyuuMoe.Infrastructure.Tenrai;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class UpdateSeiyuuLambda : BaseSqsLambda<UpdateSeiyuuMessage>
	{
		private static readonly TenraiService TenraiService;

		static UpdateSeiyuuLambda()
		{
			var serverKey = ConfigurationReader.TenraiServerKey;
			TenraiService = new TenraiService(TenraiClientFactory.Create(serverKey));
		}

		protected async override Task HandleAsync(UpdateSeiyuuMessage message)
		{
			Console.WriteLine($"UpdateSeiyuuLambda was invoked for seiyuu {message.Id}");

			var dbConfig = ConfigurationReader.DatabaseConfiguration;
			using var dbContext = new SeiyuuMoeContext(dbConfig);

			var handler = CreateHandler(dbContext);
			await handler.HandleAsync(message);
		}

		private static UpdateSeiyuuHandler CreateHandler(SeiyuuMoeContext dbContext)
		{
			var animeRepository = new AnimeRepository(dbContext);
			var seiyuuRepository = new SeiyuuRepository(dbContext);
			var characterRepository = new CharacterRepository(dbContext);
			var seiyuuRoleRepository = new SeiyuuRoleRepository(dbContext);
			var animeRoleRepository = new AnimeRoleRepository(dbContext);
			var seasonRepository = new SeasonRepository(dbContext);

			return new UpdateSeiyuuHandler(seiyuuRepository, animeRepository, characterRepository, seiyuuRoleRepository, animeRoleRepository, seasonRepository, TenraiService);
		}
	}
}
