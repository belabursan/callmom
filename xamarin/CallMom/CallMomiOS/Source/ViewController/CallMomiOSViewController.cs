using System;
using UIKit;
using CallMomCore;
using Autofac;
using System.Threading.Tasks;
using CoreGraphics;


namespace CallMomiOS
{
	public partial class CallMomiOSViewController : MomBaseViewController
	{
		private static object _lock = new object ();
		private readonly ICOController _callController;
		private UIBarButtonItem _settingsButton;
		private UIViewController _settingsViewController;

		private const string _defaultCallMomButtonTitle = "Call Mom";
		private UIColor _defaultCallMomButtonColor;


		public CallMomiOSViewController (IntPtr handle) : base (handle)
		{
			_callController = App.Container.Resolve<ICOController> ();
			_settingsViewController = null;
		}


		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.NavigationController.NavigationBar.TintColor = UIColor.Orange;
			SetupCallBall ();
			SetupCancelBall ();
			SetupInfoBall ();
			SetupNavigationButton ();
		}

		#endregion

		private void SetupCallBall ()
		{
			CallMomButton.Layer.CornerRadius = CallMomButton.Frame.Size.Height / 2;
			CallMomButton.Layer.BorderWidth = 8;
			CallMomButton.Layer.BorderColor = UIColor.Gray.CGColor;
			_defaultCallMomButtonColor = CallMomButton.BackgroundColor;
		}

		private void SetupCancelBall ()
		{
			CancelButton.Layer.CornerRadius = CancelButton.Frame.Size.Height / 2;
			CancelButton.Layer.BorderWidth = 3;
			CancelButton.Layer.BorderColor = UIColor.Gray.CGColor;
		}

		private void SetupInfoBall ()
		{
			Info.Enabled = false;
			Info.Layer.CornerRadius = Info.Frame.Size.Height / 2;
			Info.Layer.BorderWidth = 2;
			Info.Layer.BorderColor = UIColor.White.CGColor;

		}

		private void SetupNavigationButton ()
		{
			_settingsButton = new UIBarButtonItem (
				UIImage.FromFile ("settings@2x.png"),
				UIBarButtonItemStyle.Plain,
				(s, e) => {
					if (_settingsViewController == null) {
						_settingsViewController = this.Storyboard.InstantiateViewController ("CallSettingsViewController") as SettingsViewController;
					}
					this.NavigationController.PushViewController (_settingsViewController, true);
				}
			);

			NavigationItem.RightBarButtonItem = _settingsButton;
		}

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

			AnimateInfo (HandleResult (click), click == ReturnValue.Success);
		}

		private void DoCancel ()
		{
			int ret = _callController.CancelTheCall ();
			if (ret == ReturnValue.Cancelled) {
				AnimateCallMomCancel ();
			}
			// else ReturnValue.NotStarted
		}

		void AnimateInfo (string info, bool success)
		{
			Info.Enabled = false;
			Info.SetTitle (info, UIControlState.Normal);
			Info.Enabled = true;
			CGColor borderColor = success ? UIColor.Green.CGColor : UIColor.Red.CGColor;
			UIView.Animate (3.0f, 1, UIViewAnimationOptions.CurveLinear,
				() => {
					this.Info.Layer.BorderColor = borderColor;
					this.Info.SetTitleColor (UIColor.Black, UIControlState.Normal);
					this.Info.BackgroundColor = UIColor.Orange;
				},
				() => {
					Info.Enabled = false;
					this.Info.SetTitleColor (UIColor.White, UIControlState.Normal);
					this.Info.Layer.BorderColor = UIColor.White.CGColor;
					this.Info.BackgroundColor = UIColor.White;

					Info.SetTitle (String.Empty, UIControlState.Normal);
					Info.Enabled = true;
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
			lock (_lock) {
				nfloat borderwidht = this.CallMomButton.Layer.BorderWidth;
				CallMomButton.Enabled = false;
				CallMomButton.SetTitle (title, UIControlState.Normal);
				CallMomButton.Enabled = true;
				UIView.Animate (1.0f, 0, UIViewAnimationOptions.CurveLinear,
					() => {
						this.CallMomButton.BackgroundColor = color;
						this.CallMomButton.Layer.BorderWidth = (borderwidht + 2);
					},
					() => {
						this.CallMomButton.BackgroundColor = _defaultCallMomButtonColor;
						this.CallMomButton.Layer.BorderWidth = borderwidht;
						CallMomButton.Enabled = false;
						CallMomButton.SetTitle (_defaultCallMomButtonTitle, UIControlState.Normal);
						CallMomButton.Enabled = true;
					}
				);
			}
		}
			
	}
}

