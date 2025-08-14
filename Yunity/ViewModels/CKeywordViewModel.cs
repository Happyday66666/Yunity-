using Microsoft.AspNetCore.Mvc.Rendering;

namespace Yunity.ViewModels
{
    public class CKeywordViewModel
    {
      
        public string? txtKeyword { get; set; }
       // public string? SelectedField { get; set; }
        public string[]? SelectedFields { get; set; }  // 用來存儲多選的欄位
                                                     
        public string? Keyword { get; set; }

        public int? Pa_id { get; set; }

        public string? MFeeName { get; set; }
        public string? title { get; set; }
        public string? body { get; set; }
    }
}
