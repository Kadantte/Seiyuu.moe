﻿using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using SeiyuuMoe.Application.Animes;
using SeiyuuMoe.Application.Animes.GetAnimeCardInfo;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Tests.Common.Builders.Model;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Unit.Tests.Application.Animes
{
	public class GetAnimeCardInfoQueryHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnNull_ShouldReturnNotFoundResponse()
		{
			// Given
			const long animeMalId = 0;
			var mockRepository = new Mock<IAnimeRepository>();
			var handler = new GetAnimeCardInfoQueryHandler(mockRepository.Object);

			// When
			var result = await handler.HandleAsync(new GetAnimeCardInfoQuery(animeMalId));

			// Then
			mockRepository.Verify(x => x.GetAsync(animeMalId), Times.Once);
			using (new AssertionScope())
			{
				result.Found.Should().BeFalse();
				result.Payload.Should().BeNull();
			}
		}

		[Fact]
		public async Task HandleAsync_GivenRepositoryReturnAnime_ShouldReturnResponse()
		{
			// Given
			const long animeMalId = 0;
			var mockRepository = new Mock<IAnimeRepository>();
			mockRepository.Setup(x => x.GetAsync(animeMalId)).ReturnsAsync(new AnimeBuilder().WithMalId(animeMalId).Build());
			var handler = new GetAnimeCardInfoQueryHandler(mockRepository.Object);

			// When
			var result = await handler.HandleAsync(new GetAnimeCardInfoQuery(animeMalId));

			// Then
			mockRepository.Verify(x => x.GetAsync(animeMalId), Times.Once);
			using (new AssertionScope())
			{
				result.Found.Should().BeTrue();
				result.Payload.Should().NotBeNull().And.BeOfType(typeof(AnimeCardDto));
			}
		}
	}
}