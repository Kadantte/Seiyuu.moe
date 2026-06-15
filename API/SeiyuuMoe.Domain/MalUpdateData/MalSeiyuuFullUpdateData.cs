using System.Collections.Generic;

namespace SeiyuuMoe.Domain.MalUpdateData
{
	public class MalSeiyuuFullUpdateData
	{
		public MalSeiyuuUpdateData SeiyuuData { get; }

		public ICollection<MalVoiceActingRoleUpdateData> VoiceActingRoles { get; }

		public MalSeiyuuFullUpdateData(MalSeiyuuUpdateData seiyuuData, ICollection<MalVoiceActingRoleUpdateData> voiceActingRoles)
		{
			SeiyuuData = seiyuuData;
			VoiceActingRoles = voiceActingRoles ?? new List<MalVoiceActingRoleUpdateData>();
		}
	}
}
