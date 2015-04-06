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
	[Register ("CallMomiOSViewController")]
	partial class CallMomiOSViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton CallMomButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton CancelButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Info { get; set; }

		[Action ("UIButton13_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton13_TouchUpInside (UIButton sender);

		[Action ("UIButton15_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton15_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (CallMomButton != null) {
				CallMomButton.Dispose ();
				CallMomButton = null;
			}
			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}
			if (Info != null) {
				Info.Dispose ();
				Info = null;
			}
		}
	}
}
