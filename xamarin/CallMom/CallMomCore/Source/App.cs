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
			//controllers
			builder.RegisterType<COController> ().As<ICOController> ();
			builder.RegisterType<SettingsController> ().As<ISettingsController> ();

			//links
			builder.RegisterType<SQLiteLink> ().As<ISQLiteLink> ().SingleInstance ();
			builder.RegisterType<NetworkLink> ().As<INetworkLink> ().SingleInstance ();

			//services
			builder.RegisterType<SettingService> ().As<ISettingsService> ().SingleInstance ();
			builder.RegisterType<BroadcastService> ().As<IBroadcastService> ().SingleInstance ();
			builder.RegisterType<CryptoService> ().As<ICryptoService> ().SingleInstance ();
			builder.RegisterType<FileService> ().As<IFileService> ().SingleInstance ();



			App.Container = builder.Build ();
		}

		private static void SetDefaults ()
		{
			Debug.WriteLine ("[Init] - setting defaults");
			ISettingsService settings = App.Container.Resolve<ISettingsService> ();
			settings.FirstTimeInit ();
		}

	}
}

