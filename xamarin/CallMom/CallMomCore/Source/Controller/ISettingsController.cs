using System;
using System.Threading.Tasks;

namespace CallMomCore
{
	public interface ISettingsController
	{
		Task DoRegister ();

		void DoReset ();

		NetworkArguments GetSettings ();

		void SetSettings (NetworkArguments arguments);

		bool IsRegistered ();

		string GetAbout ();
	}
}
