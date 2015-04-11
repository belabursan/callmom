// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace CallMomiOS
{
	[Register ("SettingsViewController")]
	partial class SettingsViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton AboutButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField IPTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField PortTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton RegisterButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton ResetButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISlider TimeoutSlider { get; set; }

		[Action ("AboutButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void AboutButton_TouchUpInside (UIButton sender);

		[Action ("IPTextFieldClicked:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void IPTextFieldClicked (UITextField sender);

		[Action ("PortTexFieldClicked:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void PortTexFieldClicked (UITextField sender);

		[Action ("RegisterButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void RegisterButton_TouchUpInside (UIButton sender);

		[Action ("ResetButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void ResetButton_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (AboutButton != null) {
				AboutButton.Dispose ();
				AboutButton = null;
			}
			if (IPTextField != null) {
				IPTextField.Dispose ();
				IPTextField = null;
			}
			if (PortTextField != null) {
				PortTextField.Dispose ();
				PortTextField = null;
			}
			if (RegisterButton != null) {
				RegisterButton.Dispose ();
				RegisterButton = null;
			}
			if (ResetButton != null) {
				ResetButton.Dispose ();
				ResetButton = null;
			}
			if (TimeoutSlider != null) {
				TimeoutSlider.Dispose ();
				TimeoutSlider = null;
			}
		}
	}
}
