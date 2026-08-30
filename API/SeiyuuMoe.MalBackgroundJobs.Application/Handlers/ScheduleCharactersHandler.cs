using SeiyuuMoe.Domain.Publishers;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.SqsMessages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Handlers
{
	public class ScheduleCharactersHandler
	{
		private readonly int _batchSize;
		private readonly ICharacterRepository _characterRepository;
		private readonly ICharactersUpdatePublisher _charactersUpdatePublisher;

		public ScheduleCharactersHandler(int batchSize, ICharacterRepository characterRepository, ICharactersUpdatePublisher charactersUpdatePublisher)
		{
			_batchSize = batchSize;
			_characterRepository = characterRepository;
			_charactersUpdatePublisher = charactersUpdatePublisher;
		}

		public async Task HandleAsync()
		{
			var thresholdDate = DateTime.UtcNow.AddDays(-90);

			var batch = await _characterRepository.GetOlderThanModifiedDate(thresholdDate, _batchSize, null, null);

			if (batch.Count == 0)
			{
				return;
			}

			var messages = batch.Select(a => new UpdateCharacterMessage { Id = a.Id, MalId = a.MalId }).ToList();
			await _charactersUpdatePublisher.PublishCharacterUpdatesAsync(messages);
		}
	}
}