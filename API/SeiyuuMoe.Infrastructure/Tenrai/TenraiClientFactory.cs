using Tenrai;
using Tenrai.Config;

namespace SeiyuuMoe.Infrastructure.Tenrai
{
	public static class TenraiClientFactory
	{
		public static ITenrai Create(string serverKey = null)
		{
			var config = new TenraiClientConfiguration
			{
				SuppressException = true,
			};

			if (!string.IsNullOrWhiteSpace(serverKey))
			{
				config.ServerKey = serverKey;
			}

			return new TenraiClient(config);
		}
	}
}
