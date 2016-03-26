using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace widkeyPaperDiaper
{
    public class Client
    {
        public string nextStep { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string Gender { get; set; }
        public string PassportNo { get; set; }
        public string __VIEWSTATE { get; set; }
        public string __VIEWSTATEGENERATOR { get; set; }
        public string __CMS_CurrentUrl { get; set; }
        public string __EVENTVALIDATION { get; set; }
        public CookieCollection cookieContainer = null;

        public Client(string userName, string password, string passportNo, string familyName, string givenName, string gender)
        {
            UserName = userName;
            Password = password;
            PassportNo = passportNo;
            FamilyName = familyName;
            GivenName = givenName;
            Gender = gender;
        }
    }
}
