using Microsoft.AspNetCore.Http;
using MediaLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLIntegration.Tests.Services
{
    [TestClass]
    public class FileSystemStorageServiceTests
    {
        [TestMethod]
        public void TestWritingFile ()
        {
            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.txt";
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(content);
                    writer.Flush();
                    stream.Position = 0;

                    //create FormFile with desired data
                    IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
                    IStorageService svc = new FileSystemStorageService();
                    svc.SaveFile(file);
                }
            }
            
        }
    }
}
