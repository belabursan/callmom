using System;
using System.Threading.Tasks;

namespace CallMomCore
{
	public class SettingsController : ISettingsController
	{
		private readonly ISettingsService _settingsService;
		private ICommand _register;

		public SettingsController (ISettingsService settingsService)
		{
			_settingsService = settingsService;
			_register = null;
		}

		#region ISettingsController implementation

		public async Task<int> DoRegister (string passw)
		{
			int retv = ReturnValue.AlreadyRunning;
			if (_register == null) {
				_register = new Register ();
				retv = await _register.ExecuteAsync (passw);
				_register = null;
			}
			return retv;
		}

		public void DoReset ()
		{
			if (_register != null) {
				Cancel ();
			}

			_settingsService.ResetTODefaults ();
		}

		public int Cancel ()
		{
			return _register != null ? _register.Cancel () : ReturnValue.NotRunning;
		}


		public SettingsData GetSettings ()
		{
			SettingsData settings = new SettingsData ();
			settings.IP = _settingsService.GetIP ();
			settings.Port = _settingsService.GetPort ();
			settings.TimeoutSec = _settingsService.GetConnectTimeOut ();
			settings.IsRegistred = IsRegistered ();
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
			try {
				byte[] key = _settingsService.GetServerPublicKey ();
				return (key != null && key.Length > 10);
			} catch (MomSqlException mox) {
				if (mox.ErrorCode == MomSqlException.NOT_FOUND) {
					return false;
				}
				throw mox;
			}
		}

		public string GetAbout ()
		{
			return "blabla";
		}

		#endregion
	}
}

