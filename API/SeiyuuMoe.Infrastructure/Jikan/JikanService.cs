using JikanDotNet;
using SeiyuuMoe.Domain.MalUpdateData;
using SeiyuuMoe.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Jikan
{
	public class JikanService : IMalApiService
	{
		private readonly IJikan _jikanClient;

		public JikanService(IJikan jikanClient)
		{
			_jikanClient = jikanClient;
		}

		public async Task<MalAnimeUpdateData> GetAnimeDataAsync(long malId)
		{
			var parsedData = await _jikanClient.GetAnimeAsync(malId);

			if (parsedData?.Data is null)
			{
				return null;
			}

			var animeTitles = parsedData.Data.Titles;

			return new MalAnimeUpdateData(
				GetMainTitle(animeTitles),
				parsedData.Data.Synopsis,
				GetTitleByType(animeTitles, "English"),
				GetTitleByType(animeTitles, "Japanese"),
				GetSynonyms(animeTitles),
				parsedData.Data.Members,
				EmptyStringIfPlaceholder(parsedData.Data.Images?.JPG?.ImageUrl),
				parsedData.Data.Aired?.From,
				parsedData.Data.Type,
				parsedData.Data.Status,
				parsedData.Data.Season.ToString(),
				parsedData.Data.Year
			);
		}

		public async Task<MalCharacterUpdateData> GetCharacterDataAsync(long malId)
		{
			var parsedData = await _jikanClient.GetCharacterAsync(malId);

			if (parsedData?.Data is null)
			{
				return null;
			}

			return new MalCharacterUpdateData(
				parsedData.Data.Name,
				parsedData.Data.About,
				parsedData.Data.NameKanji,
				EmptyStringIfPlaceholder(parsedData.Data.Images?.JPG?.ImageUrl),
				(parsedData.Data.Nicknames != null && parsedData.Data.Nicknames.Any()) ? string.Join(';', parsedData.Data.Nicknames) : string.Empty,
				parsedData.Data.Favorites
			);
		}

		public async Task<MalSeasonUpdateData> GetSeasonDataAsync()
		{
			var parsedData = await _jikanClient.GetSeasonArchiveAsync();

			if (parsedData?.Data is null)
			{
				return null;
			}

			var latestYear = parsedData?.Data?.FirstOrDefault();

			return new MalSeasonUpdateData(latestYear.Year, latestYear.Season.Last().ToString());
		}

		public async Task<MalSeiyuuFullUpdateData> GetSeiyuuFullDataAsync(long malId)
		{
			var parsedData = await _jikanClient.GetPersonFullDataAsync(malId);

			if (parsedData?.Data is null)
			{
				return null;
			}

			return new MalSeiyuuFullUpdateData(
				MapSeiyuuData(parsedData.Data),
				MapVoiceActingRoles(parsedData.Data.VoiceActingRoles)
			);
		}

		private static MalSeiyuuUpdateData MapSeiyuuData(Person person)
		{
			return new MalSeiyuuUpdateData(
				person.Name,
				person.About,
				$"{person.FamilyName ?? string.Empty} {person.GivenName ?? string.Empty}".Trim(),
				EmptyStringIfPlaceholder(person.Images?.JPG?.ImageUrl),
				person.MemberFavorites,
				person.Birthday
			);
		}

		private static ICollection<MalVoiceActingRoleUpdateData> MapVoiceActingRoles(ICollection<VoiceActingRole> voiceActingRoles)
		{
			if (voiceActingRoles is null)
			{
				return new List<MalVoiceActingRoleUpdateData>();
			}

			return voiceActingRoles.Select(
				x => new MalVoiceActingRoleUpdateData(
					x.Anime.MalId,
					x.Character.MalId,
					x.Role
				)
			).ToList();
		}

		private static string EmptyStringIfPlaceholder(string imageUrl)
		{
			var isEmptyOrPlaceholder = string.IsNullOrWhiteSpace(imageUrl) ||
				imageUrl.Equals("https://cdn.myanimelist.net/images/questionmark_23.gif") ||
				imageUrl.Equals("https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png");

			return isEmptyOrPlaceholder ? string.Empty : imageUrl;
		}

		private static string GetMainTitle(ICollection<TitleEntry> titles)
		{
			var defaultTitle = GetTitleByType(titles, "Default");

			if (!string.IsNullOrWhiteSpace(defaultTitle))
			{
				return defaultTitle;
			}

			return titles?.Select(x => x?.Title).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
		}

		private static string GetTitleByType(ICollection<TitleEntry> titles, string type)
			=> titles?.FirstOrDefault(
				x => x != null
					&& !string.IsNullOrWhiteSpace(x.Type)
					&& x.Type.Equals(type, StringComparison.OrdinalIgnoreCase)
			)?.Title;

		private static string GetSynonyms(ICollection<TitleEntry> titles)
		{
			var synonyms = titles?
				.Where(x => x != null && !string.IsNullOrWhiteSpace(x.Type) && x.Type.Equals("Synonym", StringComparison.OrdinalIgnoreCase))
				.Select(x => x.Title)
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.ToList();

			return synonyms != null && synonyms.Any() ? string.Join(';', synonyms) : string.Empty;
		}
	}
}
