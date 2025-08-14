namespace Yunity.Models
{
    public class CCalendar
    {
        //public string Title { get; set; }

        //public int StartDayOfWeak { get; set; }

        //public int EndDay { get; set; }

        //public string EventNote { get; set; }
        public string Calendar { get; set; }
        public string Title { get; set; } // 月曆標題
        public int StartDayOfWeak { get; set; } // 當月第一天是星期幾
        public int EndDay { get; set; } // 當月的最後一天
        public string EventNote { get; set; } // 全局的備註
        public Dictionary<int, List<string>> Events { get; set; } // 每日事件

        public Dictionary<int, List<string>> ComEvents { get; set; } // 廠商每日事件

        public int PreviousYear { get; set; }
        public int PreviousMonth { get; set; }
        public int NextYear { get; set; }
        public int NextMonth { get; set; }
    }
}
