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
		UIBarButtonItem settingsButton;
		UIViewController _settingsViewController;

		private const string DefaultCallMomButtonTitle = "Call Mom";
		private UIColor DefaultCallMomButtonColor;

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
			DefaultCallMomButtonColor = CallMomButton.BackgroundColor;
			this.NavigationController.NavigationBar.Translucent = true;
			base.ViewDidLoad ();
			DefaultCallMomButtonColor = CallMomButton.BackgroundColor;
			//_settingsViewController = this.Storyboard.InstantiateViewController ("SettingsViewController") as SettingsViewController;

			settingsButton = new UIBarButtonItem (
				UIImage.FromFile ("settings@2x.png"),
				UIBarButtonItemStyle.Plain,
				(s, e) => {
					//this.NavigationController.PushViewController (_settingsViewController, true);
					System.Diagnostics.Debug.WriteLine ("++button tapped");
				}
			);

			NavigationItem.RightBarButtonItem = settingsButton;
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
			AnimateCallMomStart ();

			int click = await _callController.DoTheCallAsync ();
			Console.WriteLine ("[GUI] - call ended with code {0}", click);

			switch (click) {
			case ReturnValue.NotRegistered:
				AnimateInfo ("Not Registered");
				break;
			case ReturnValue.Cancelled:
				AnimateInfo ("Cancelled");
				break;
			case ReturnValue.NetworkError:
				AnimateInfo ("Network Error");
				break;
			case ReturnValue.Success:
				AnimateInfo ("Success");
				break;
			}
		}

		private void DoCancel ()
		{
			int ret = _callController.CancelTheCall ();
			if (ret == ReturnValue.Cancelled) {
				AnimateCallMomCancel ();
			}
			// else ReturnValue.NotStarted
		}

		void AnimateInfo (string info)
		{
			Info.SetTitle (info, UIControlState.Normal);
			UIView.Animate (2.0f, 1, UIViewAnimationOptions.CurveLinear,
				() => {
					this.Info.BackgroundColor = UIColor.Black;
				},
				() => {
					this.Info.BackgroundColor = UIColor.White;
					Info.SetTitle ("", UIControlState.Normal);
				}
			);
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

