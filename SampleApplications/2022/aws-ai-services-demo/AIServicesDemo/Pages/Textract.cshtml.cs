using Amazon.Textract;
using Amazon.Textract.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Encodings.Web;

namespace AIServicesDemo.Pages
{
    public class TextractModel : PageModel
    {
        [BindProperty]
        public IFormFile? FormFile { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Result { get; set; } = String.Empty;

        private readonly IAmazonTextract _textractClient;
        private readonly IWebHostEnvironment _hostenvironment;

        public TextractModel(IAmazonTextract textractClient, IWebHostEnvironment hostenvironment)
        {
            _textractClient = textractClient;
            _hostenvironment = hostenvironment;
        }

        public void OnGet()
        {
        }

        public async Task OnPostAsync()
        {
            if (FormFile == null)
            {
                return;
            }
            // save image to display it
            var fileName = String.Format("{0}{1}", Guid.NewGuid().ToString(), Path.GetExtension(FormFile.FileName));

            using (var stream = new FileStream(Path.Combine(_hostenvironment.WebRootPath, "uploads", fileName), FileMode.Create))
            {
                await FormFile.CopyToAsync(stream);
                FileName = fileName;
            }

            var memoryStream = new MemoryStream();
            await FormFile.CopyToAsync(memoryStream);

            var request = new AnalyzeExpenseRequest()
            {
                Document = new Document { Bytes = memoryStream }
            };

            var response = await _textractClient.AnalyzeExpenseAsync(request);

            var stringBuilder = new StringBuilder();

            foreach (var document in response.ExpenseDocuments)
            {
                stringBuilder.AppendLine("Summary:<br>");
                stringBuilder.AppendLine("==========================<br>");
                foreach (var field in document.SummaryFields)
                {
                    if (field.Type.Text != "OTHER")
                    {
                        stringBuilder.AppendFormat(
                            "Type: <b>{0}</b>, Text: <b>{1}</b><br>",
                            field.Type.Text,
                            field.ValueDetection.Text);
                    }
                }

                stringBuilder.AppendLine("==========================<br>");
                stringBuilder.AppendLine("Items:<br>");
                stringBuilder.AppendLine("==========================<br>");
                foreach (var itemGroup in document.LineItemGroups)
                {
                    foreach (var item in itemGroup.LineItems)
                    {
                        var price = item.LineItemExpenseFields.FirstOrDefault(i => i.Type.Text == "PRICE")?.ValueDetection?.Text;
                        var title = item.LineItemExpenseFields.FirstOrDefault(i => i.Type.Text == "ITEM")?.ValueDetection?.Text;

                        stringBuilder.AppendFormat(
                            "Item: <b>{0}</b>, Price: <b>{1}</b><br>",
                            title,
                            price);               
                    }            
                }
            }

            Result = stringBuilder.ToString();
        }
    }
}
