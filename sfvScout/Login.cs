using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Net.Cache;
using System.Text.RegularExpressions;
using System;

namespace widkeyPaperDiaper
{

        
    public class Login
    {
        static string rgx;
        static Match myMatch;

        //    string username = "";           
        //    string password = "";
        string username = "";
        string password = "";
        Form1 form1;
        CookieCollection cookieContainer = null;

        public Login(Form1 f)
        {
            form1 = f;
        }

        public int loginF()
        {
            form1.setLogT("login..");
            string respHtml = "";

            respHtml = Form1.weLoveYue(
                form1,
                "https://www.facebook.com/",
                "GET",
                "",
                false,
                "",
               ref cookieContainer,
                true);

            string lgnrnd = "";
            string token = "";
            string datr = "";

            if (respHtml.Equals("Found"))
            {
                form1.setLogT("getting login page failed!");
                return -1;
            }

            rgx = @"(?<=name=""reg_instance"" value="").+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                datr = myMatch.Groups[0].Value;
            }
            else
            {
                form1.setLogT("getting login page failed!");
                return -1;
            }

            rgx = @"(?<=input type=""hidden"" name=""lgnrnd"" value="").*?(?="" />)";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                lgnrnd = myMatch.Groups[0].Value;
            }

            rgx = @"(?<=type=""hidden"" name=""lsd"" value="").*?(?="" autocomplete=""off"" />)";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                token = myMatch.Groups[0].Value;
            }

                cookieContainer.Add(new Cookie("_js_datr", datr) { Domain = "www.facebook.com" });

            //to get cookies: datr
            // weLoveMuYue("https://www.facebook.com/hellocdn/results?data=%7B%22results%22%3A%5B%7B%22loading_time%22%3A0%2C%22platform%22%3A%22www%22%2C%22cdn%22%3A%22ak%22%2C%22resource_timing%22%3A%7B%22name%22%3A%22https%3A%2F%2Ffbcdn-photos-b-a.akamaihd.net%2Fhphotos-ak-prn1%2Ftest-80KB.jpg%22%2C%22entryType%22%3A%22resource%22%2C%22startTime%22%3A307.131182%2C%22duration%22%3A422.90310700000003%2C%22initiatorType%22%3A%22xmlhttprequest%22%2C%22redirectStart%22%3A0%2C%22redirectEnd%22%3A0%2C%22fetchStart%22%3A307.131182%2C%22domainLookupStart%22%3A307.131182%2C%22domainLookupEnd%22%3A307.131182%2C%22connectStart%22%3A307.131182%2C%22connectEnd%22%3A307.131182%2C%22secureConnectionStart%22%3A0%2C%22requestStart%22%3A308.652099%2C%22responseStart%22%3A398.19683299999997%2C%22responseEnd%22%3A730.0342890000001%7D%2C%22url%22%3A%22https%3A%2F%2Ffbcdn-photos-b-a.akamaihd.net%2Fhphotos-ak-prn1%2Ftest-80KB.jpg%22%2C%22headers%22%3A%22Access-Control-Allow-Origin%3A%20*%5Cr%5CnCache-Control%3A%20no-transform%2C%20max-age%3D8%5Cr%5CnContent-Length%3A%2079957%5Cr%5CnContent-Type%3A%20image%2Fjpeg%5Cr%5CnDate%3A%20Sat%2C%2019%20Sep%202015%2003%3A55%3A59%20GMT%5Cr%5CnExpires%3A%20Sat%2C%2019%20Sep%202015%2003%3A56%3A07%20GMT%5Cr%5CnLast-Modified%3A%20Fri%2C%2012%20Dec%202014%2000%3A53%3A28%20GMT%5Cr%5CnServer%3A%20proxygen%5Cr%5CnTiming-Allow-Origin%3A%20*%5Cr%5Cnx-akamai-session-info%3A%20name%3DBEGIN_CLOCK%3B%20value%3D1435761000%2C%20name%3DCLOCK_DURATION%3B%20value%3D6873959%2C%20name%3DFB_DISABLE_FULL_HTTPS%3B%20value%3Dtrue%2C%20name%3DFB_DISABLE_FULL_LOGGING%3B%20value%3Dtrue%2C%20name%3DFB_LOGGING_URL_SAMPLE%3B%20value%3Dtrue%2C%20name%3DFULL_PATH_KEY%3B%20value%3Dfalse%2C%20name%3DHSAFSERIAL%3B%20value%3D842%2C%20name%3DNOW_CLOCK%3B%20value%3D1442634959%2C%20name%3DORIGIN%3B%20value%3Dhphotos-ak-prn1%2C%20name%3DOVERRIDE_HTTPS_IE_CACHE_BUST%3B%20value%3Dall%2C%20name%3DSERIALNEXT%3B%20value%3D1791%2C%20name%3DSINGLE_TIER%3B%20value%3Dtrue%2C%20name%3DSINGLE_TIER_HVAL%3B%20value%3D789613%2C%20name%3DVALIDORIGIN%3B%20value%3Dtrue%3B%20full_location_id%3Dmetadata%5Cr%5Cnx-akamai-ssl-client-sid%3A%20B2VGSAuDC%2BONy6lq7deAkQ%3D%3D%2C%20jXyFJAJ1swG8eI9JJLO85A%3D%3D%2C%20eHGuYL6ebCGxtQ%2FFzTWoqQ%3D%3D%2C%20xTmDZCNM%2BaM1EFSiyU%2B5PQ%3D%3D%2C%2000ViDZZBGURh2d4RBXYqtA%3D%3D%2C%20ldw%2F1r4y03Q8umDIRzyoDw%3D%3D%2C%20QoOhGh3xuf88M%2BjTOOnWfg%3D%3D%2C%20Z8kyY5MKFQLQt3zz2YkPsQ%3D%3D%2C%20slblWhmVC8ViR3qetpM4dw%3D%3D%2C%20xrYGqTI4Hs1DdfyZ5Yx27w%3D%3D%2C%20VjZkcoaZbgPN8byHaDILuA%3D%3D%2C%20p4Jq2SVzcMwCfYiMGWuigg%3D%3D%2C%20rMFYfXpdb3PLXvjNNOBgrw%3D%3D%5Cr%5CnX-Cache%3A%20TCP_MISS%20from%20a119-224-129-198.deploy.akamaitechnologies.com%20(AkamaiGHost%2F7.3.2.2-15906379)%20(-)%5Cr%5Cnx-cache-key%3A%20S%2FL%2F1791%2F98030%2F14d%2Fphoto.facebook.com%2Ftest-80KB.jpg%5Cr%5Cnx-cache-remote%3A%20TCP_HIT%20from%20a119-224-129-207.deploy.akamaitechnologies.com%20(AkamaiGHost%2F7.3.2.2-15906379)%20(-)%5Cr%5Cnx-check-cacheable%3A%20YES%5Cr%5Cnx-serial%3A%201791%5Cr%5Cnx-true-cache-key%3A%20%2FL%2Fphoto.facebook.com%2Ftest-80KB.jpg%5Cr%5CnX-Firefox-Spdy%3A%203.1%5Cr%5Cn%22%2C%22status%22%3A200%7D%5D%7D",
            //   "GET",
            //    "",
            //    false,
            //   "");


            respHtml = Form1.weLoveYue(
                form1,
                "https://www.facebook.com/login.php?login_attempt=1&lwv=110",
                "POST",
                "https://www.facebook.com/",
                false,
                "lsd=" + token +
                "&email=" + username +
                "&pass=" + password +
                "&default_persistent=0&timezone=-720" +
                "&lgndim=eyJ3IjoxNDQwLCJoIjo5MDAsImF3IjoxNDQwLCJhaCI6ODA1LCJjIjoyNH0%3D" +
                "&lgnrnd=" + lgnrnd +
                "&lgnjs=1442408093&locale=en_US&qsstamp=W1tbMjAsMjMsMzAsMzEsOTYsMTE3LDEyNiwxMjgsMTM4LDE1MywxODUsMTg2LDE4NywyMTIsMjIyLDI0MywyNDcsMjY5LDI3NywyODQsMjg3LDMxMiwzMTMsMzUzLDM4MCwzOTQsNDE2LDQzOSw0NjAsNDY4LDQ5MCw0OTEsNDk4LDUxMyw1MjEsNTQzLDU0OCw1NjMsNTg1LDYwNSw2MjcsODg5XV0sIkFabW9wclU0QTBjdXFFWWdNYlFPQ19hRklCdHNfWWZXMjA4MFRTSVIyNF9lcUVsa3k3aE04YUx1WmpsZFAxUTNPZWl6LU5tZEpUcXNuRHZaN0lXU2hwc05Ba0VYZXNnN0NRRXdNdGZ4Yl9NQy0wNVg3aThybDhSTUNubTRPaWVBbWZrUmRqeUlXZzNMRGhPd0oxazBwWlNkZnhBSENwdllUd3RGTGlDUUNRMDBGUlVNSTNndTVfOEJyZ1cwTE51dWJCV2pRVFpkdFlxWTJjekVOSHFjUi0zRlJCQTk3UmczRjdKQWRWUXJYREhPZ2pZVjNWdHkyNzRUWm5tMTM3QWN5R0EiXQ%3D%3D",
               ref cookieContainer,
                true);


            if (respHtml.Equals("Found"))
            {
                form1.setLogT("login succeed");
                Form1.gLoginOkFlag = true;
                return 1;

            }
            if (respHtml.Contains("Incorrect email or phone number")
                || respHtml.Contains("It looks like you entered a slight misspelling of your email or username")
                || respHtml.Contains("The email you entered does not belong to any account")
            )
            {
                form1.setLogT("username error!");
                return -1;
            }
            else if (respHtml.Contains("The password you entered is incorrect"))
            {
                form1.setLogT("password error!");
                return -1;
            }
            else if (respHtml.Contains("Please enable cookies in your browser preferences to continue"))
            {
                form1.setLogT("cookies error!");
                return -1;
            }

            return -2;
        }



        public void loginT()
        {
            if (!Form1.debug)
            {
                username = form1.inputT.Text.Replace("@", "%40");
                password = form1.textBox1.Text;
                if (form1.inputT.Text.Equals("") || form1.textBox1.Text.Equals(""))
                {
                    form1.setLogT("please enter username and password");
                    return;
                }
            }

            while (true)
            {
                if (form1.rate.Text.Equals(""))
                {
                    Thread.Sleep(500);
                }
                else if (Convert.ToInt32(form1.rate.Text) > 0)
                {
                    Thread.Sleep(Convert.ToInt32(form1.rate.Text));
                }
                else
                {
                    Thread.Sleep(500);
                }

                int r = loginF();
                if (r == -3)
                {
                    continue;
                }
                if (r != -2)
                {
                    break;
                }

            }
        }




    }
}
