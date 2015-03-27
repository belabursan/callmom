using System;
using System.Threading.Tasks;

namespace CallMomCore
{
	public class BroadcastService : IBroadcastService
	{
		private static readonly object _Lock = new object ();

		public BroadcastService ()
		{
		}

		#region IBroadcastService implementation

		public event EventHandler<CallMomEventArgs> CoreEvents;

		public void RaiseNewEvent (object sender, CallMomEventArgs data, int delay = 0)
		{
			Task.Run (async () => {
				if (delay > 0) {
					await Task.Delay (delay);
				}
				OnDataChanged (sender, data);
			});
		}

		#endregion

		public virtual void OnDataChanged (object sender, CallMomEventArgs data)
		{
			lock (_Lock) {
				//run in new thread
				Task.Run (() => {
					var handler = CoreEvents;
					if (handler != null) {
						handler (sender, data);
					}
				});
			}
		}


	}
}

