using System;
using UIKit;
using CallMomCore;
using System.Diagnostics;

namespace CallMomiOS
{
	public class MomBaseViewController : UIViewController
	{
		public MomBaseViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
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

