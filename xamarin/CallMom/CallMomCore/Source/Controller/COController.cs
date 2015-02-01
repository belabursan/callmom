using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CallMomCore
{
	public class COController : ICOController
	{
		public COController ()
		{
		}

		#region ICOController implementation

		public async Task<int> DoTheCallAsync ()
		{
			Debug.WriteLine ("[Controller] - doing the call");
			await Task.Delay (100);
			return 1;
		}

		public async Task CancelTheCall ()
		{
			Debug.WriteLine ("[Controller] - canceling the call");
			await Task.Delay (100);
		}

		#endregion
	}
}

