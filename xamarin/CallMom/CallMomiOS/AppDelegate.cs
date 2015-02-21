﻿using System;
using System.Linq;
using System.Collections.Generic;

using Foundation;
using UIKit;
using Autofac;
using CallMomCore;

namespace CallMomiOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		private const string DatabaseName = "mom.db";

		public override UIWindow Window {
			get;
			set;
		}
		
		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation (UIApplication application)
		{
		}
		
		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
		}
		
		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
		}
		
		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			var builder = new ContainerBuilder ();
			builder.Register (x => new SQLiteFactory (DatabaseName)).As<ISQLiteFactory> ();
			//builder.Register (x => new NetworkFactory ()).As<INetworkFactory> ();
			builder.RegisterType<COController> ().As<ICOController> ().SingleInstance ();
			builder.RegisterType<NetworkFactory> ().As<INetworkFactory> ();
			App.Initialize (builder);
			return true;
		}
	}
}
