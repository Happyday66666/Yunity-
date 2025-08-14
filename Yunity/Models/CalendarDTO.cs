using System.ComponentModel.DataAnnotations;

namespace Yunity.Models
{
    public class CalendarDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool Deletable { get; set; }

    }
}
