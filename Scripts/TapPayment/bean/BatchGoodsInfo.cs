

using LC.Newtonsoft.Json;


namespace TapTap.Payment.bean
{
	public class BatchGoodsInfo
	{
		public Goods[] goods;
		public string regionId;

		public override string ToString ()
		{
			return JsonConvert.SerializeObject ( this );
		}
	}
}


