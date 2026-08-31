namespace SeiyuuMoe.Application.Seasons.GetSeasonSummaries
{
	public class GetSeasonSummariesQuery
	{
		public long Year { get; set; }

		public string Season { get; set; }

		public bool MainRolesOnly { get; set; }

		public bool TVSeriesOnly { get; set; }

		public int Page { get; set; }

		public int PageSize { get; set; }
	}
}