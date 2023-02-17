using System;
using TapTap.Payment.util;
using TapTap.Payment.bean;
using UnityEngine;

namespace TapTap.Payment
{
	class TapPayment
	{
		public enum ErrorCode
		{
			UNKNOWN = -1,
			OK = 0,
			ILLEGAL_ARGUMENT = 1,
			PAYMENT_FAILED = 3,
			NETWORK_ERROR = 4,
			
			NOT_LOGIN = 401,

			GOODS_NOT_FOUND = 1001,
			REPEAT_PURCHASE = 1002,
			ORDER_DUPLICATE = 1003,
			ORDER_NOT_FOUND = 1004,
			ORDER_CREATE_FAILED = 1005,
		}

		public class Callback
		{
			public delegate void OnSuccess ( string goodsId, string orderId );
			public delegate void OnFailure ( ErrorCode code, string message );
			
			
			public OnSuccess onSuccess { get; set; }
			
			public OnFailure onFailure { get; set; }
			
			public Action onCancel { get; set; }
		}

		private class CallbackWrapper : AndroidJavaProxy
		{
			private Callback callback;

			public CallbackWrapper ( Callback callback ) : base ( "com.taptap.payment.api.TapPayment$Callback" )
			{
				this.callback = callback;
			}

			public void onSuccess ( string goodsId, string orderId )
			{
				callback.onSuccess?.Invoke ( goodsId, orderId );
			}

			public void onFailure ( AndroidJavaObject code, string msg )
			{
				callback.onFailure?.Invoke ( Enum.Parse < ErrorCode > ( code.Call < string > ( "name" ) ), msg );
			}

			public void onCancel ()
			{
				callback.onCancel?.Invoke ();
			}
		}
		
		private static AndroidJavaObject getSDK ()
		{
			var TapPaySDKLoader = new AndroidJavaClass ( "com.taptap.payment.shell.TapPaySDKLoader" );
			return TapPaySDKLoader.CallStatic < AndroidJavaObject > ( "getSDK" );
		}
		
		public static void requestPay ( string goodsId, string regionId, Callback callback )
		{
			var UnityPlayer = new AndroidJavaClass ( "com.unity3d.player.UnityPlayer" );
			var currentActivity = UnityPlayer.GetStatic < AndroidJavaObject > ( "currentActivity" );
			getSDK ().Call ( "requestPay", currentActivity, goodsId, regionId, new CallbackWrapper ( callback ) );
		}

		public static BatchGoodsInfo batchQueryGoods ( string [] goodsIds )
		{
			var batchGoodsInfoObject = getSDK ().Call < AndroidJavaObject > ( "batchQueryGoods", new object [] { goodsIds } );
			return JavaUnityInterface.CopyFormObject < BatchGoodsInfo > ( batchGoodsInfoObject );

		}

		public static Order queryOrder ( string orderId )
		{
			var order = getSDK ().Call < AndroidJavaObject > ( "queryOrder", orderId );
			return JavaUnityInterface.CopyFormObject < Order > ( order );
		}
	}
}