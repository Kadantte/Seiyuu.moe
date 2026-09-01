using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Database.Characters;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Tenrai;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class UpdateCharacterLambda : BaseSqsLambda<UpdateCharacterMessage>
	{
		private static readonly TenraiService TenraiService;

		static UpdateCharacterLambda()
		{
			var serverKey = ConfigurationReader.TenraiServerKey;
			TenraiService = new TenraiService(TenraiClientFactory.Create(serverKey));
		}

		protected async override Task HandleAsync(UpdateCharacterMessage message)
		{
			Console.WriteLine($"UpdateCharacterLambda was invoked for character {message.Id}");

			var dbConfig = ConfigurationReader.DatabaseConfiguration;
			using var dbContext = new SeiyuuMoeContext(dbConfig);

			var handler = CreateHandler(dbContext);
			await handler.HandleAsync(message);
		}

		private static UpdateCharacterHandler CreateHandler(SeiyuuMoeContext dbContext)
		{
			var characterRepository = new CharacterRepository(dbContext);

			return new UpdateCharacterHandler(characterRepository, TenraiService);
		}
	}
}
