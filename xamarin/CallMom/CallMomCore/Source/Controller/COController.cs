using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CallMomCore
{
	public class COController : ICOController
	{
		private ICommand _call;

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
				returnValue = await _call.ExecuteAsync ();
				_call = null;
			}

			return returnValue;
		}

		public int CancelTheCall ()
		{
			Debug.WriteLine ("[Controller] - canceling the call");

			return _call != null ? _call.Cancel () : ReturnValue.NotRunning;

		}

		#endregion
	}
}

