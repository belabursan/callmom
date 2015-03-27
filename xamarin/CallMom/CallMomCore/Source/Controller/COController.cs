using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CallMomCore
{
	public class COController : ICOController
	{
		private Call _call;

		public COController ()
		{
			_call = null;
		}

		#region ICOController implementation

		public async Task<int> DoTheCallAsync ()
		{
			Debug.WriteLine ("[Controller] - doing the call");
			int returnValue = ReturnValue.AlreadyRunning;

			if (_call == null) {
				_call = new Call ();
				returnValue = await _call.Execute ();
				_call = null;
			}

			return returnValue;
		}

		public async Task<int> CancelTheCall ()
		{
			Debug.WriteLine ("[Controller] - canceling the call");

			if (_call != null) {
				return await _call.Cancel ();
			}

			return ReturnValue.NotRunning;
		}

		#endregion
	}
}

