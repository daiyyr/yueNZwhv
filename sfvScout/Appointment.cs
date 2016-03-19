using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace widkeyPaperDiaper
{
    public class Appointment
    {
        public string CardNo { get; set; }
        public string CardPassword { get; set; }
        public string ChineseName { get; set; }
        public string JapaneseName { get; set; }
        public string Phone { get; set; }

        public Appointment(string cardNo, string cardPassword, string chineseName, string japaneseName, string phone)
        {
            CardNo = cardNo;
            CardPassword = cardPassword;
            ChineseName = chineseName;
            JapaneseName = japaneseName;
            Phone = phone;
        }
    }
}
