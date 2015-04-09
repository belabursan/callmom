using System;
using UIKit;
using CallMomCore;

namespace CallMomiOS
{
	public class MomBaseViewController : UIViewController
	{
		public MomBaseViewController (IntPtr handle) : base (handle)
		{
		}

		protected string HandleResult (int click)
		{
			switch (click) {
			case ReturnValue.NotRegistered:
				return "Not Registered";
			case ReturnValue.Cancelled:
				return "Cancelled";
			case ReturnValue.NetworkError:
				return "Network Error";
			case ReturnValue.Success:
				return "Success";
			default:
				return "What Happened?";
			}
		}
	}
}

