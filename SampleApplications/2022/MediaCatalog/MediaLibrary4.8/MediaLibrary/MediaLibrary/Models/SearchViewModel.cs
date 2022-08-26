using System.Collections.Generic;

namespace MediaLibrary.Models
{
    public class SearchViewModel
    {
        public string CurrentValue { get; set; }
        public IList<string> Labels { get; set; }
        public List<FileMetadataDataModel> Items { get; set; }
    }
}
