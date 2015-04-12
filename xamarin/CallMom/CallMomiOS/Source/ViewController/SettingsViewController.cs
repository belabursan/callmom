using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CallMomCore;
using Autofac;
using CoreAnimation;

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
			SetupInfo ();
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

		private void SetupInfo ()
		{
			SettingsInfoText.Enabled = false;
			SettingsInfoText.Layer.CornerRadius = SettingsInfoText.Frame.Size.Height / 2;
			SettingsInfoText.Layer.BackgroundColor = UIColor.White.CGColor;
			SettingsInfoText.Layer.BorderWidth = 2;
			SettingsInfoText.SetTitleColor (UIColor.White, UIControlState.Normal);
			SettingsInfoText.BackgroundColor = UIColor.White;
			SettingsInfoText.Layer.BorderColor = UIColor.White.CGColor;

			SettingsInfoText.SetTitle (String.Empty, UIControlState.Normal);
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

		private void ShowInfo (string info)
		{
			SettingsInfoText.Enabled = false;
			SettingsInfoText.SetTitle (info, UIControlState.Normal);
			SettingsInfoText.Enabled = true;
			UIView.Animate (3.0f, 1, UIViewAnimationOptions.CurveLinear,
				() => {
					this.SettingsInfoText.SetTitleColor (UIColor.Black, UIControlState.Normal);
					this.SettingsInfoText.BackgroundColor = UIColor.Orange;
					this.SettingsInfoText.Layer.BorderColor = UIColor.Red.CGColor;
				},
				() => {
					SettingsInfoText.Enabled = false;
					this.SettingsInfoText.SetTitleColor (UIColor.White, UIControlState.Normal);
					this.SettingsInfoText.BackgroundColor = UIColor.White;
					this.SettingsInfoText.Layer.BorderColor = UIColor.White.CGColor;

					SettingsInfoText.SetTitle (String.Empty, UIControlState.Normal);
					SettingsInfoText.Enabled = true;
				}
			);

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
			ShowInfo ("ERROR");
		}

		partial void AboutButton_TouchUpInside (UIButton sender)
		{
			ShowAbout ();
		}

		partial void RegisterButton_TouchUpInside (UIButton sender)
		{
			UIAlertView alert = new UIAlertView ();
			SaveValues ();
			alert.Title = "Enter Password";
			alert.AddButton ("Register");
			alert.AddButton ("Cancel");
			alert.AlertViewStyle = UIAlertViewStyle.SecureTextInput;
			alert.Clicked += (object s, UIButtonEventArgs ev) => {
				if (ev.ButtonIndex == 0) {
					InvokeOnMainThread (async () => {
						string psswd = alert.GetTextField (0).Text;
						if (String.IsNullOrEmpty (psswd) || psswd.Length < 4) {
							ShowInfo ("Bad Password");
							return;
						}
						int result = await _settingsController.DoRegister (psswd);
						HandleRegisterResult (result);
					});
				} else {
					return;
				}
			};
			alert.Show ();
		}

		void HandleRegisterResult (int result)
		{
			if (result == ReturnValue.Success) {
				SwapButtons (true);
			} else {
				ShowInfo (HandleResult (result));
			}
		}

		void SwapButtons (bool b)
		{
			UIView.Animate (
				0.5f,
				0.5f,
				UIViewAnimationOptions.CurveLinear,
				() => ShowResetAndRegistered (b),
				null
			);
		}

		partial void ResetButton_TouchUpInside (UIButton sender)
		{
			_settingsController.DoReset ();
			SwapButtons (false);
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
