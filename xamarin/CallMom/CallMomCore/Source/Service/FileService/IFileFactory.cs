using System;
using System.IO;

namespace CallMomCore
{
	public interface IFileFactory
	{
		Stream FileStream (string name);

		void DeleteFile (string name);
	}
}

