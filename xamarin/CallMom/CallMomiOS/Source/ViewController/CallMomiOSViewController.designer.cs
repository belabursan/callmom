// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace CallMomiOS
{
	[Register ("CallMomiOSViewController")]
	partial class CallMomiOSViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIActivityIndicatorView MomActivity { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel MomLabel { get; set; }

		[Action ("UIButton13_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton13_TouchUpInside (UIButton sender);

		[Action ("UIButton15_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton15_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (MomActivity != null) {
				MomActivity.Dispose ();
				MomActivity = null;
			}
			if (MomLabel != null) {
				MomLabel.Dispose ();
				MomLabel = null;
			}
		}
	}
}
