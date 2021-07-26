using System;

namespace Mindscan.Media.VideoUtils.FileStorage
{
	public interface IFileStorageFacade : IDisposable
	{
		Uri FileStorageUrl { get; }

		Uri UploadFromBase64String(string imgBase64, string name);
		string CheckFile(string fileName, long localSize);
		Uri SendMediaFileToFileStorage(string localPath);
	}
}