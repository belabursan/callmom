using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace CallMomiOS
{
	partial class SettingsViewController : UIViewController
	{
		public SettingsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			Console.WriteLine (" did load");
			SetupRegisterBall ();
			SetupResetBall ();
			SetupAboutBall ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			Console.WriteLine (" will dissapear");
		}

		private void SetupRegisterBall ()
		{
			RegisterButton.Layer.CornerRadius = RegisterButton.Frame.Size.Height / 2;
			RegisterButton.Layer.BorderWidth = 3;
			RegisterButton.Layer.BorderColor = UIColor.LightGray.CGColor;
		}

		private void SetupResetBall ()
		{
			ResetButton.Layer.CornerRadius = ResetButton.Frame.Size.Height / 2;
			ResetButton.Layer.BorderWidth = 3;
			ResetButton.Layer.BorderColor = UIColor.LightGray.CGColor;
		}

		private void SetupAboutBall ()
		{
			AboutButton.Layer.CornerRadius = AboutButton.Frame.Size.Height / 2;
			AboutButton.Layer.BorderWidth = 2;
			AboutButton.Layer.BorderColor = UIColor.DarkGray.CGColor;
		}

		partial void AboutButton_TouchUpInside (UIButton sender)
		{
			Console.WriteLine ("about");
		}

		partial void RegisterButton_TouchUpInside (UIButton sender)
		{
			Console.WriteLine (" register");
		}

		partial void ResetButton_TouchUpInside (UIButton sender)
		{
			Console.WriteLine ("reset");
		}
	}
}
