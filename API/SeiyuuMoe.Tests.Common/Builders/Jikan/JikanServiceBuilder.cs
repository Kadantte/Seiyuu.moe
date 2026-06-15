using JikanDotNet;
using JikanDotNet.Exceptions;
using Moq;
using SeiyuuMoe.Infrastructure.Jikan;
using System.Collections.Generic;

namespace SeiyuuMoe.Tests.Common.Builders.Jikan
{
	public class JikanServiceBuilder
	{
		public readonly Mock<IJikan> JikanClient = new Mock<IJikan>();

		public JikanService Build() => new JikanService(JikanClient.Object);

		public JikanServiceBuilder WithAnimeReturned(Anime anime)
		{
			JikanClient.Setup(x => x.GetAnimeAsync(It.IsAny<long>()))
				.ReturnsAsync(new BaseJikanResponse<Anime>() { Data = anime });
			return this;
		}

		public JikanServiceBuilder WithTwoAnimeReturned(Anime firstAnime, Anime secondAnime)
		{
			JikanClient.SetupSequence(x => x.GetAnimeAsync(It.IsAny<long>()))
				.ReturnsAsync(new BaseJikanResponse<Anime>() { Data = firstAnime })
				.ReturnsAsync(new BaseJikanResponse<Anime>() { Data = secondAnime });
			return this;
		}

		public JikanServiceBuilder WithGetAnimeThrowing()
		{
			JikanClient.Setup(x => x.GetAnimeAsync(It.IsAny<long>()))
				.ThrowsAsync(new JikanRequestException());
			return this;
		}

		public JikanServiceBuilder WithCharacterReturned(Character character)
		{
			JikanClient.Setup(x => x.GetCharacterAsync(It.IsAny<long>()))
				.ReturnsAsync(new BaseJikanResponse<Character>() { Data = character });
			return this;
		}

		public JikanServiceBuilder WithTwoCharactersReturned(Character firstCharacter, Character secondCharacter)
		{
			JikanClient.SetupSequence(x => x.GetCharacterAsync(It.IsAny<long>()))
				.ReturnsAsync(new BaseJikanResponse<Character>() { Data = firstCharacter })
				.ReturnsAsync(new BaseJikanResponse<Character>() { Data = secondCharacter });
			return this;
		}

		public JikanServiceBuilder WithGetCharacterThrowing()
		{
			JikanClient.Setup(x => x.GetCharacterAsync(It.IsAny<long>()))
				.ThrowsAsync(new JikanRequestException());
			return this;
		}

		public JikanServiceBuilder WithPersonReturned(Person person, ICollection<VoiceActingRole> voiceActingRoles)
		{
			JikanClient.Setup(x => x.GetPersonFullDataAsync(It.IsAny<long>()))
				.ReturnsAsync(new BaseJikanResponse<PersonFull>() { Data = CreatePersonFull(person, voiceActingRoles) });
			return this;
		}

		public JikanServiceBuilder WithGetPersonThrowing()
		{
			JikanClient.Setup(x => x.GetPersonFullDataAsync(It.IsAny<long>()))
				.ThrowsAsync(new JikanRequestException());
			return this;
		}

		public JikanServiceBuilder WithTwoPersonsReturned(Person firstPerson, Person secondPerson)
		{
			JikanClient.SetupSequence(x => x.GetPersonFullDataAsync(It.IsAny<long>()))
				.ReturnsAsync(new BaseJikanResponse<PersonFull>() { Data = CreatePersonFull(firstPerson, null) })
				.ReturnsAsync(new BaseJikanResponse<PersonFull>() { Data = CreatePersonFull(secondPerson, null) });
			return this;
		}

		public JikanServiceBuilder WithLastSeasonArchiveReturned(SeasonArchive seasonArchive)
		{
			JikanClient.Setup(x => x.GetSeasonArchiveAsync())
				.ReturnsAsync(new PaginatedJikanResponse<ICollection<SeasonArchive>>() { Data = new List<SeasonArchive> { seasonArchive } });
			return this;
		}

		public JikanServiceBuilder WithGetSeasonArchiveThrowing()
		{
			JikanClient.Setup(x => x.GetSeasonArchiveAsync())
				.ThrowsAsync(new JikanRequestException());
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
