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

		public SettingsData GetSettings ()
		{
			SettingsData settings = new SettingsData ();
			settings.IP = _settingsService.GetIP ();
			settings.Port = _settingsService.GetPort ();
			settings.TimeoutSec = _settingsService.GetConnectTimeOut () / 1000;
			return settings;
		}

		public void SetSettings (SettingsData settings)
		{
			_settingsService.InsertIP (settings.IP);
			_settingsService.InsertPort (settings.Port);
			_settingsService.InsertConnectTimeOut (settings.TimeoutSec);
		}

		public bool IsRegistered ()
		{
			byte[] key = _settingsService.GetServerPublicKey ();
			return (key != null && key.Length > 10);
		}

		public string GetAbout ()
		{
			return "blabla";
		}

		#endregion
	}
}

