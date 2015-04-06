using System;
using System.Threading.Tasks;

namespace CallMomCore
{
	public class SettingsController : ISettingsController
	{
		private readonly ISettingsService _settingsService;

		public SettingsController (ISettingsService settingsService)
		{
			_settingsService = settingsService;
		}

		#region ISettingsController implementation

		public async Task DoRegister ()
		{
			throw new NotImplementedException ();
		}

		public void DoReset ()
		{
			throw new NotImplementedException ();
		}

		public NetworkArguments GetSettings ()
		{
			NetworkArguments args = new NetworkArguments ();
			args.Ip = _settingsService.GetIP ();
			args.Port = _settingsService.GetPort ();
			args.ConnectTimeout = _settingsService.GetConnectTimeOut () / 1000;
			return args;
		}

		public void SetSettings (NetworkArguments arguments)
		{
			_settingsService.InsertIP (arguments.Ip);
			_settingsService.InsertPort (arguments.Port);
			_settingsService.InsertConnectTimeOut (arguments.ConnectTimeout);
		}

		public bool IsRegistered ()
		{
			return false;
		}

		public string GetAbout ()
		{
			return "blabla";
		}

		#endregion
	}
}

