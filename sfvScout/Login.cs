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

        //Rayflo Di710463     (yls7104@gmail.com)
        //dudeea Dd123456
        //tadarcy Zxc123456  -- 有线下付款表
        //Asdmo 废
        //hansha Dd123456 di710463
        //eru1989 Dd123456
        //daiyyr D 0

        //有表格后不能再创建表格, 但是同一张表可能会被不同的程序多次支付,所以支付的每个环节都需要去查看表格状态

        public Client Client;

        public Login(Form1 f, Client client)
        {
            form1 = f;
            Client = client;
        }


        static string rgx;
        static Match myMatch;

        Form1 form1;


        public int obtainLoginPage()
        {
            form1.setLogT(Client.FamilyName + " " + Client.GivenName + " " + Client.PassportNo + ": obtainLoginPage..");
            string respHtml = "";

            /*
          respHtml = Form1.weLoveYue(
              form1,
              "https://www.immigration.govt.nz/secure/Login+Working+Holiday.htm",
              "GET",
              "",
              true,
              "",
             ref cookieContainer,
              true);
            //     cookieContainer.Add(new Cookie("TS0120d49b", "0152807fb20d92231761cb749be9bf0c068e6b51b7b7c8b3ca3163b50d0ded4393ddab932f0e53bfd0276edcf78ed51aeba4f9a69be4cf15ca8c961e1184690d83fa9fded1") { Domain = "www.immigration.govt.nz" });
       //   cookieContainer.Add(new Cookie("ASP.NET_SessionId", "vr5pmzbicspjp455xatxmcmk") { Domain = "www.immigration.govt.nz" });
    
          //     cookieContainer.Add(new Cookie("BIGipServerwww.immigration.govt.nz", "342776330.20480.0000") { Domain = "www.immigration.govt.nz" });
            */


            HttpWebResponse resp = null;
            Client.cookieContainer = null;

            obtainLoginPage:

            if (true) //(Client.cookieContainer == null || Client.cookieContainer.Count == 0)
            {

                resp = Form1.weLoveYueer(
                form1,
                "https://onlineservices.immigration.govt.nz/secure/Login+Working+Holiday.htm",
            //    "https://www.immigration.govt.nz/secure/Login+Working+Holiday.htm",
                "POST",
                "https://onlineservices.immigration.govt.nz/secure/Login+Working+Holiday.htm",
                false,

                //"&TS0120d49b_cr=08eba48ebbab28003238e681cda9bcdb55fb8668c04df479fb10d8abfc03767f1e29dd"
                    //+ "c98c63655c45544f7c3017e8a208a31e8d5e894800b1f1ba4a307fa5e66367f309e83050399eeb03126a5ccd8d6d47cb838f19f033c01b6797071278b6a0f3675c971bb86c9a33cf630623fb90ced6a4a3b20b3e59b1ed1519a48b7fff"
                    //+
                    //"TS0120d49b_id=3&TS0120d49b_76=0&TS0120d49b_86=0&TS0120d49b_md=1&TS0120d49b_rf=0&TS0120d49b_ct=0&TS0120d49b_pd=0",

                "TS8e49d4_id=3&TS8e49d4_md=1&TS8e49d4_rf=0&TS8e49d4_ct=0&TS8e49d4_pd=0",
                    //this code comes from yuejie@20160320

                ref Client.cookieContainer
                );
            }
            else
            {
                resp = Form1.weLoveYueer(
                form1,
                "https://onlineservices.immigration.govt.nz/secure/Login+Working+Holiday.htm",
                "GET",
                "https://onlineservices.immigration.govt.nz/secure/Login+Working+Holiday.htm",
                false,
                "",

                ref Client.cookieContainer
                );

            }
            

            if (resp == null)
            {
                goto obtainLoginPage;
            }

            if (resp.StatusCode == HttpStatusCode.Found)
            {
                form1.setLogT("getting login page failed!");
                return -1;
            }

            respHtml = Form1.resp2html(resp);

            rgx = @"(?<=name=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                Client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            else
            {
                form1.setLogT("getting login page failed!");
                return -1;
            }

            rgx = @"(?<=var __CMS_CurrentUrl = "")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                Client.__CMS_CurrentUrl = myMatch.Groups[0].Value;
            }
            else
            {
                form1.setLogT("getting login page failed!");
                return -1;
            }

            rgx = @"(?<=name=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                Client.__VIEWSTATEGENERATOR = myMatch.Groups[0].Value;
            }
            else
            {
                form1.setLogT("getting login page failed!");
                return -1;
            } 


    //            cookieContainer.Add(new Cookie("_js_datr", datr) { Domain = "www.facebook.com" });

            //to get cookies: datr
            // weLoveMuYue("https://www.facebook.com/hellocdn/results?data=%7B%22results%22%3A%5B%7B%22loading_time%22%3A0%2C%22platform%22%3A%22www%22%2C%22cdn%22%3A%22ak%22%2C%22resource_timing%22%3A%7B%22name%22%3A%22https%3A%2F%2Ffbcdn-photos-b-a.akamaihd.net%2Fhphotos-ak-prn1%2Ftest-80KB.jpg%22%2C%22entryType%22%3A%22resource%22%2C%22startTime%22%3A307.131182%2C%22duration%22%3A422.90310700000003%2C%22initiatorType%22%3A%22xmlhttprequest%22%2C%22redirectStart%22%3A0%2C%22redirectEnd%22%3A0%2C%22fetchStart%22%3A307.131182%2C%22domainLookupStart%22%3A307.131182%2C%22domainLookupEnd%22%3A307.131182%2C%22connectStart%22%3A307.131182%2C%22connectEnd%22%3A307.131182%2C%22secureConnectionStart%22%3A0%2C%22requestStart%22%3A308.652099%2C%22responseStart%22%3A398.19683299999997%2C%22responseEnd%22%3A730.0342890000001%7D%2C%22url%22%3A%22https%3A%2F%2Ffbcdn-photos-b-a.akamaihd.net%2Fhphotos-ak-prn1%2Ftest-80KB.jpg%22%2C%22headers%22%3A%22Access-Control-Allow-Origin%3A%20*%5Cr%5CnCache-Control%3A%20no-transform%2C%20max-age%3D8%5Cr%5CnContent-Length%3A%2079957%5Cr%5CnContent-Type%3A%20image%2Fjpeg%5Cr%5CnDate%3A%20Sat%2C%2019%20Sep%202015%2003%3A55%3A59%20GMT%5Cr%5CnExpires%3A%20Sat%2C%2019%20Sep%202015%2003%3A56%3A07%20GMT%5Cr%5CnLast-Modified%3A%20Fri%2C%2012%20Dec%202014%2000%3A53%3A28%20GMT%5Cr%5CnServer%3A%20proxygen%5Cr%5CnTiming-Allow-Origin%3A%20*%5Cr%5Cnx-akamai-session-info%3A%20name%3DBEGIN_CLOCK%3B%20value%3D1435761000%2C%20name%3DCLOCK_DURATION%3B%20value%3D6873959%2C%20name%3DFB_DISABLE_FULL_HTTPS%3B%20value%3Dtrue%2C%20name%3DFB_DISABLE_FULL_LOGGING%3B%20value%3Dtrue%2C%20name%3DFB_LOGGING_URL_SAMPLE%3B%20value%3Dtrue%2C%20name%3DFULL_PATH_KEY%3B%20value%3Dfalse%2C%20name%3DHSAFSERIAL%3B%20value%3D842%2C%20name%3DNOW_CLOCK%3B%20value%3D1442634959%2C%20name%3DORIGIN%3B%20value%3Dhphotos-ak-prn1%2C%20name%3DOVERRIDE_HTTPS_IE_CACHE_BUST%3B%20value%3Dall%2C%20name%3DSERIALNEXT%3B%20value%3D1791%2C%20name%3DSINGLE_TIER%3B%20value%3Dtrue%2C%20name%3DSINGLE_TIER_HVAL%3B%20value%3D789613%2C%20name%3DVALIDORIGIN%3B%20value%3Dtrue%3B%20full_location_id%3Dmetadata%5Cr%5Cnx-akamai-ssl-client-sid%3A%20B2VGSAuDC%2BONy6lq7deAkQ%3D%3D%2C%20jXyFJAJ1swG8eI9JJLO85A%3D%3D%2C%20eHGuYL6ebCGxtQ%2FFzTWoqQ%3D%3D%2C%20xTmDZCNM%2BaM1EFSiyU%2B5PQ%3D%3D%2C%2000ViDZZBGURh2d4RBXYqtA%3D%3D%2C%20ldw%2F1r4y03Q8umDIRzyoDw%3D%3D%2C%20QoOhGh3xuf88M%2BjTOOnWfg%3D%3D%2C%20Z8kyY5MKFQLQt3zz2YkPsQ%3D%3D%2C%20slblWhmVC8ViR3qetpM4dw%3D%3D%2C%20xrYGqTI4Hs1DdfyZ5Yx27w%3D%3D%2C%20VjZkcoaZbgPN8byHaDILuA%3D%3D%2C%20p4Jq2SVzcMwCfYiMGWuigg%3D%3D%2C%20rMFYfXpdb3PLXvjNNOBgrw%3D%3D%5Cr%5CnX-Cache%3A%20TCP_MISS%20from%20a119-224-129-198.deploy.akamaitechnologies.com%20(AkamaiGHost%2F7.3.2.2-15906379)%20(-)%5Cr%5Cnx-cache-key%3A%20S%2FL%2F1791%2F98030%2F14d%2Fphoto.facebook.com%2Ftest-80KB.jpg%5Cr%5Cnx-cache-remote%3A%20TCP_HIT%20from%20a119-224-129-207.deploy.akamaitechnologies.com%20(AkamaiGHost%2F7.3.2.2-15906379)%20(-)%5Cr%5Cnx-check-cacheable%3A%20YES%5Cr%5Cnx-serial%3A%201791%5Cr%5Cnx-true-cache-key%3A%20%2FL%2Fphoto.facebook.com%2Ftest-80KB.jpg%5Cr%5CnX-Firefox-Spdy%3A%203.1%5Cr%5Cn%22%2C%22status%22%3A200%7D%5D%7D",
            //   "GET",
            //    "",
            //    false,
            //   "");
            Client.nextStep = "login";
            login();
            return 1;
        }

        public int login()
        {
            form1.setLogT(Client.FamilyName + " " + Client.GivenName + " " + Client.PassportNo + ": login..");

            HttpWebResponse resp = null;

            login:
            resp = Form1.weLoveYueer(
                form1,
                "https://onlineservices.immigration.govt.nz" + Client.__CMS_CurrentUrl,
                "POST",
                "https://onlineservices.immigration.govt.nz/secure/Login+Working+Holiday.htm",
                false,
                "__EVENTTARGET="+
                    "&__EVENTARGUMENT="+
                    "&__VIEWSTATEGENERATOR=" + Client.__VIEWSTATEGENERATOR +
                    "&HeaderCommunityHomepage%3ASearchControl%3AtxtSearchString="+
                    "&VisaDropDown=%2Fsecure%2FLogin%2BWorking%2BHoliday.htm"+
                    "&OnlineServicesLoginStealth%3AVisaLoginControl%3AuserNameTextBox="+ Form1.ToUrlEncode( Client.UserName ) +
                    "&OnlineServicesLoginStealth%3AVisaLoginControl%3ApasswordTextBox="+ Form1.ToUrlEncode( Client.Password ) +
                    "&OnlineServicesLoginStealth%3AVisaLoginControl%3AloginImageButton.x=42"+
                    "&OnlineServicesLoginStealth%3AVisaLoginControl%3AloginImageButton.y=10"+
                    "&__VIEWSTATE=" + Client.__VIEWSTATE,
               ref Client.cookieContainer
                );

            if (resp == null)
            {
                goto login;
            }

            if (resp.StatusCode == HttpStatusCode.Found)
            {
                form1.setLogT("login succeed");
                Form1.gLoginOkFlag = true;
                Client.nextStep = "createNewFormPage";
                return 1;

            }
            string respHtml = Form1.resp2html(resp);

            if (respHtml.Contains("Incorrect email or phone number")
                || respHtml.Contains("It looks like you entered a slight misspelling of your email or username")
                || respHtml.Contains("The email you entered does not belong to any account")
            )
            {
                form1.setLogT(Client.FamilyName + " " + Client.GivenName + " " + Client.PassportNo + ": username error!");
                return -1;
            }
            else if (respHtml.Contains("The password you entered is incorrect") || respHtml.Contains("You have entered an incorrect user name or password"))
            {
                form1.setLogT(Client.FamilyName + " " + Client.GivenName + " " + Client.PassportNo + ": password error!");
                return -1;
            }
            else if (respHtml.Contains("Please enable cookies in your browser preferences to continue"))
            {
                form1.setLogT(Client.FamilyName + " " + Client.GivenName + " " + Client.PassportNo + ": cookies error!");
                return -1;
            }

            return -2;
        }




        public int loginT()
        {
            /*
            if (!Form1.debug)
            {
                Client.UserName = form1.inputT.Text.Replace("@", "%40");
                Client.Password = form1.textBox1.Text;
                if (form1.inputT.Text.Equals("") || form1.textBox1.Text.Equals(""))
                {
                    form1.setLogT("please enter username and password");
                    return;
                }
            }
            */
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

                int r = obtainLoginPage();
                if (r == -3 || r == -2)
                {
                    continue;
                }
               
                return r;

            }
            
        }




    }
}
