using SeiyuuMoe.Domain.Publishers;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.SqsMessages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Handlers
{
	public class ScheduleSeiyuuHandler
	{
		private readonly int _batchSize;
		private readonly ISeiyuuRepository _seiyuuRepository;
		private readonly ISeiyuuUpdatePublisher _seiyuuUpdatePublisher;

		public ScheduleSeiyuuHandler(int batchSize, ISeiyuuRepository seiyuuRepository, ISeiyuuUpdatePublisher seiyuuUpdatePublisher)
		{
			_batchSize = batchSize;
			_seiyuuRepository = seiyuuRepository;
			_seiyuuUpdatePublisher = seiyuuUpdatePublisher;
		}

		public async Task HandleAsync()
		{
			var thresholdDate = DateTime.UtcNow.AddDays(-7);

			var batch = await _seiyuuRepository.GetOlderThanModifiedDate(thresholdDate, _batchSize, null, null);

			if (batch.Count == 0)
			{
				return;
			}

			var messages = batch.Select(a => new UpdateSeiyuuMessage { Id = a.Id, MalId = a.MalId }).ToList();
			await _seiyuuUpdatePublisher.PublishSeiyuuUpdatesAsync(messages);
		}
	}
}