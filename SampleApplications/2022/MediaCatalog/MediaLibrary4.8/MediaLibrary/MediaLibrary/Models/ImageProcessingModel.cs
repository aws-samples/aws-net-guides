using System.Collections.Generic;

namespace MediaLibrary.Models
{
    public class ImageProcessingModel
    {
        public string ImageUri { get; set; }
        public List<string> Labels { get; set; }
        public string OriginalName { get; set; }
        public string ImageName { get; set; }
    }
}
