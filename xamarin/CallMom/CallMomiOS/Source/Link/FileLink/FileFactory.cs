using System;
using CallMomCore;
using UIKit;
using Foundation;
using System.IO;


namespace CallMomiOS
{
	public class FileFactory : IFileFactory
	{
		private const string _imagesDirectory = "Key";
		private readonly string _imagesPath;

		public FileFactory ()
		{
			string _baseDirectory;
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var docs = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0];
				_baseDirectory = docs.Path;
			} else {
				_baseDirectory = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			}
			_imagesPath = Path.Combine (_baseDirectory, _imagesDirectory);

			if (!Directory.Exists (_imagesPath)) {
				Directory.CreateDirectory (_imagesPath);
			}
		}


		#region IFileFactory implementation

		public Stream FileStream (string name)
		{
			try {
				//create path
				string path = GetPath (name);
				Console.WriteLine ("[FileFactory] - opening stream for {0}", path);

				//open file stream
				Stream fileStream = File.Open (path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

				return fileStream;
			} catch (Exception ex) {
				Console.WriteLine ("[FileFactory] - exception in GetFileStream: {0}", U.InnerExMessage (ex));
				throw;
			}
		}

		public void DeleteFile (string name)
		{
			try {
				string path = GetPath (name);

				if (File.Exists (path)) {
					File.Delete (path);
				}
			} catch (Exception ex) {
				Console.WriteLine ("[FileFactory] - exception in DeleteFile: {0}", U.InnerExMessage (ex));
				throw;
			}
		}

		#endregion

		private string GetPath (string fileName)
		{
			if (string.IsNullOrEmpty (fileName)) {
				Console.WriteLine ("[FileFactory] - filename is null or empty!");
				throw new ArgumentNullException ("fileName", "File name is null or empty");
			}

			return Path.Combine (_imagesPath, fileName);
		}
	}
}

