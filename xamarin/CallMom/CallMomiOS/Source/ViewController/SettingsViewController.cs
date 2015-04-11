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
			base.ViewDidLoad ();
			SetupRegisterBall ();
			SetupResetBall ();
			SetupAboutBall ();

			this.PortTextField.ShouldReturn += (textField) => {
				textField.ResignFirstResponder ();
				return true;
			};
			this.IPTextField.ShouldReturn += (textField) => {
				textField.ResignFirstResponder ();
				return true;
			};

		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			try {
				var _settingsData = _settingsController.GetSettings ();

				ShowResetAndRegistered (_settingsData.IsRegistred);
				ShowValues (_settingsData);
			} catch (MomException mox) {
				HandleMomError (mox);
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			SaveValues ();
		}

		private void SetupRegisterBall ()
		{
			RegisterButton.Enabled = false;
			RegisterButton.Layer.CornerRadius = RegisterButton.Frame.Size.Height / 2;
			RegisterButton.Layer.BorderColor = UIColor.White.CGColor;
			RegisterButton.Layer.BorderWidth = 3;
		}

		private void SetupResetBall ()
		{
			ResetButton.Enabled = false;
			ResetButton.Layer.CornerRadius = ResetButton.Frame.Size.Height / 2;
			ResetButton.Layer.BorderColor = UIColor.White.CGColor;
			ResetButton.Layer.BorderWidth = 3;
		}

		private void SetupAboutBall ()
		{
			AboutButton.Layer.CornerRadius = AboutButton.Frame.Size.Height / 2;
			AboutButton.Layer.BorderWidth = 2;
			AboutButton.Layer.BorderColor = UIColor.DarkGray.CGColor;
		}

		private void ShowResetAndRegistered (bool show)
		{
			ResetButton.Enabled = false;
			ResetButton.Layer.BorderColor = show ? UIColor.DarkGray.CGColor : UIColor.White.CGColor;
			ResetButton.BackgroundColor = show ? UIColor.Gray : UIColor.White;
			ResetButton.Enabled = show;

			RegisterButton.Enabled = false;
			RegisterButton.Layer.BorderColor = show ? UIColor.White.CGColor : UIColor.DarkGray.CGColor;
			RegisterButton.BackgroundColor = show ? UIColor.White : UIColor.Gray;
			RegisterButton.Enabled = !show;
		}

		async void ShowAbout ()
		{
			new UIAlertView (
				"About CallMom",
				String.Format ("\n{0}", _settingsController.GetAbout ()),
				null,
				"Ok",
				null
			).Show ();
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

		void ShowValues (SettingsData values)
		{
			try {
				this.IPTextField.Text = values.IP;
				this.PortTextField.Text = values.Port.ToString ();
				this.TimeoutSlider.SetValue (values.TimeoutSec, true);
				ShowResetAndRegistered (values.IsRegistred);
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
			ShowAbout ();
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
			_settingsController.DoReset ();
		}

		partial void IPTextFieldClicked (UITextField sender)
		{
			IPTextField.BecomeFirstResponder ();
		}

		partial void PortTexFieldClicked (UITextField sender)
		{
			PortTextField.BecomeFirstResponder ();
		}
	}
}
