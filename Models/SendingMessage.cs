using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_Test.Models
{
    public class SendingMessage
    {
        public int IdSent { get; set; }
        public int MessageId { get; set; }
        public DateTime Sent_Date { get; set; }
        public string Confirmation_Code { get; set; }
        public object tbl_Messages { get; set; }
    }
}
