using System;
using System.Threading.Tasks;

namespace CallMomCore
{
	public interface ICOController
	{
		Task<int> DoTheCallAsync ();

		int CancelTheCall ();
	}
}

