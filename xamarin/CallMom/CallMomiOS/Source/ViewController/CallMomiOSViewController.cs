using System;
using System.Drawing;

using Foundation;
using UIKit;
using CallMomCore;
using Autofac;
using System.Threading.Tasks;


namespace CallMomiOS
{
	public partial class CallMomiOSViewController : UIViewController
	{
		CallMomCore.ICOController ioc;

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

			ioc = new COController ();//App.Container.Resolve<ICOController> ();
			
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
			DoCall ();

		}

		partial void UIButton15_TouchUpInside (UIButton sender)
		{
			MomActivity.StopAnimating ();
			DoCancel ();
		}

		private async Task DoCall ()
		{
			int click = await ioc.DoTheCallAsync ();
		}

		private async Task DoCancel ()
		{
			await ioc.CancelTheCall ();
		}
	}
}

