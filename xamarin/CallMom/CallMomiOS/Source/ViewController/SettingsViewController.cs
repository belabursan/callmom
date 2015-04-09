using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CallMomCore;
using Autofac;

namespace CallMomiOS
{
	partial class SettingsViewController : MomBaseViewController
	{
		private readonly ISettingsController _settingsController;


		public SettingsViewController (IntPtr handle) : base (handle)
		{
			_settingsController = App.Container.Resolve<ISettingsController> ();
		}

		public override void ViewDidLoad ()
		{
			Console.WriteLine (" did load");
			SetupRegisterBall ();
			SetupResetBall (_settingsController.IsRegistered ());
			SetupAboutBall ();
		}

		public override void ViewDidAppear (bool animated)
		{
			Console.WriteLine (" will appear");
			ShowResetAndRegistered (_settingsController.IsRegistered ());
			ShowValues ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			Console.WriteLine (" will disappear");
			SaveValues ();
		}

		private void SetupRegisterBall ()
		{
			RegisterButton.Layer.CornerRadius = RegisterButton.Frame.Size.Height / 2;
			RegisterButton.Layer.BorderWidth = 3;
			RegisterButton.Layer.BorderColor = UIColor.LightGray.CGColor;
			ShowResetAndRegistered (_settingsController.IsRegistered ());
		}

		private void SetupResetBall (bool show)
		{
			ResetButton.Layer.CornerRadius = ResetButton.Frame.Size.Height / 2;
			ResetButton.Layer.BorderWidth = 3;
			ShowResetAndRegistered (show);
		}

		private void SetupAboutBall ()
		{
			AboutButton.Layer.CornerRadius = AboutButton.Frame.Size.Height / 2;
			AboutButton.Layer.BorderWidth = 2;
			AboutButton.Layer.BorderColor = UIColor.DarkGray.CGColor;
		}

		private void ShowResetAndRegistered (bool show)
		{
			ResetButton.Enabled = show;
			ResetButton.Layer.BorderColor = show ? UIColor.DarkGray.CGColor : UIColor.White.CGColor;
			ResetButton.BackgroundColor = show ? UIColor.Gray : UIColor.White;

			RegisterButton.Enabled = !show;
			RegisterButton.Layer.BorderColor = show ? UIColor.White.CGColor : UIColor.DarkGray.CGColor;
			RegisterButton.BackgroundColor = show ? UIColor.White : UIColor.Gray;
		}

		void SaveValues ()
		{
			try {
				_settingsController.SetSettings (new SettingsData (
					IPTextField.Text,
					PortTextField.Text,
					TimeoutSlider.Value
				));
			} catch (MomException mox) {
				HandleMomError (mox);
			}
		}

		void ShowValues ()
		{
			try {
				SettingsData values = _settingsController.GetSettings ();

				this.IPTextField.Text = values.IP;
				this.PortTextField.Text = values.Port.ToString ();
				this.TimeoutSlider.SetValue (values.TimeoutSec, true);
			} catch (MomException mox) {
				HandleMomError (mox);
			}
		}

		private void HandleMomError (MomException ex)
		{
			Console.WriteLine ("ERROR - todo-> handle, show ball?: " + ex.Message);
		}

		partial void AboutButton_TouchUpInside (UIButton sender)
		{
			Console.WriteLine ("about");
			var aboutText = _settingsController.GetAbout ();
		}

		partial void RegisterButton_TouchUpInside (UIButton sender)
		{
			int result;
			InvokeOnMainThread (async () => {
				result = await _settingsController.DoRegister ();
			});
		}

		partial void ResetButton_TouchUpInside (UIButton sender)
		{
			Console.WriteLine ("reset");
		}
	}
}
