using System;
using System.Threading.Tasks;

namespace CallMomCore
{
	/// <summary>
	/// Interface for commands
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		/// Execute this instance.
		/// </summary>
		/// <returns>ReturnValue.Success or error code</returns>
		Task<int> ExecuteAsync ();

		/// <summary>
		/// Cancels a command, finishing the execution
		/// </summary>
		/// <returns> ReturnValue.NotRunning if not executing
		/// or ReturnValue.Cancelled if execution is canceled</returns>
		int Cancel ();
	}
}

