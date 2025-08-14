using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;

namespace Yunity.ViewModels
{
    public class CBoardDetailViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? Time { get; set; }
        public string? Poster { get; set; }
        public string? Type { get; set; }
        public string? State { get; set; }

    }
}
