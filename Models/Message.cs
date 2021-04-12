using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_Test.Models
{
    public class Message
    {

        public int MessageId { get; set; }
        public DateTime Created_Date { get; set; }
        public string SendTo { get; set; }
        public string MessageValue { get; set; }
        public List<object> tbl_SendingMessages { get; set; }

    }
}
