using System;
using System.Threading;
using Autofac;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CallMomCore
{
	public class Register : CommandBase
	{

		#region implemented abstract members of CommandBase

		protected override Task<int> Run (CancellationToken token)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

