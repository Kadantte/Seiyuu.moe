using Moq;
using SeiyuuMoe.Infrastructure.Tenrai;
using System.Collections.Generic;
using Tenrai;
using Tenrai.Exceptions;

namespace SeiyuuMoe.Tests.Common.Builders.Tenrai
{
	public class TenraiServiceBuilder
	{
		public readonly Mock<ITenrai> TenraiClient = new Mock<ITenrai>();

		public TenraiService Build() => new TenraiService(TenraiClient.Object);

		public TenraiServiceBuilder WithAnimeReturned(Anime anime)
		{
			TenraiClient.Setup(x => x.GetAnimeAsync(It.IsAny<long>()))
				.ReturnsAsync(new BaseTenraiResponse<Anime>() { Data = anime });
			return this;
		}

		public TenraiServiceBuilder WithTwoAnimeReturned(Anime firstAnime, Anime secondAnime)
		{
			TenraiClient.SetupSequence(x => x.GetAnimeAsync(It.IsAny<long>()))
				.ReturnsAsync(new BaseTenraiResponse<Anime>() { Data = firstAnime })
				.ReturnsAsync(new BaseTenraiResponse<Anime>() { Data = secondAnime });
			return this;
		}

		public TenraiServiceBuilder WithGetAnimeThrowing()
		{
			TenraiClient.Setup(x => x.GetAnimeAsync(It.IsAny<long>()))
				.ThrowsAsync(new TenraiRequestException());
			return this;
		}

		public TenraiServiceBuilder WithCharacterReturned(Character character)
		{
			TenraiClient.Setup(x => x.GetCharacterAsync(It.IsAny<long>()))
				.ReturnsAsync(new BaseTenraiResponse<Character>() { Data = character });
			return this;
		}

		public TenraiServiceBuilder WithTwoCharactersReturned(Character firstCharacter, Character secondCharacter)
		{
			TenraiClient.SetupSequence(x => x.GetCharacterAsync(It.IsAny<long>()))
				.ReturnsAsync(new BaseTenraiResponse<Character>() { Data = firstCharacter })
				.ReturnsAsync(new BaseTenraiResponse<Character>() { Data = secondCharacter });
			return this;
		}

		public TenraiServiceBuilder WithGetCharacterThrowing()
		{
			TenraiClient.Setup(x => x.GetCharacterAsync(It.IsAny<long>()))
				.ThrowsAsync(new TenraiRequestException());
			return this;
		}

		public TenraiServiceBuilder WithPersonReturned(Person person, ICollection<VoiceActingRole> voiceActingRoles)
		{
			TenraiClient.Setup(x => x.GetPersonFullDataAsync(It.IsAny<long>()))
				.ReturnsAsync(new BaseTenraiResponse<PersonFull>() { Data = CreatePersonFull(person, voiceActingRoles) });
			return this;
		}

		public TenraiServiceBuilder WithGetPersonThrowing()
		{
			TenraiClient.Setup(x => x.GetPersonFullDataAsync(It.IsAny<long>()))
				.ThrowsAsync(new TenraiRequestException());
			return this;
		}

		public TenraiServiceBuilder WithTwoPersonsReturned(Person firstPerson, Person secondPerson)
		{
			TenraiClient.SetupSequence(x => x.GetPersonFullDataAsync(It.IsAny<long>()))
				.ReturnsAsync(new BaseTenraiResponse<PersonFull>() { Data = CreatePersonFull(firstPerson, null) })
				.ReturnsAsync(new BaseTenraiResponse<PersonFull>() { Data = CreatePersonFull(secondPerson, null) });
			return this;
		}

		public TenraiServiceBuilder WithLastSeasonArchiveReturned(SeasonArchive seasonArchive)
		{
			TenraiClient.Setup(x => x.GetSeasonArchiveAsync())
				.ReturnsAsync(new PaginatedTenraiResponse<ICollection<SeasonArchive>>() { Data = new List<SeasonArchive> { seasonArchive } });
			return this;
		}

		public TenraiServiceBuilder WithGetSeasonArchiveThrowing()
		{
			TenraiClient.Setup(x => x.GetSeasonArchiveAsync())
				.ThrowsAsync(new TenraiRequestException());
			return this;
		}

		private static PersonFull CreatePersonFull(Person person, ICollection<VoiceActingRole> voiceActingRoles)
		{
			if (person is null)
			{
				return null;
			}

			return new PersonFull
			{
				MalId = person.MalId,
				Url = person.Url,
				Name = person.Name,
				GivenName = person.GivenName,
				FamilyName = person.FamilyName,
				AlternativeNames = person.AlternativeNames,
				About = person.About,
				Images = person.Images,
				MemberFavorites = person.MemberFavorites,
				Birthday = person.Birthday,
				VoiceActingRoles = voiceActingRoles
			};
		}
	}
}
