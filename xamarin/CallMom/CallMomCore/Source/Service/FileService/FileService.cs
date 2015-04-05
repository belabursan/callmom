using System;
using System.Threading.Tasks;
using System.Threading;

namespace CallMomCore
{
	public class FileService : IFileService
	{
		private readonly IFileFactory _fileFactory;
		private const string _KeyFileName = "server.pub";
		private byte[] _buffer;

		public FileService (IFileFactory fileFactory)
		{
			_fileFactory = fileFactory;
			_buffer = new byte[8192];
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
			int readed = await _fileFactory.FileStream (name).ReadAsync (_buffer, 0, _buffer.Length, token);
			byte[] file = new byte[readed];
			Array.Copy (_buffer, file, readed);
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

