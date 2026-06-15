using FluentAssertions;
using FluentAssertions.Execution;
using JikanDotNet;
using Moq;
using SeiyuuMoe.Infrastructure.Jikan;
using SeiyuuMoe.Tests.Common.Builders.Jikan;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Jikan
{
	public class JikanServiceTests
	{
		[Fact]
		public async Task GetSeiyuuFullDataAsync_GivenNullResponse_ShouldReturnNull()
		{
			// Given
			var jikanService = new JikanServiceBuilder()
				.WithPersonReturned(null, null)
				.Build();

			// When
			var result = await jikanService.GetSeiyuuFullDataAsync(1);

			// Then
			result.Should().BeNull();
		}

		[Fact]
		public async Task GetSeiyuuFullDataAsync_GivenPersonData_ShouldMapSeiyuuFields()
		{
			// Given
			const string name = "Tomokazu Seki";
			const string about = "Hometown: Tokyo, Japan";
			const string givenName = "智一";
			const string familyName = "関";
			const string imageUrl = "https://cdn.myanimelist.net/images/person.jpg";
			const int popularity = 5000;
			var birthday = new DateTime(1972, 9, 8);

			var person = new Person
			{
				Name = name,
				About = about,
				GivenName = givenName,
				FamilyName = familyName,
				Images = new ImagesSet
				{
					JPG = new Image { ImageUrl = imageUrl }
				},
				MemberFavorites = popularity,
				Birthday = birthday
			};

			var jikanService = new JikanServiceBuilder()
				.WithPersonReturned(person, null)
				.Build();

			// When
			var result = await jikanService.GetSeiyuuFullDataAsync(1);

			// Then
			using (new AssertionScope())
			{
				result.Should().NotBeNull();
				result.SeiyuuData.Name.Should().Be(name);
				result.SeiyuuData.About.Should().Be(about);
				result.SeiyuuData.JapaneseName.Should().Be($"{familyName} {givenName}");
				result.SeiyuuData.ImageUrl.Should().Be(imageUrl);
				result.SeiyuuData.Popularity.Should().Be(popularity);
				result.SeiyuuData.Birthday.Should().Be(birthday);
				result.VoiceActingRoles.Should().BeEmpty();
			}
		}

		[Fact]
		public async Task GetSeiyuuFullDataAsync_GivenVoiceActingRoles_ShouldMapRoles()
		{
			// Given
			const long animeMalId = 100;
			const long characterMalId = 1000;

			var person = new Person
			{
				Name = "Test Seiyuu",
				GivenName = "花澤",
				FamilyName = "香菜"
			};

			var voiceActingRoles = new List<VoiceActingRole>
			{
				new VoiceActingRole
				{
					Role = "Main",
					Anime = new MalImageSubItem { MalId = animeMalId },
					Character = new MalImageSubItem { MalId = characterMalId }
				}
			};

			var jikanService = new JikanServiceBuilder()
				.WithPersonReturned(person, voiceActingRoles)
				.Build();

			// When
			var result = await jikanService.GetSeiyuuFullDataAsync(1);

			// Then
			using (new AssertionScope())
			{
				result.Should().NotBeNull();
				result.VoiceActingRoles.Should().ContainSingle();
				var role = result.VoiceActingRoles.Should().ContainSingle().Subject;
				role.AnimeMalId.Should().Be(animeMalId);
				role.CharacterMaId.Should().Be(characterMalId);
				role.RoleType.Should().Be("Main");
			}
		}

		[Fact]
		public async Task GetSeiyuuFullDataAsync_ShouldCallGetPersonFullDataAsyncOnce()
		{
			// Given
			const long malId = 42;
			var jikanServiceBuilder = new JikanServiceBuilder().WithPersonReturned(new Person { Name = "Test" }, null);
			var jikanService = jikanServiceBuilder.Build();

			// When
			await jikanService.GetSeiyuuFullDataAsync(malId);

			// Then
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonFullDataAsync(malId), Times.Once);
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonAsync(It.IsAny<long>()), Times.Never);
			jikanServiceBuilder.JikanClient.Verify(x => x.GetPersonVoiceActingRolesAsync(It.IsAny<long>()), Times.Never);
		}
	}
}
