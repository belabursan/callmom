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
			InitDependencies (builder);
		}

		/**
		 * Initialize IoC-dependencies here
		 */
		private static void InitDependencies (ContainerBuilder builder)
		{
			builder.RegisterType<COController> ().As<ICOController> ();
		}

	}
}

