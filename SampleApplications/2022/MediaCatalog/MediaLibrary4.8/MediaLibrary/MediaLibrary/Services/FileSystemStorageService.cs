using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace MediaLibrary.Services
{
    /// <summary>
    /// Implementation for a local file store, mainly used for local testing.
    /// </summary>
    public class FileSystemStorageService : IStorageService
    {
        private string baseFileLocation = Path.PathSeparator + "mockstorage";

        public FileSystemStorageService ()
        {
            if (!Directory.Exists (baseFileLocation))
            {
                Directory.CreateDirectory (baseFileLocation);
            }
        }
        /// <summary>
        /// Write the file to primary storage, retruing the file storage location.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string SaveFile(HttpPostedFileBase file)
        {
            var filePath = Path.Combine(baseFileLocation, DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName));
            using (var stream = System.IO.File.Create(filePath))
            {
                file.InputStream.CopyTo(stream);
                return Path.Combine(baseFileLocation, filePath);
            }
        }

        public Task<string[]> GetFileList()
        {
            return new Task<string[]>(() =>
            {
                 return Directory.GetFiles(baseFileLocation);
            });
            
        }

        public Task<int> PurgeFiles()
        {
            return new Task<int>(() =>
            {
                string[] FileList = Directory.GetFiles(baseFileLocation);
                foreach (string file in FileList)
                {
                    File.Delete(file);
                }
                return FileList.Length;
            });
        }

        public async Task<bool> DeleteFile(string KeyName)
        {
            try
            {
                File.Delete(string.Format("{0}{1}", new[] { baseFileLocation, KeyName }));
                return true;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return false;
            }
        }
    }
}
