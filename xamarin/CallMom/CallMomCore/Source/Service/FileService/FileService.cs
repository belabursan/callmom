using System;
using System.Threading.Tasks;
using System.Threading;

namespace CallMomCore
{
	public class FileService : IFileService
	{
		private readonly IFileFactory _fileFactory;
		private const string _KeyFileName = "server.pub";

		public FileService (IFileFactory fileFactory)
		{
			_fileFactory = fileFactory;
		}


		#region IFileService implementation

		public async Task SaveFileAsync (byte[] file, string name, CancellationToken token = default(CancellationToken))
		{
			if (file == null || file.Length < 1)
				throw new ArgumentNullException ("file", "file is null or empty");

			await _fileFactory.FileStream (name).WriteAsync (file, 0, file.Length, token);
		}

		public async Task<byte[]> GetFileAsync (string name, CancellationToken token = default(CancellationToken))
		{
			byte[] buffer = new byte[8192];
			int readed = await _fileFactory.FileStream (name).ReadAsync (buffer, 0, buffer.Length, token);
			byte[] file = new byte[readed];
			Array.Copy (buffer, file, readed);
			return file;
		}

		public async Task SavePublicKeyAsync (byte[] file, CancellationToken token = default(CancellationToken))
		{
			await SaveFileAsync (file, _KeyFileName, token);
		}

		public async Task<byte[]> GetPublicKeyAsync (CancellationToken token = default(CancellationToken))
		{
			return await GetFileAsync (_KeyFileName, token);
		}

		#endregion
	}
}

