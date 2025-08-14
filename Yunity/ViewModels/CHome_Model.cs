using System.ComponentModel;
using Yunity.Models;

namespace Yunity.ViewModels
{
    public class CHome_Model
    {

        [DisplayName("大樓合作倒數件數")]
        public string BD_ContractEnd { get; set; }

        [DisplayName("廠商合作倒數件數")]
        public string com_ContractEnd { get; set; }

        public static string user_name { get; set; }

    }
}