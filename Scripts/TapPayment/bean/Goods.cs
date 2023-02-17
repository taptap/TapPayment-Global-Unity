namespace TapTap.Payment.bean
{
	public class Goods
	{
		public class Config
		{
			public string name;
			public string description;
			public string language;
		}

		public class Price
		{
			public string amount;
			public string currency;
			public string region;
		}

		public int type;
		public string id;
		public Price price;
		public Config config;
	}
}