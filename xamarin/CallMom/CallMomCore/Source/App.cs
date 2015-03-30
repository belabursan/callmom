using System;
using Autofac;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CallMomCore
{
	public class App
	{
		public static IContainer Container { get; set; }

		private static bool _isAppInitialized = false;

		public static bool IsAppInitialized {
			get {
				return _isAppInitialized;
			}
		}


		public static void Initialize (ContainerBuilder builder)
		{
			Debug.WriteLine ("[Init] - Initialize");
			InitDependencies (builder);
			SetDefaults ();
		}

		/**
		 * Initialize IoC-dependencies here
		 */
		private static void InitDependencies (ContainerBuilder builder)
		{
			builder.RegisterType<COController> ().As<ICOController> ();
			builder.RegisterType<SQLiteLink> ().As<ISQLiteLink> ().SingleInstance ();
			builder.RegisterType<SettingService> ().As<ISettingsService> ().SingleInstance ();
			builder.RegisterType<BroadcastService> ().As<IBroadcastService> ().SingleInstance ();
			builder.RegisterType<NetworkLink> ().As<INetworkLink> ().SingleInstance ();
			builder.RegisterType<CryptoService> ().As<ICryptoService> ().SingleInstance ();

			App.Container = builder.Build ();
		}

		private static void SetDefaults ()
		{
			Debug.WriteLine ("[Init] - setting defaults");
			ISettingsService settings = App.Container.Resolve<ISettingsService> ();
			settings.InsertCallTime (Defaults.CALLTIMEOUT);
			settings.InsertIP (Defaults.IP);
			settings.InsertPort (Defaults.PORT);
			settings.InsertNetworkTimeoutSeconds (Defaults.NETTIMEOUT);
			settings.InsertConnectTimeOut (Defaults.CONNECTTIMEOUT);
		}

	}
}

