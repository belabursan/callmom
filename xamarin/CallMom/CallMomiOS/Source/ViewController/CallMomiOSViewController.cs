using System;
using System.Drawing;

using Foundation;
using UIKit;
using CallMomCore;
using Autofac;
using System.Threading.Tasks;
using System.Net;


namespace CallMomiOS
{
	public partial class CallMomiOSViewController : UIViewController
	{
		private readonly ICOController _callController;
		private static object Lock = new object ();

		private const string DefaultCallMomButtonTitle = "Call Mom";
		private UIColor DefaultCallMomButtonColor = UIColor.LightGray;

		public CallMomiOSViewController (IntPtr handle) : base (handle)
		{
			_callController = App.Container.Resolve<ICOController> ();
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
			DefaultCallMomButtonColor = CallMomButton.BackgroundColor;
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
			InvokeOnMainThread (async () => await DoCall ());
		}

		partial void UIButton15_TouchUpInside (UIButton sender)
		{
			DoCancel ();
		}

		private async Task DoCall ()
		{
			MomActivity.StartAnimating ();
			AnimateCallMomStart ();

			int click = await _callController.DoTheCallAsync ();
			Console.WriteLine ("[GUI] - call ended with code {0}", click);

			if (click != ReturnValue.AlreadyRunning) {
				MomActivity.StopAnimating ();
			}
			switch (click) {
			case ReturnValue.NotRegistered:
				AnimateNotRegistered ();
				break;
			}

			//todo handle returnvalue
		}

		private void DoCancel ()
		{
			int ret = _callController.CancelTheCall ();
			if (ret == ReturnValue.Cancelled) {
				AnimateCallMomCancel ();
			}
			// else ReturnValue.NotStarted
		}

		void AnimateNotRegistered ()
		{
			throw new NotImplementedException ();
		}

		private void AnimateCallMomStart ()
		{
			AnimateCallMomButtonColor (UIColor.FromRGB (100, 175, 250), "Calling");
		}

		private void AnimateCallMomCancel ()
		{
			AnimateCallMomButtonColor (UIColor.Red, "Cancelling");
		}

		private void AnimateCallMomButtonColor (UIColor color, string title)
		{
			lock (Lock) {
				CallMomButton.SetTitle (title, UIControlState.Normal);
				UIView.Animate (1.0f, 0, UIViewAnimationOptions.CurveLinear,
					() => {
						this.CallMomButton.BackgroundColor = color;
					},
					() => {
						this.CallMomButton.BackgroundColor = DefaultCallMomButtonColor;
						CallMomButton.SetTitle (DefaultCallMomButtonTitle, UIControlState.Normal);
					}
				);
			}
		}
			
	}
}

