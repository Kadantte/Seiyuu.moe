using SeiyuuMoe.Domain.Publishers;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.SqsMessages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Handlers
{
	public class ScheduleAnimesHandler
	{
		private readonly int _batchSize;
		private readonly IAnimeRepository _animeRepository;
		private readonly IAnimeUpdatePublisher _animeUpdatePublisher;

		public ScheduleAnimesHandler(int batchSize, IAnimeRepository animeRepository, IAnimeUpdatePublisher animeUpdatePublisher)
		{
			_batchSize = batchSize;
			_animeRepository = animeRepository;
			_animeUpdatePublisher = animeUpdatePublisher;
		}

		public async Task HandleAsync()
		{
			var thresholdDate = DateTime.UtcNow.AddDays(-31);

			var batch = await _animeRepository.GetOlderThanModifiedDate(thresholdDate, _batchSize, null, null);

			if (batch.Count == 0)
			{
				return;
			}

			var messages = batch.Select(a => new UpdateAnimeMessage { Id = a.Id, MalId = a.MalId }).ToList();
			await _animeUpdatePublisher.PublishAnimeUpdatesAsync(messages);
		}
	}
}