using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.ScheduleItems;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface ICharacterRepository
	{
		Task<AnimeCharacter> GetAsync(long characterMalId);

		Task AddAsync(AnimeCharacter character);

		Task UpdateAsync(AnimeCharacter character);

		Task<IReadOnlyList<CharacterScheduleItem>> GetOlderThanModifiedDate(DateTime olderThan, int pageSize = 150, DateTime? afterModificationDate = null, Guid? afterId = null);
	}
}