using System;
using System.Drawing;

using Foundation;
using UIKit;


namespace CallMomiOS
{
	public partial class CallMomiOSViewController : UIViewController
	{
		public CallMomiOSViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			MomActivity.StopAnimating ();
			MomLabel.Hidden = true;
			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion

		partial void UIButton13_TouchUpInside (UIButton sender)
		{
			MomActivity.StartAnimating ();
		}

		partial void UIButton15_TouchUpInside (UIButton sender)
		{
			MomActivity.StopAnimating ();
		}
	}
}

