using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Net.Cache;
using System.Text.RegularExpressions;
namespace widkeyPaperDiaper
{
    public class Apply
    {
        static string rgx;
        static Match myMatch;

        string verificationCode;

        Mail163<Apply> mail;
        Client client;

        string sizeType = null;
        string TS0120d49b_cr = "";

        public List<string> gFriends = new List<string>();

        string gFileName = null;

        string eventId = "";
        Form1 form1;
        string keyURL;
        bool requireVeriCode = false;
        string countyForMulti, shopForMulti;


        string minDate = DateTime.Now.ToString("%d", DateTimeFormatInfo.InvariantInfo) + "+" +
                            DateTime.Now.ToString("MMM", DateTimeFormatInfo.InvariantInfo) + "+" +
                            (DateTime.Now.Year - 31).ToString();
        string now = DateTime.Now.ToString("%d", DateTimeFormatInfo.InvariantInfo) + "+" +
                            DateTime.Now.ToString("MMM", DateTimeFormatInfo.InvariantInfo) + "+" +
                            DateTime.Now.ToString("yyyy", DateTimeFormatInfo.InvariantInfo);
        string maxDate = DateTime.Now.ToString("%d", DateTimeFormatInfo.InvariantInfo) + "+" +
                            DateTime.Now.ToString("MMM", DateTimeFormatInfo.InvariantInfo) + "+" +
                            (DateTime.Now.Year + 25).ToString();
        string oneYearLater = DateTime.Now.ToString("%d", DateTimeFormatInfo.InvariantInfo) + "+" +
                            DateTime.Now.ToString("MMM", DateTimeFormatInfo.InvariantInfo) + "+" +
                            (DateTime.Now.Year + 1).ToString();

        public Apply(Form1 f, Client cli)
        {
            form1 = f;
            client = cli;
        }

        public Apply(Form1 f, Client app, Mail163<Apply> em, string countyForMulti, string shopForMulti, string eventId, bool requireVeriCode)
        {
            form1 = f;
            client = app;
            mail = em;
            sizeType = f.selectedType;
            this.requireVeriCode = requireVeriCode;
            this.countyForMulti =countyForMulti;
            this.shopForMulti = shopForMulti;
            this.eventId = eventId;
        }

        public int writeResult(string content)
        {
            if (gFriends.Count > 0)
            {
                if (gFileName == null)
                {
                    gFileName = "save_" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", DateTimeFormatInfo.InvariantInfo) + ".txt";
                    form1.setLogT("Create file: " + System.Environment.CurrentDirectory + "\\" + gFileName);
                }
                Form1.writeFile(System.Environment.CurrentDirectory + "\\" + gFileName, content);

            }
            return 1;
        }


        public void showNextAppTime()
        {
            string county = form1.selecteCounty.Shops[form1.selectedShop];        //big surprise!!! the official entrance is the encoding with s-jis of county's name, BUT, shop's name also works! And now I use shop's name with others' will narely do

            string shop = form1.selecteCounty.Sids[form1.selectedShop];
            string forTest = Form1.ToUrlEncode(
                        county,
                        System.Text.Encoding.GetEncoding("shift-jis")
                     );

            DateTime dateTime = DateTime.MinValue;
            string day = "";
            string time = "";
            
            string html = Form1.weLoveYue(
                form1,
                "http://aksale.advs.jp/cp/akachan_sale_pc/search_event_list.cgi?area2=" 
                + Form1.ToUrlEncode(
                        county, 
                        System.Text.Encoding.GetEncoding("shift-jis")
                     )
                + "&event_type=" + sizeType + "&sid=" + shop + "&kmws=",

                "GET", "", false, "", ref  client.cookieContainer,
                false
                );

            //available 
            //   <th>予約受付期間</th>
            //					<td>
            //						10/12<font color="#ff0000">(月)</font>&nbsp;13:30～10/12<font color="#ff0000">(月)</font>&nbsp;22:00<br />

            if (county == form1.selecteCounty.Shops[form1.selectedShop] 
                && shop == form1.selecteCounty.Sids[form1.selectedShop]
                && sizeType == form1.selectedType)
            {
                rgx = @"(?<=<th>予約受付期間</th>\n.*\n\s*)\d+\/\d+(?=\D)";
                myMatch = (new Regex(rgx)).Match(html);
                while (myMatch.Success)
                {
                    day = myMatch.Groups[0].Value;// no available appointment
                    rgx = @"(?<=<th>予約受付期間</th>\n.*\n\s*" + day + @"(\s|\S)+?)\d+\:\d+(?=\D)";
                    Match match2 = (new Regex(rgx)).Match(html);
                    if (match2.Success)
                    {
                        time = match2.Groups[0].Value;

                        DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                        dtFormat.ShortDatePattern = "yyyy-M-d hh:mm:ss";
                        dateTime = Convert.ToDateTime("2015-" + Regex.Match(day, @"\d+(?=\/)") + "-" + Regex.Match(day, @"(?<=\/)\d+") + " " + time + ":00");
                        

                        //how to find the year ?

                        TimeZoneInfo jst = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                        TimeZoneInfo cst = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                        DateTime nowInTokyoTime = TimeZoneInfo.ConvertTime(DateTime.Now, jst);
                        if ((dateTime - nowInTokyoTime).TotalMinutes > -15)
                        {
                            delegate2 d111 = new delegate2(
                                delegate() {
                                    form1.label14.Text = "the nearest booking on type " + (sizeType == "6" ? "M" : "L") + " in " + form1.selecteCounty.Name + " " + county
                                    + " is on: \n" + dateTime.ToString("MM/dd HH:mm") + " Tokyo Standard Time\n"
                                    + TimeZoneInfo.ConvertTimeFromUtc(TimeZoneInfo.ConvertTimeToUtc(dateTime, jst), cst).ToString("MM/dd HH:mm")
                                    + " China Standard Time"
                                    ;
                                 }
                            );
                            form1.label14.Invoke(d111);
                            return;
                        }
                    }
                    myMatch = myMatch.NextMatch();
                }
                delegate2 d222 = new delegate2(
                    delegate() {
                        if (Regex.Match(html, @"条件に一致する予約販売が存在しません").Success)
                        {
                            form1.label14.Text = "There is no type " + (sizeType == "6" ? "M" : "L") + " in " + form1.selecteCounty.Name + " " + county;
                        }
                        else
                        {
                            form1.label14.Text = "No available booking these days on type " + (sizeType == "6" ? "M" : "L") + " in " + form1.selecteCounty.Name + " " + county;
                        }
                    }
                );
                form1.label14.Invoke(d222);
            }//end of if the search option not changed
        }

        //big surprise!!! the official entrance is the encoding with s-jis of county's name, BUT, shop's name also works! And now I use shop's name with others' will narely do
        //And the response body between them are 3 lines:

        //<a href="http://aksale.advs.jp/cp/akachan_sale_pc/search_event_detail.cgi?event_id=9782391339&area1=&area2=北海道&area3=&event_open_date=201510&kmws=">
        //<a href="http://aksale.advs.jp/cp/akachan_sale_pc/search_event_detail.cgi?event_id=9782391339&area1=&area2=旭川店&area3=&event_open_date=201510&kmws=">
		
        //<a href="http://aksale.advs.jp/cp/akachan_sale_pc/search_event_detail.cgi?event_id=5637179822&area1=&area2=北海道&area3=&event_open_date=201510&kmws=">
        //<a href="http://aksale.advs.jp/cp/akachan_sale_pc/search_event_detail.cgi?event_id=5637179822&area1=&area2=旭川店&area3=&event_open_date=201510&kmws=">
				
		//<a href="./search_shop_list.cgi?event_type=6&area1=&area2=%96k%8aC%93%b9&area3=&kmws=">戻る</a>
		//<a href="./search_shop_list.cgi?event_type=6&area1=&area2=%88%ae%90%ec%93X&area3=&kmws=">戻る</a>








        public int createNewFormPage()
        {
            createForm:
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": createNewForm..");
            foreach (Cookie ck in client.cookieContainer)
            {
                if (ck.Name == "TS0120d49b")
                {
                    TS0120d49b_cr = ck.Value;
                    break;
                }
            }
            string respHtml;

            /*
            //default page
            if (TS0120d49b_cr == "")
            {
                respHtml = Form1.weLoveYue(
              form1,
              "http://www.immigration.govt.nz/migrant/default.htm",
                "GET",
              "",
              false,
              "",

             ref client.cookieContainer,
              true);
            }
            else
            {
                respHtml = Form1.weLoveYue(
              form1,
              "http://www.immigration.govt.nz/migrant/default.htm",
              "POST",
              "",
              false,

              "&TS0120d49b_cr=" + TS0120d49b_cr +
                "&TS0120d49b_id=3" +
                "&TS0120d49b_76=0" +
                "&TS0120d49b_86=0" +
                "&TS0120d49b_md=1" +
                "&TS0120d49b_rf=0" +
                "&TS0120d49b_ct=0" +
                "&TS0120d49b_pd=0",

             ref client.cookieContainer,
              true);
            }
            */

            //create form page
            if (TS0120d49b_cr == "")
            {
                respHtml = Form1.weLoveYue(
              form1,
              "https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Create.aspx?CountryId=" + (Form1.debug || Form1.testButton ? "82" : "46"), //82 for Germany, 46 for China
              "GET",
              "https://www.immigration.govt.nz/secure/Login+Working+Holiday.htm",
              false,
              "",

             ref client.cookieContainer,
              true);
            }
            else
            {
                respHtml = Form1.weLoveYue(
              form1,
              "https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Create.aspx?CountryId=" + (Form1.debug || Form1.testButton ? "82" : "46"), //82 for Germany, 46 for China
              "POST",
              "https://www.immigration.govt.nz/secure/Login+Working+Holiday.htm",
              false,

              "&TS0120d49b_cr=" + TS0120d49b_cr +
                "&TS0120d49b_id=3" +
                "&TS0120d49b_76=0" +
                "&TS0120d49b_86=0" +
                "&TS0120d49b_md=1" +
                "&TS0120d49b_rf=0" +
                "&TS0120d49b_ct=0" +
                "&TS0120d49b_pd=0",

             ref client.cookieContainer,
              true);
            }

            if (respHtml == "")
            {
                goto createForm;
            }


            if (!respHtml.Contains("ctl00_ContentPlaceHolder1_applyNowButton"))
            {
                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": quotas for china do not open yet, retry...");
                return -1;
            }

            rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            else
            {
                form1.setLogT("getting __VIEWSTATE failed!");
                return -1;
            }

            rgx = @"(?<=id=""__EVENTVALIDATION"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__EVENTVALIDATION = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            else
            {
                form1.setLogT("getting __EVENTVALIDATION failed!");
                return -1;
            }

            rgx = @"(?<=id=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATEGENERATOR = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            else
            {
                form1.setLogT("getting __VIEWSTATEGENERATOR failed!");
                return -1;
            }
            client.nextStep = "clickCreateNow";
            clickCreateNow();
            return 1;
        }



        public int clickCreateNow(){


            string respHtml;
            HttpWebResponse resp = null;

            clickCreateNow:
            resp= Form1.weLoveYueer(
                form1,
                "https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Create.aspx?CountryId=" + (Form1.debug || Form1.testButton ? "82" : "46"), //82 for Germany, 46 for China
                "POST",
                "https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Create.aspx?CountryId=" + (Form1.debug || Form1.testButton ? "82" : "46"), //82 for Germany, 46 for China
                false,
                "__EVENTVALIDATION=" + client.__EVENTVALIDATION +
                "&__VIEWSTATE=" + client.__VIEWSTATE +
                "&__VIEWSTATEGENERATOR=" + client.__VIEWSTATEGENERATOR +
                "&ctl00%24ContentPlaceHolder1%24applyNowButton.x=35" + 
                "&ctl00%24ContentPlaceHolder1%24applyNowButton.y=7",
            ref client.cookieContainer
                );

            if (resp == null)
            {
                goto clickCreateNow;
            }

            respHtml = Form1.resp2html(resp);

            //点击create now时显示已有表格
            if (respHtml.Contains("Multiple applications are not supported"))
            {
                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": The form exists!");
                client.nextStep = "obtainStatus";
                obtainStatus();
                return 2;
            }

            //新建表格成功, 从found响应的跳转地址里找到ApplicationId号
            if (resp.StatusCode == HttpStatusCode.Found)
            {

                rgx = @"(?<=ApplicationId=)\d+?(?=$)";
                myMatch = (new Regex(rgx)).Match(resp.Headers["location"]);
                if (myMatch.Success)
                {
                    client.ApplicationId = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }

                respHtml = Form1.weLoveYue(
                              form1,
                              "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Personal1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                              "POST",
                              "https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Create.aspx?CountryId=82",
                              false,
                              "TS8e49d4_id=3&TS8e49d4_md=1&TS8e49d4_rf=0&TS8e49d4_ct=0&TS8e49d4_pd=0",
                             ref client.cookieContainer,
                             true);



                rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }
                else
                {
                    form1.setLogT("出现js加密页!");
                    return -1;
                }

                rgx = @"(?<=id=""__EVENTVALIDATION"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.__EVENTVALIDATION = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }
                else
                {
                    form1.setLogT("出现js加密页!");
                    return -1;
                }

                rgx = @"(?<=id=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.__VIEWSTATEGENERATOR = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }
                else
                {
                    form1.setLogT("出现js加密页!");
                    return -1;
                }

                rgx = @"(?<=ApplicationId=)\d+?(?=&)";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.ApplicationId = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }
                else
                {
                    form1.setLogT("出现js加密页!");
                    return -1;
                }

                rgx = @"(?<=id=""ctl00_ContentPlaceHolder1_personDetails_dateOfBithDatePicker_ControlState"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.ctl00_ContentPlaceHolder1_personDetails_dateOfBithDatePicker_ControlState = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }
                else
                {
                    form1.setLogT("clickCreateNow failed!");
                    return -1;
                }
            }

            //如果有已完成的页面, 则可以跳页, 图标: ../Images/Tabcontrol/icon_tick.gif    待完善 daiyyr
            client.nextStep = "personalDetails";
            personalDetails();
            return 1;
        }
        public int deleteForms()
        {
            deleteForms:
            form1.setLogT("delete form begins, detect application ID..");
            string respHtml = Form1.weLoveYue(
              form1,
              "https://www.immigration.govt.nz/WORKINGHOLIDAY/default.aspx",
              "GET",
              "https://www.immigration.govt.nz/secure/Login+Working+Holiday.htm",
              false,
              "",
             ref client.cookieContainer,
              true);

            if (respHtml == "")
            {
                goto deleteForms;
            }

            if (respHtml.Contains("ctl00_ContentPlaceHolder1_applicationList_applicationsDataGrid_ctl02_deleteHyperlink"))
            {
                rgx = @"(?<=id=""ctl00_ContentPlaceHolder1_applicationList_applicationsDataGrid_ctl02_deleteHyperlink"" href=""Application\/Delete\.aspx\?ApplicationId=)\d+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.ApplicationId = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }
                else
                {
                    form1.setLogT("Status page changed!");
                    return -3;
                }
                respHtml = Form1.weLoveYue(
                          form1,
                          "https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Delete.aspx?ApplicationId=" + client.ApplicationId,
                          "POST",
                          "https://www.immigration.govt.nz/WORKINGHOLIDAY/default.aspx",
                          false,
                          "TS8e49d4_id=3&TS8e49d4_md=1&TS8e49d4_rf=0&TS8e49d4_ct=0&TS8e49d4_pd=0",
                         ref client.cookieContainer,
                         true);

                rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }

                rgx = @"(?<=id=""__EVENTVALIDATION"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.__EVENTVALIDATION = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }

                rgx = @"(?<=id=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.__VIEWSTATEGENERATOR = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }

                respHtml = Form1.weLoveYue(
                          form1,
                          "https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Delete.aspx?ApplicationId=" + client.ApplicationId,
                          "POST",
                          "https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Delete.aspx?ApplicationId=" + client.ApplicationId,
                          false,
                          "__EVENTVALIDATION=" + client.__EVENTVALIDATION +
                            "&__VIEWSTATE=" + client.__VIEWSTATE +
                            "&__VIEWSTATEGENERATOR=" + client.__VIEWSTATEGENERATOR +
                            "&ctl00%24ContentPlaceHolder1%24okDeleteButton.x=59&ctl00%24ContentPlaceHolder1%24okDeleteButton.y=1",
                         ref client.cookieContainer,
                         true);
                if (respHtml.Contains("The application has been successfully deleted."))
                {
                    form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": The application has been successfully deleted.");
                }
                
            }
            else
            {
                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": The application do not exist.");
            }
                return 1;
        }
        public int obtainStatus()
        {
            string respHtml = Form1.weLoveYue(
              form1,
              "https://www.immigration.govt.nz/WORKINGHOLIDAY/default.aspx",
              "GET",
              "https://www.immigration.govt.nz/secure/Login+Working+Holiday.htm",
              false,
              "",
             ref client.cookieContainer,
              true);

            //status页显示有未完成的表格
            if (respHtml.Contains("ctl00_ContentPlaceHolder1_applicationList_applicationsDataGrid_ctl02_editHyperLink"))
            {
                rgx = @"(?<=id=""ctl00_ContentPlaceHolder1_applicationList_applicationsDataGrid_ctl02_editHyperLink"" href=""Application\/Edit\.aspx\?ApplicationId=)\d+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.ApplicationId = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }
                else
                {
                    form1.setLogT("Status page changed!");
                    return -3;
                }

               respHtml = Form1.weLoveYue(
                          form1,
                          "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Personal1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                          "POST",
                          "https://www.immigration.govt.nz/WORKINGHOLIDAY/default.aspx",
                          false,
                          "TS8e49d4_id=3&TS8e49d4_md=1&TS8e49d4_rf=0&TS8e49d4_ct=0&TS8e49d4_pd=0",
                         ref client.cookieContainer,
                         true);

                rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }

                rgx = @"(?<=id=""__EVENTVALIDATION"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.__EVENTVALIDATION = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }

                rgx = @"(?<=id=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.__VIEWSTATEGENERATOR = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }

                rgx = @"(?<=id=""ctl00_ContentPlaceHolder1_personDetails_dateOfBithDatePicker_ControlState"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.ctl00_ContentPlaceHolder1_personDetails_dateOfBithDatePicker_ControlState = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }

                client.nextStep = "personalDetails";
                personalDetails();
                return 2;
            }

            // pay
            if (respHtml.Contains("ctl00_ContentPlaceHolder1_applicationList_applicationsDataGrid_ctl02_payHyperLink"))
            {
                rgx = @"(?<=id=""ctl00_ContentPlaceHolder1_applicationList_applicationsDataGrid_ctl02_payHyperLink"" href=""Application\/Pay\.aspx\?ApplicationId=)\d+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.ApplicationId = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }

                client.nextStep = "pay";
                pay();
                return 3;
            }

            return 1;
        }

        public int personalDetails()
        {
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": personal 1...");

            string respHtml = Form1.weLoveYue(
                form1,
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Personal1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Personal1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                false,
                "ctl00%24ContentPlaceHolder1%24personDetails%24familyNameTextBox="+ client.FamilyName.ToUpper() +
                "&ctl00%24ContentPlaceHolder1%24personDetails%24givenName1Textbox="+ client.GivenName.ToUpper() +
                "&ctl00%24ContentPlaceHolder1%24personDetails%24genderDropDownList=" + client.Gender.ToUpper() +
                "&ctl00_ContentPlaceHolder1_personDetails_dateOfBithDatePicker_Day=" + client.BithDateDay +
                "&ctl00_ContentPlaceHolder1_personDetails_dateOfBithDatePicker_Month=" + client.BithDateMonth +
                "&ctl00_ContentPlaceHolder1_personDetails_dateOfBithDatePicker_Year=" + client.BithDateYear +
                "&ctl00%24ContentPlaceHolder1%24personDetails%24dateOfBithDatePicker=" +
                "&ctl00_ContentPlaceHolder1_personDetails_dateOfBithDatePicker_MaxDate=" + now +
                "&ctl00_ContentPlaceHolder1_personDetails_dateOfBithDatePicker_MinDate=" + minDate +
                "&ctl00_ContentPlaceHolder1_personDetails_dateOfBithDatePicker_ControlState=" + client.ctl00_ContentPlaceHolder1_personDetails_dateOfBithDatePicker_ControlState +
                "&ctl00%24ContentPlaceHolder1%24personDetails%24CountryDropDownList=" + (Form1.debug || Form1.testButton ? "82" : "46") + //82 for Germany, 46 for China
                "&ctl00%24ContentPlaceHolder1%24addressContactDetails%24address%24address1TextBox=" + Form1.ToUrlEncode(client.Street) +
                "&ctl00%24ContentPlaceHolder1%24addressContactDetails%24address%24cityTextBox=" + client.City +
                "&ctl00%24ContentPlaceHolder1%24addressContactDetails%24address%24countryDropDownList=" + (Form1.debug || Form1.testButton ? "82" : "46") + //82 for Germany, 46 for China
                "&ctl00%24ContentPlaceHolder1%24addressContactDetails%24contactDetails%24emailAddressTextBox=" + client.Email.Replace("@","%40") +
                "&ctl00%24ContentPlaceHolder1%24hasAgent%24representedByAgentDropdownlist=No"+
                "&ctl00%24ContentPlaceHolder1%24communicationMethod%24communicationMethodDropDownList=1"+
                "&ctl00%24ContentPlaceHolder1%24hasCreditCard%24hasCreditCardDropDownlist=Yes"+
                "&ctl00%24ContentPlaceHolder1%24wizardPageFooter%24wizardPageNavigator%24validateButton.x=56"+
                "&ctl00%24ContentPlaceHolder1%24wizardPageFooter%24wizardPageNavigator%24validateButton.y=6"+
                "&__EVENTVALIDATION=" + client.__EVENTVALIDATION +
                "&__VIEWSTATE=" + client.__VIEWSTATE +
                "&__VIEWSTATEGENERATOR=" + client.__VIEWSTATEGENERATOR,

                ref client.cookieContainer,
                true
          );

            client.nextStep = "identificationDetails";
            identificationDetails();

            return 1;
        }


        public int identificationDetails()
        {
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": personal 2...");

            //必须先拿到person2这一页的viewstate及其generator, 否则提交的数据不会被保存
            string respHtml;
            respHtml = Form1.weLoveYue(
                form1,
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Personal2.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Personal1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                false,
                "TS8e49d4_id=3&TS8e49d4_md=1&TS8e49d4_rf=0&TS8e49d4_ct=0&TS8e49d4_pd=0",
                ref client.cookieContainer,
                true);

            rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            rgx = @"(?<=id=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATEGENERATOR = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            //本页无 __EVENTVALIDATION


            respHtml = Form1.weLoveYue(
                form1,
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Personal2.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Personal2.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                false,
                "&ctl00%24ContentPlaceHolder1%24identification%24confirmPassportNumberTextBox=" + client.PassportNo +
                "&ctl00%24ContentPlaceHolder1%24identification%24passportNumberTextBox=" + client.PassportNo +
                "&ctl00_ContentPlaceHolder1_identification_passportExpiryDateDatePicker_Day=" + client.PassportExpiryDateDay +
                "&ctl00_ContentPlaceHolder1_identification_passportExpiryDateDatePicker_Month=" + client.PassportExpiryDateMonth +
                "&ctl00_ContentPlaceHolder1_identification_passportExpiryDateDatePicker_Year=" + client.PassportExpiryDateYear +
                "&ctl00%24ContentPlaceHolder1%24identification%24passportExpiryDateDatePicker=" +
                "&ctl00_ContentPlaceHolder1_identification_passportExpiryDateDatePicker_MaxDate=" + maxDate +
                "&ctl00_ContentPlaceHolder1_identification_passportExpiryDateDatePicker_MinDate=" + now +
                "&ctl00_ContentPlaceHolder1_identification_passportExpiryDateDatePicker_ControlState=%2FwEXBQUMU2VsZWN0ZWREYXRlBgAAAAAAAAAABQxQcmV2aW91c0RhdGUGAAAAAAAAAAAFB01heERhdGUGAHjRulpd74gFEFNlbGVjdGVkRGF0ZVRleHQFBTAtMC0wBQdNaW5EYXRlBgA47vAuVtOI" +
                "&ctl00%24ContentPlaceHolder1%24identification%24otherIdentificationDropdownlist=3" +
                "&ctl00_ContentPlaceHolder1_identification_otherIssueDateDatePicker_Day=" + client.NationalIdIssueDateDay +
                "&ctl00_ContentPlaceHolder1_identification_otherIssueDateDatePicker_Month=" + client.NationalIdIssueDateMonth + 
                "&ctl00_ContentPlaceHolder1_identification_otherIssueDateDatePicker_Year=" + client.NationalIdIssueDateYear +
                "&ctl00%24ContentPlaceHolder1%24identification%24otherIssueDateDatePicker=" +
                "&ctl00_ContentPlaceHolder1_identification_otherIssueDateDatePicker_MaxDate=" + now +
                "&ctl00_ContentPlaceHolder1_identification_otherIssueDateDatePicker_MinDate=" + minDate +
                "&ctl00_ContentPlaceHolder1_identification_otherIssueDateDatePicker_ControlState=%2FwEXBQUMU2VsZWN0ZWREYXRlBgAAAAAAAAAABQxQcmV2aW91c0RhdGUGAAAAAAAAAAAFB01heERhdGUGADju8C5W04gFEFNlbGVjdGVkRGF0ZVRleHQFBTAtMC0wBQdNaW5EYXRlBgD4jv2HlLCI" +
                "&ctl00_ContentPlaceHolder1_identification_otherExpiryDateDatePicker_Day=" + client.NationalIdExpiryDateDay +
                "&ctl00_ContentPlaceHolder1_identification_otherExpiryDateDatePicker_Month=" + client.NationalIdExpiryDateMonth +
                "&ctl00_ContentPlaceHolder1_identification_otherExpiryDateDatePicker_Year=" + client.NationalIdExpiryDateYear +
                "&ctl00%24ContentPlaceHolder1%24identification%24otherExpiryDateDatePicker=" +
                "&ctl00_ContentPlaceHolder1_identification_otherExpiryDateDatePicker_MaxDate=" + maxDate +
                "&ctl00_ContentPlaceHolder1_identification_otherExpiryDateDatePicker_MinDate" + now +
                "&ctl00_ContentPlaceHolder1_identification_otherExpiryDateDatePicker_ControlState=%2FwEXBQUMU2VsZWN0ZWREYXRlBgAAAAAAAAAABQxQcmV2aW91c0RhdGUGAAAAAAAAAAAFB01heERhdGUGAHjRulpd74gFEFNlbGVjdGVkRGF0ZVRleHQFBTAtMC0wBQdNaW5EYXRlBgA47vAuVtOI" +
                "&ctl00%24ContentPlaceHolder1%24identification%24passportCountryDropDownList=" + (Form1.debug || Form1.testButton ? "82" : "46") + //82 for Germany, 46 for China
                "&ctl00%24ContentPlaceHolder1%24wizardPageFooter%24wizardPageNavigator%24validateButton.x=19" +
                "&ctl00%24ContentPlaceHolder1%24wizardPageFooter%24wizardPageNavigator%24validateButton.y=3" +
                "&__VIEWSTATE=" + client.__VIEWSTATE +
                "&__VIEWSTATEGENERATOR=" + client.__VIEWSTATEGENERATOR +
                "&__EVENTARGUMENT=" +
                "&__EVENTTARGET=",

                ref client.cookieContainer,
                true
          );

            client.nextStep = "medical";
            medical();

            return 1;

        }


        public int medical()
        {
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": medical...");

            string respHtml;
            respHtml = Form1.weLoveYue(
                form1,
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Medical1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Personal2.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                false,
                "TS8e49d4_id=3&TS8e49d4_md=1&TS8e49d4_rf=0&TS8e49d4_ct=0&TS8e49d4_pd=0",
                ref client.cookieContainer,
                true);

            rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            rgx = @"(?<=id=""__EVENTVALIDATION"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__EVENTVALIDATION = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            rgx = @"(?<=id=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATEGENERATOR = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }

            respHtml = Form1.weLoveYue(
                form1,
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Medical1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Medical1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                false,
                "ctl00%24ContentPlaceHolder1%24medicalConditions%24renalDialysisDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24tuberculosisDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24cancerDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24heartDiseaseDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24disabilityDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24hospitalisationDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24residentailCareDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24pregnancy%24pregnancyStatusDropDownList=No" +
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24tbRiskDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24wizardPageFooter%24wizardPageNavigator%24validateButton.x=53"+
                "&ctl00%24ContentPlaceHolder1%24wizardPageFooter%24wizardPageNavigator%24validateButton.y=2" +
                "&__EVENTVALIDATION=" + client.__EVENTVALIDATION +
                "&__VIEWSTATE=" + client.__VIEWSTATE +
                "&__VIEWSTATEGENERATOR=" + client.__VIEWSTATEGENERATOR,

                ref client.cookieContainer,
                true
          );


            client.nextStep = "character";
            character();

            return 1;

        }

        public int character()
        {
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": character...");

            string respHtml;
            respHtml = Form1.weLoveYue(
                form1,
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Character.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Medical1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                false,
                "TS8e49d4_id=3&TS8e49d4_md=1&TS8e49d4_rf=0&TS8e49d4_ct=0&TS8e49d4_pd=0",
                ref client.cookieContainer,
                true);

            rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            rgx = @"(?<=id=""__EVENTVALIDATION"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__EVENTVALIDATION = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            rgx = @"(?<=id=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATEGENERATOR = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }

            respHtml = Form1.weLoveYue(
                form1,
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Character.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Character.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                false,
                "ctl00%24ContentPlaceHolder1%24character%24imprisonment5YearsDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24character%24imprisonment12MonthsDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24character%24removalOrderDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24character%24deportedDropDownList=No"+
                "&ctl00_ContentPlaceHolder1_character_deportedDateDatePicker_Day=0"+
                "&ctl00_ContentPlaceHolder1_character_deportedDateDatePicker_Month=0"+
                "&ctl00_ContentPlaceHolder1_character_deportedDateDatePicker_Year=0"+
                "&ctl00%24ContentPlaceHolder1%24character%24deportedDateDatePicker="+
                "&ctl00_ContentPlaceHolder1_character_deportedDateDatePicker_MaxDate="+ now +
                "&ctl00_ContentPlaceHolder1_character_deportedDateDatePicker_MinDate="+ minDate +
                "&ctl00_ContentPlaceHolder1_character_deportedDateDatePicker_ControlState=%2FwEXBQUMU2VsZWN0ZWREYXRlBgAAAAAAAAAABQxQcmV2aW91c0RhdGUGAAAAAAAAAAAFB01heERhdGUGALho7%2BVa04gFEFNlbGVjdGVkRGF0ZVRleHQFBTAtMC0wBQdNaW5EYXRlBgB4Cfw%2BmbCI"+
                "&ctl00%24ContentPlaceHolder1%24character%24countryDropDownList="+
                "&ctl00%24ContentPlaceHolder1%24character%24chargedDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24character%24convictedDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24character%24underInvestigationDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24character%24excludedDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24character%24removedDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24character%24excludeRemovedDetailsTextbox="+
                "&ctl00%24ContentPlaceHolder1%24wizardPageFooter%24wizardPageNavigator%24validateButton.x=24"+
                "&ctl00%24ContentPlaceHolder1%24wizardPageFooter%24wizardPageNavigator%24validateButton.y=8"+
                "&__EVENTVALIDATION=" + client.__EVENTVALIDATION +
                "&__VIEWSTATE=" + client.__VIEWSTATE +
                "&__VIEWSTATEGENERATOR=" + client.__VIEWSTATEGENERATOR,

                ref client.cookieContainer,
                true
          );


            client.nextStep = "workingHolidaySpecific";
            workingHolidaySpecific();

            return 1;

        }
        

        public int workingHolidaySpecific()
        {
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": workingHolidaySpecific...");

            string respHtml;
            respHtml = Form1.weLoveYue(
                form1,
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/WorkingHolidaySpecific.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/Character.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                false,
                "TS8e49d4_id=3&TS8e49d4_md=1&TS8e49d4_rf=0&TS8e49d4_ct=0&TS8e49d4_pd=0",
                ref client.cookieContainer,
                true);

            rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            rgx = @"(?<=id=""__EVENTVALIDATION"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__EVENTVALIDATION = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            rgx = @"(?<=id=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATEGENERATOR = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }

            respHtml = Form1.weLoveYue(
                form1,
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/WorkingHolidaySpecific.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/WorkingHolidaySpecific.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                false,
                "ctl00%24ContentPlaceHolder1%24offshoreDetails%24commonWHSQuestions%24previousWhsPermitVisaDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24offshoreDetails%24commonWHSQuestions%24sufficientFundsHolidayDropDownList=Yes"+
                "&ctl00_ContentPlaceHolder1_offshoreDetails_intendedTravelDateDatePicker_Day=" + client.IntendedTravelDateDay +
                "&ctl00_ContentPlaceHolder1_offshoreDetails_intendedTravelDateDatePicker_Month=" + client.IntendedTravelDateMonth +
                "&ctl00_ContentPlaceHolder1_offshoreDetails_intendedTravelDateDatePicker_Year="+ client.IntendedTravelDateYear +
                "&ctl00%24ContentPlaceHolder1%24offshoreDetails%24intendedTravelDateDatePicker="+
                "&ctl00_ContentPlaceHolder1_offshoreDetails_intendedTravelDateDatePicker_MaxDate="+ oneYearLater +
                "&ctl00_ContentPlaceHolder1_offshoreDetails_intendedTravelDateDatePicker_MinDate="+ now +
                "&ctl00_ContentPlaceHolder1_offshoreDetails_intendedTravelDateDatePicker_ControlState=%2FwEXBQUMU2VsZWN0ZWREYXRlBgAAAAAAAAAABQxQcmV2aW91c0RhdGUGAAAAAAAAAAAFB01heERhdGUGAODzyb951IgFEFNlbGVjdGVkRGF0ZVRleHQFBTAtMC0wBQdNaW5EYXRlBgC4aO%2FlWtOI"+
                "&ctl00%24ContentPlaceHolder1%24offshoreDetails%24beenToNzDropDownList=" + (client.BeenToNz == "Yes" ? "Yes" : "No") +
                "&ctl00_ContentPlaceHolder1_offshoreDetails_whenInNZDatePicker_Day=0"+ (client.BeenToNz == "Yes" ? client.BeenToNzDateDay : "0") +
                "&ctl00_ContentPlaceHolder1_offshoreDetails_whenInNZDatePicker_Month=0" + (client.BeenToNz == "Yes" ? client.BeenToNzDateMonth : "0") +
                "&ctl00_ContentPlaceHolder1_offshoreDetails_whenInNZDatePicker_Year=0" + (client.BeenToNz == "Yes" ? client.BeenToNzDateYear : "0") +
                "&ctl00%24ContentPlaceHolder1%24offshoreDetails%24whenInNZDatePicker="+
                "&ctl00_ContentPlaceHolder1_offshoreDetails_whenInNZDatePicker_MaxDate="+ now +
                "&ctl00_ContentPlaceHolder1_offshoreDetails_whenInNZDatePicker_MinDate="+ minDate +
                "&ctl00_ContentPlaceHolder1_offshoreDetails_whenInNZDatePicker_ControlState=%2FwEXBQUMU2VsZWN0ZWREYXRlBgAAAAAAAAAABQxQcmV2aW91c0RhdGUGAAAAAAAAAAAFB01heERhdGUGALho7%2BVa04gFEFNlbGVjdGVkRGF0ZVRleHQFBTAtMC0wBQdNaW5EYXRlBgB4Cfw%2BmbCI"+
                "&ctl00%24ContentPlaceHolder1%24offshoreDetails%24requirementsQuestions%24sufficientFundsOnwardTicketDropDownList=Yes"+
                "&ctl00%24ContentPlaceHolder1%24offshoreDetails%24requirementsQuestions%24readRequirementsDropDownList=Yes"+
                "&ctl00%24ContentPlaceHolder1%24wizardPageFooter%24wizardPageNavigator%24validateButton.x=14"+
                "&ctl00%24ContentPlaceHolder1%24wizardPageFooter%24wizardPageNavigator%24validateButton.y=5"+
                "&__EVENTVALIDATION=" + client.__EVENTVALIDATION +
                "&__VIEWSTATE=" + client.__VIEWSTATE +
                "&__VIEWSTATEGENERATOR=" + client.__VIEWSTATEGENERATOR,

                ref client.cookieContainer,
                true
          );


            client.nextStep = "submit";
            submit();

            return 1;

        }


        public int submit()
        {
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": submit...");

            string respHtml;
            respHtml = Form1.weLoveYue(
                form1,
                "https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Submit.aspx?ApplicationId=" + client.ApplicationId,
                "POST",
                "https://www.immigration.govt.nz/WorkingHoliday/Wizard/WorkingHolidaySpecific.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                false,
                "TS8e49d4_id=3&TS8e49d4_md=1&TS8e49d4_rf=0&TS8e49d4_ct=0&TS8e49d4_pd=0",
                ref client.cookieContainer,
                true);

            rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            rgx = @"(?<=id=""__EVENTVALIDATION"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__EVENTVALIDATION = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            rgx = @"(?<=id=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATEGENERATOR = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }

            respHtml = Form1.weLoveYue(
                form1,
                "https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Submit.aspx?ApplicationId=" + client.ApplicationId,
                "POST",
                "https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Submit.aspx?ApplicationId=" + client.ApplicationId,
                false,
                "ctl00%24ContentPlaceHolder1%24falseStatementCheckBox=on"+
                "&ctl00%24ContentPlaceHolder1%24notesCheckBox=on"+
                "&ctl00%24ContentPlaceHolder1%24circumstancesCheckBox=on"+
                "&ctl00%24ContentPlaceHolder1%24warrantsCheckBox=on"+
                "&ctl00%24ContentPlaceHolder1%24informationCheckBox=on"+
                "&ctl00%24ContentPlaceHolder1%24healthCheckBox=on"+
                "&ctl00%24ContentPlaceHolder1%24adviceCheckBox=on"+
                "&ctl00%24ContentPlaceHolder1%24registrationCheckBox=on"+
                "&ctl00%24ContentPlaceHolder1%24entitlementCheckbox=on"+
                "&ctl00%24ContentPlaceHolder1%24permitExpiryCheckBox=on"+
                "&ctl00%24ContentPlaceHolder1%24medicalInsuranceCheckBox=on"+
                "&ctl00%24ContentPlaceHolder1%24submitImageButton.x=45"+
                "&ctl00%24ContentPlaceHolder1%24submitImageButton.y=7" +
                "&__EVENTVALIDATION=" + client.__EVENTVALIDATION +
                "&__VIEWSTATE=" + client.__VIEWSTATE +
                "&__VIEWSTATEGENERATOR=" + client.__VIEWSTATEGENERATOR,

                ref client.cookieContainer,
                true
          );

            rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            rgx = @"(?<=id=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATEGENERATOR = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }

            client.nextStep = "pay";
            pay();

            return 1;

        }

        public int pay()
        {

            lock (form1.rate) //防止本地其它线程支付
            {
                form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": pay...");

                //get https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Pay.aspx?ApplicationId=1451998
                //有收到cookie, 也许需要

                string respHtml;
                respHtml = Form1.weLoveYue(
                    form1,
                    "https://www.immigration.govt.nz/PaymentGateway/OnLinePayment.aspx?SourceUrl=https://www.immigration.govt.nz/WorkingHoliday/Application/SubmitConfirmation.aspx?ApplicationId=" + client.ApplicationId + "&ApplicationId=" + client.ApplicationId + "&ProductId=2",
                    "POST",
                    "https://www.immigration.govt.nz/PaymentGateway/OnLinePayment.aspx?SourceUrl=https://www.immigration.govt.nz/WorkingHoliday/Application/SubmitConfirmation.aspx?ApplicationId=" + client.ApplicationId + "&ApplicationId=" + client.ApplicationId + "&ProductId=2",
                    false,
                    "TS8e49d4_id=3&TS8e49d4_md=1&TS8e49d4_ct=0&TS8e49d4_pd=0" +
                    "&TS8e49d4_rf=" + Form1.ToUrlEncode("https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Pay.aspx?ApplicationId=" + client.ApplicationId),
                    ref client.cookieContainer,
                    true);

                rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }
                rgx = @"(?<=id=""__EVENTVALIDATION"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.__EVENTVALIDATION = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }
                rgx = @"(?<=id=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.__VIEWSTATEGENERATOR = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }

                HttpWebResponse resp = null;

            PaymentGateway:
                //如果不使用信用卡余额控制支付, 则最好每次提交都去确认一下beingPaid参数. 那么就必须要有clientId(8位随机字符)来识别不同线程创建的client. 
                //于是, 支付函数中的每一次提交之前,都去判断一下client列表,如果(护照号和这个client相同, clientId不同, beingPaid为true) 那么直接return, 同一台机器上不同的进程也可以适用该规则
                //至于这个client列表, 对应的是一个本地文件, 主键是clientId, 有一个进程不停地同步服务器上的client和本地client.
                resp = Form1.weLoveYueer(
                    form1,
                    "https://www.immigration.govt.nz/PaymentGateway/OnLinePayment.aspx?SourceUrl=https%3a%2f%2fwww.immigration.govt.nz%2fWorkingHoliday%2fApplication%2fSubmitConfirmation.aspx%3fApplicationId%3d" + client.ApplicationId + "&ApplicationId=" + client.ApplicationId + "&ProductId=2",
                    "POST",
                    "https://www.immigration.govt.nz/WorkingHoliday/Wizard/WorkingHolidaySpecific.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                    false,
                    "__EVENTTARGET="+
                    "&__EVENTARGUMENT=" +
                    "&__VIEWSTATE=" + client.__VIEWSTATE +
                    "&__VIEWSTATEGENERATOR=" + client.__VIEWSTATEGENERATOR +
                    "&__EVENTVALIDATION=" + client.__EVENTVALIDATION +
                    "&ctl00%24ContentPlaceHolder1%24payorNameTextBox=" + client.PayerName +
                    "&ctl00%24ContentPlaceHolder1%24okImageButton.x=39"+
                    "&ctl00%24ContentPlaceHolder1%24okImageButton.y=12" 
                    ,

                    ref client.cookieContainer
              );
                //也许不需要这一步 daiyyr
                /*
                resp = Form1.weLoveYueer(
                    form1,
                    "https://www.immigration.govt.nz/PaymentGateway/OnLinePayment.aspx?SourceUrl=https%3a%2f%2fwww.immigration.govt.nz%2fWorkingHoliday%2fApplication%2fSubmitConfirmation.aspx%3fApplicationId%3d" + client.ApplicationId + "&ApplicationId="+ client.ApplicationId +"&ProductId=2",
                    "POST",
                    "https://www.immigration.govt.nz/PaymentGateway/OnLinePayment.aspx?SourceUrl=https%3a%2f%2fwww.immigration.govt.nz%2fWorkingHoliday%2fApplication%2fSubmitConfirmation.aspx%3fApplicationId%3d" + client.ApplicationId + "&ApplicationId=" + client.ApplicationId + "&ProductId=2",
                    false,
                    "TS8e49d4_id=3&TS8e49d4_md=2&TS8e49d4_ct=application%2Fx-www-form-urlencoded" +
                    "&TS8e49d4_rf=" + Form1.ToUrlEncode("https://www.immigration.govt.nz/WORKINGHOLIDAY/Application/Pay.aspx?ApplicationId=" + client.ApplicationId )+
                    
                    //值内部的符号经历两次URL转换, 比如, % 转为 %25
                    "&TS8e49d4_pd=" +
                    Form1.ToUrlEncode(
                        "__EVENTTARGET=" +
                        "&__EVENTARGUMENT=" +
                        "&__VIEWSTATE=" + client.__VIEWSTATE +
                        "&__VIEWSTATEGENERATOR=" + client.__VIEWSTATEGENERATOR +
                        "&__EVENTVALIDATION=" + client.__EVENTVALIDATION +
                        "&ctl00%24ContentPlaceHolder1%24payorNameTextBox=" + client.PayerName +
                        "&ctl00%24ContentPlaceHolder1%24okImageButton.x=39" +
                        "&ctl00%24ContentPlaceHolder1%24okImageButton.y=12"
                        )
                    ,
                    ref client.cookieContainer
                    );

                 */

                if (resp == null)
                {
                    goto PaymentGateway;
                }
                if (resp.StatusCode == HttpStatusCode.Found)
                {
                    if (resp.Headers["location"].Contains("paymark"))
                    {
                        form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": 支付请求成功推送到paymark...");

                        //注意host改变
                        respHtml = Form1.weLoveYue(
                            form1,
                            resp.Headers["location"],
                            "GET",
                            "https://www.immigration.govt.nz/PaymentGateway/OnLinePayment.aspx?SourceUrl=https%3a%2f%2fwww.immigration.govt.nz%2fWorkingHoliday%2fApplication%2fSubmitConfirmation.aspx%3fApplicationId%3d" + client.ApplicationId + "&ApplicationId=" + client.ApplicationId + "&ProductId=2",
                            false,
                            "",
                            ref client.cookieContainer,
                            "webcomm.paymark.co.nz",
                            true
                      );

                        //不进行URL code转换
                        string rm = "";
                        rgx = @"(?<=name=""payment_type_selection"" method=""POST"" action=""\?rm=)\d+?(?="")";
                        myMatch = (new Regex(rgx)).Match(respHtml);
                        if (myMatch.Success)
                        {
                            rm = myMatch.Groups[0].Value;
                        }
                        string hk = "";
                        rgx = @"(?<=id=""hk"" value="")(\s|\S)+?(?="")";
                        myMatch = (new Regex(rgx)).Match(respHtml);
                        if (myMatch.Success)
                        {
                            hk = myMatch.Groups[0].Value;
                        }


                        respHtml = Form1.weLoveYue(
                            form1,
                            "https://webcomm.paymark.co.nz/hosted/?rm=" + rm,
                            "POST",
                            resp.Headers["location"],
                            false,
                            "hk=" + hk +
                            "&hosted_responsive_format=N" +
                            (client.CardType.ToUpper() == "VISA" ? 
                                "&card_type_VISA.x=45&card_type_VISA.y=37" 
                                : "&card_type_MASTERCARD.x=34&card_type_MASTERCARD.y=27"
                            ) +
                            "&processingStage=card_entry" +
                            "&future_pay=" +
                            "&future_pay_save_only=",
                            ref client.cookieContainer,
                            "webcomm.paymark.co.nz",
                            true
                      );

                        string rm2 = "";
                        rgx = @"(?<=\&rm=)\d+?(?="")";
                        myMatch = (new Regex(rgx)).Match(respHtml);
                        if (myMatch.Success)
                        {
                            rm2 = myMatch.Groups[0].Value;

                            string hkc = "";
                            rgx = @"(?<=\?hkc=)(\s|\S)+?(?=\&rm)";
                            myMatch = (new Regex(rgx)).Match(respHtml);
                            if (myMatch.Success)
                            {
                                hkc = myMatch.Groups[0].Value;
                            }


                        finalPay:
                            resp = Form1.weLoveYueer(
                                form1,
                                "https://webcomm.paymark.co.nz/hosted/?hkc=" + hkc + "&rm=" + rm2,
                                "POST",
                                "https://webcomm.paymark.co.nz/hosted/?rm=" + rm,
                                false,
                                "cardnumber=" + client.CardNumber +
                                "&use_card_security_code=Y" +
                                "&enforce_card_security_code=N" +
                                "&enforce_card_security_code_3party=Y" +
                                "&enforce_card_security_code_futurepay=Y" +
                                "&cardverificationcode=" + client.CardVerificationCode +
                                "&expirymonth=" + client.CardExpiryMonth +
                                "&expiryyear=" + client.CardExpiryYear +
                                "&hk=" + hk +
                                "&hosted_responsive_format=N" +
                                "&cardtype=" + (client.CardType.ToUpper() == "VISA" ? "VISA" : "MASTERCARD" )+
                                "&future_pay=N" +
                                "&future_pay_save_only=" +
                                "&cardholder=" + client.CardHolder +
                                "&pay_now=Pay+Now)",

                                ref client.cookieContainer,
                                "webcomm.paymark.co.nz"
                                );

                            if (resp == null)
                            {
                                goto finalPay;
                            }

                            if (resp.StatusCode == HttpStatusCode.Found)
                            {
                                string processAdreess = resp.Headers["location"];
                            waitForProcess:
                                resp = Form1.weLoveYueer(
                                    form1,
                                    processAdreess,
                                    "GET",
                                    "https://webcomm.paymark.co.nz/hosted/?rm=" + rm,
                                    false,
                                    "",
                                    ref client.cookieContainer,
                                    "payments.paystation.co.nz"
                                );
                                respHtml = Form1.resp2html(resp);
                                if (respHtml.Contains("Please wait while your payment is processed") || respHtml == "")
                                {
                                    goto waitForProcess;
                                }
                                if (respHtml.Contains("Your payment was not successful"))
                                {
                                    string message = "";
                                    rgx = @"(?<=\>Your payment was not successful\<\/p\>\s+?\<p class\=""failSub""\>)(\s|\S)+?(?=\<\/p\>)";
                                    myMatch = (new Regex(rgx)).Match(respHtml);
                                    if (myMatch.Success)
                                    {
                                        message = myMatch.Groups[0].Value;
                                        form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ", Payment was not successful: " + message);
                                        return -6;
                                    }
                                }
                                else if(resp.StatusCode == HttpStatusCode.Found)
                                {
                                    respHtml = Form1.weLoveYue(
                                        form1,
                                        resp.Headers["location"],
                                        "GET",
                                        "https://webcomm.paymark.co.nz/hosted/?rm=" + rm,
                                        false,
                                        "",
                                        ref client.cookieContainer,
                                        "payments.paystation.co.nz",
                                        true
                                        );
                                }
                                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": Congratulation! See you in NZ!");
                                
                                //下载成功网页
                                form1.downloadHtml("successPage.passportNo" + client.PassportNo, respHtml);

                                return 1;
                            }
                            else
                            {
                                string message = "";
                                rgx = @"(?<=\<h3 id=""messageStep2""\>2\: Enter Your Card Details\<\/h3\>\s+?\<h3\>)(\s|\S)+?(?=\<\/h3\>)";
                                myMatch = (new Regex(rgx)).Match(respHtml);
                                if (myMatch.Success)
                                {
                                    message = myMatch.Groups[0].Value;
                                    form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ", Invalid card information: " + message);
                                    return -6;
                                }
                                else 
                                {
                                    form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ", Invalid card information");
                                }
                            }
                            
                        }


                            //提示信息直接显示网页的报错内容
                            //因为不验证成功的情况, 关键支付页检查cookies
                    }
                }
                else
                {
                    respHtml = Form1.resp2html(resp);
                    goto PaymentGateway;

                }

            
            }
            

            return 1;
        }












        public void searchMailDirectely()
        {
            string keyURL = mail.queery("ご注文予約案内", @"https://aksale(\s|\S)+?(?=\r)");
            if (keyURL == null | keyURL == "")
            {
                form1.setLogT("NULL url from email");
            }
            setAppointment(mail.address, keyURL);
        }

        public void searchMailDirectelyFromReaded()
        {
            string keyURL = mail.queeryReaded("ご注文予約案内", @"https://aksale(\s|\S)+?(?=\r)");
            if (keyURL == null | keyURL == "")
            {
                form1.setLogT("NULL url from email");
            }
            setAppointment(mail.address, keyURL);
        }

        public int setAppointment(string email, string url){

            form1.setLogT("CardNo" + ", start setting appointment from " + email);

            //https://aksale.advs.jp/cp/akachan_sale_pc/reg?id=fJrKmJSkXbhtEuAEFWSpeZJ4lfwyV93d
            string result = Form1.weLoveMuYue(
                form1,
                url,
                "GET",
                "https://aksale.advs.jp/cp/akachan_sale_pc/mail_form.cgi",
                true,
                "",
                ref client.cookieContainer
                );


            if (!result.Contains("Found"))
            {
                form1.setLogtRed("error code from page whose url("+ url +") getting in" + email);
      //          return -1;
            }
            string id = Regex.Match(url, @"(?<=id=).+").Value;
            string html =  Form1.weLoveYue(
                form1,
                "https://aksale.advs.jp/cp/akachan_sale_pc/form_card_no.cgi",
                "POST",
                "https://aksale.advs.jp/cp/akachan_sale_pc/_reg_form.cgi?id=" + id, //fJrKmJSkXbhtEuAEFWSpeZJ4lfwyV93d",
                false,
                "card_no=" + "&sbmt=%8E%9F%82%D6",
                ref client.cookieContainer,
                false
                );

            if (html.Contains("恐れ入りますが、もう一度最初から操作してください"))
            {
                form1.setLogtRed("no available quota, url from: " + email);
                return -2;
            }
            if (html.Contains("正しいカード番号を入力してください"))
            {
                form1.setLogtRed("invalid cardNo: " );
                return -3;
            }
            if (html.Contains("DB接続に失敗しました"))
            {
                form1.setLogtRed("oops, something wrong from url in: " + email);
                return -4;
            }
            
            html = Form1.weLoveYue(
                form1,
                "https://aksale.advs.jp/cp/akachan_sale_pc/reg_form_event_1.cgi"
                ,
                "POST",
                "https://aksale.advs.jp/cp/akachan_sale_pc/form_card_no.cgi",
                false,
                ""
                /*
                "password="+client.CardPassword
                +"&sei="+ Form1.ToUrlEncode(client.ChineseName.Substring(0, 1), System.Text.Encoding.GetEncoding("shift-jis"))
                +"&mei="+ Form1.ToUrlEncode(client.ChineseName.Substring(1, client.ChineseName.Length - 1), System.Text.Encoding.GetEncoding("shift-jis"))
                +"&sei_kana="+Form1.ToUrlEncode(client.JapaneseName.Substring(0, 1), System.Text.Encoding.GetEncoding("shift-jis"))
                +"&mei_kana="+Form1.ToUrlEncode(client.JapaneseName.Substring(1, client.JapaneseName.Length - 1), System.Text.Encoding.GetEncoding("shift-jis"))
                +"&tel1="+Regex.Match(client.Phone, @"\d+(?=\-)").Value
                +"&tel2="+Regex.Match(client.Phone, @"(?<=\d+\-)\d+(?=-)").Value
                +"&tel3="+Regex.Match(client.Phone, @"(?<=\d+\-\d+\-)\d+").Value
                */


                //"&sei=%9B%C1&mei=%94%F2%94%F2&sei_kana=%83T&mei_kana=%83C%83q%83q&tel1=090&tel2=8619&tel3=3569"

                +"&sbmt=%8E%9F%82%D6",
                ref client.cookieContainer,
                false
                );

            html = Form1.weLoveYue(
                form1,
                "https://aksale.advs.jp/cp/akachan_sale_pc/reg_confirm_event.cgi"
                ,
                "POST",
                "https://aksale.advs.jp/cp/akachan_sale_pc/reg_form_event_1.cgi",
                false,
                "sbmt=%91%97%90M",
                ref client.cookieContainer,
                false
                );
            
            if(html.Contains("ご予約ありがとうございます")){
                form1.setLogT("CardNo" + ", Setting appointment succeed!");//daiyyr details
                return 1;
            }
            if (html.Contains("予約数に達したため、受付は終了いたしました"))
            {
                return 2;
            }
            return 1;
        }


        public delegate void delegate2();


        public void startProbe()
        {
            if (form1.mailGrid.InvokeRequired)
            {
                delegate2 sl = new delegate2(delegate()
                {
                    form1.deleteMail.Enabled = false;
                    form1.deleteApp.Enabled = false;
                });
                form1.mailGrid.Invoke(sl);
            }
            else
            {
                form1.deleteMail.Enabled = false;
                form1.deleteApp.Enabled = false;
            }
            form1.setLogT("start to create form for " + client.FamilyName + " " + client.GivenName + " " + client.PassportNo);
            while (true)
            {
                if (Form1.gForceToStop)
                {
                    break;
                }

                int r1 = 0;
            
                while (
                          (r1 = this.createNewFormPage()) == -1
                      )
                {
  
                }
                if (r1 == -5) //unavailable for china
                {
                    goto delay;
                }

                if (r1 > 0)
                {
                    break;
                }

        delay://不写在while的开头，避免第一次就延时
                if (form1.rate.Text.Equals(""))
                {
                    Thread.Sleep(100);
                }
                else 
                {
                    try{
                        if (Convert.ToInt32(form1.rate.Text) > 0){
                            Thread.Sleep(Convert.ToInt32(form1.rate.Text));
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(100);
                    }
                }
            }//end of while
            if (form1.mailGrid.InvokeRequired)
            {
                delegate2 sl = new delegate2(delegate()
                {
                    form1.deleteMail.Enabled = true;
                });
                form1.mailGrid.Invoke(sl);
            }
            else
            {
                form1.deleteMail.Enabled = true;
            }

            Form1.gForceToStop = false;
            return;
        }

        public void multiBook()
        {
            string respHtml;

            if (!requireVeriCode)//        daiyyr
            {
                // jump to verification
            }
            else
            {

                //post eventId to get the verification code page
                respHtml = Form1.weLoveYue(
                form1,
                "http://aksale.advs.jp/cp/akachan_sale_pc/captcha.cgi",
                "POST",
                "http://aksale.advs.jp/cp/akachan_sale_pc/search_event_list.cgi?area2=" + countyForMulti + "&event_type=" + sizeType + "&sid=" + shopForMulti + "&kmws=",
                false,
                "sbmt=%97%5C%96%F1&event_id=" + eventId + "&event_type=6",
               ref  client.cookieContainer,
                false
                );


                //show verification code

                //<img src="./captcha/144445570520561.jpeg" alt="画像認証" /><br />
                //http://aksale.advs.jp/cp/akachan_sale_pc/captcha/144445570520561.jpeg

                string cCodeGuid = "";
                rgx = @"(?<=img src=""\./captcha/)\d+?(?=\.jpeg)";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    cCodeGuid = myMatch.Groups[0].Value;
                }
                lock (form1.pictureBox1)
                {

                    if (form1.textBox2.InvokeRequired)
                    {
                        delegate2 sl = new delegate2(delegate()
                        {
                            form1.pictureBox1.ImageLocation = @"http://aksale.advs.jp/cp/akachan_sale_pc/captcha/" + cCodeGuid + ".jpeg";
                            form1.textBox2.Text = "";
                            form1.textBox2.ReadOnly = false;
                            form1.textBox2.Focus();
                            form1.label9.Text = "cardNo" ;
                            form1.label9.Visible = true;
                        });
                        form1.textBox2.Invoke(sl);
                    }
                    else
                    {
                        form1.pictureBox1.ImageLocation = @"http://aksale.advs.jp/cp/akachan_sale_pc/captcha/" + cCodeGuid + ".jpeg";
                        form1.textBox2.Text = "";
                        form1.textBox2.ReadOnly = false;
                        form1.textBox2.Focus();
                        form1.label9.Text = "cardNo" +  ":请输入验证码";
                        form1.label9.Visible = true;
                    }

                    while (form1.textBox2.Text.Length < 5)
                    {
                        Thread.Sleep(30);
                    }

                    verificationCode = form1.textBox2.Text.Substring(0, 5);
                    if (form1.textBox2.InvokeRequired)
                    {
                        delegate2 sl = new delegate2(delegate()
                        {
                            form1.textBox2.ReadOnly = true;
                            form1.pictureBox1.ImageLocation = @"";
                            form1.label9.Visible = false;
                        });
                        form1.textBox2.Invoke(sl);
                    }
                    else
                    {
                        form1.textBox2.ReadOnly = true;
                        form1.pictureBox1.ImageLocation = @"";
                        form1.label9.Visible = false;
                    }
                }// end of lock picturebox1


                //submit the veri code
                respHtml = Form1.weLoveYue(
                form1,
                "http://aksale.advs.jp/cp/akachan_sale_pc/_mail.cgi",
                "POST",
                "http://aksale.advs.jp/cp/akachan_sale_pc/captcha.cgi",
                false,
                "input_captcha=" + verificationCode + "&sbmt=%8E%9F%82%D6&event_id=" + eventId + "&event_type=" + sizeType,
               ref  client.cookieContainer,
                false
                );




                while (respHtml.Contains("captcha"))
                {
                    form1.setLogT("CardNo" +  ", 验证码错误！请重新输入");
                    rgx = @"(?<=img src=""\./captcha/)\d+?(?=\.jpeg)";
                    myMatch = (new Regex(rgx)).Match(respHtml);
                    if (myMatch.Success)
                    {
                        cCodeGuid = myMatch.Groups[0].Value;
                    }
                    lock (form1.pictureBox1)
                    {

                        if (form1.textBox2.InvokeRequired)
                        {
                            delegate2 sl = new delegate2(delegate()
                            {
                                form1.pictureBox1.ImageLocation = @"http://aksale.advs.jp/cp/akachan_sale_pc/captcha/" + cCodeGuid + ".jpeg";
                                form1.textBox2.Text = "";
                                form1.textBox2.ReadOnly = false;
                                form1.textBox2.Focus();
                                form1.label9.Text = "CardNo" +  ":请输入验证码";
                                form1.label9.Visible = true;
                            });
                            form1.textBox2.Invoke(sl);
                        }
                        else
                        {
                            form1.pictureBox1.ImageLocation = @"http://aksale.advs.jp/cp/akachan_sale_pc/captcha/" + cCodeGuid + ".jpeg";
                            form1.textBox2.Text = "";
                            form1.textBox2.ReadOnly = false;
                            form1.textBox2.Focus();
                            form1.label9.Text = "CardNo" + ":请输入验证码";
                            form1.label9.Visible = true;
                        }

                        while (form1.textBox2.Text.Length < 5)
                        {
                            Thread.Sleep(30);
                        }

                        verificationCode = form1.textBox2.Text.Substring(0, 5);
                        if (form1.textBox2.InvokeRequired)
                        {
                            delegate2 sl = new delegate2(delegate()
                            {
                                form1.textBox2.ReadOnly = true;
                                form1.pictureBox1.ImageLocation = @"";
                                form1.label9.Visible = false;
                            });
                            form1.textBox2.Invoke(sl);
                        }
                        else
                        {
                            form1.textBox2.ReadOnly = true;
                            form1.pictureBox1.ImageLocation = @"";
                            form1.label9.Visible = false;
                        }
                    }// end of lock picturebox1

                    //submit the veri code
                    respHtml = Form1.weLoveYue(
                    form1,
                    "http://aksale.advs.jp/cp/akachan_sale_pc/_mail.cgi",
                    "POST",
                    "http://aksale.advs.jp/cp/akachan_sale_pc/captcha.cgi",
                    false,
                    "input_captcha=" + verificationCode + "&sbmt=%8E%9F%82%D6&event_id=" + eventId + "&event_type=" + sizeType,
                    ref client.cookieContainer,
                    false
                );

                }//end of while wrong code 
            }//end of if need vervification code


            //post email
            respHtml = Form1.weLoveYue(
                form1,
                "https://aksale.advs.jp/cp/akachan_sale_pc/mail_form.cgi"
                ,
                "POST",
                requireVeriCode ? "http://aksale.advs.jp/cp/akachan_sale_pc/_mail.cgi" :
                ("http://aksale.advs.jp/cp/akachan_sale_pc/_mail.cgi?sbmt=%97%5C%96%F1&event_id=" + eventId + "&event_type=" + sizeType)
                ,
                false,
                "mail1=" + mail.address.Replace("@", "%40") + "&mail2=" + mail.address.Replace("@", "%40") + "&sbmt=%8E%9F%82%D6&event_id=" + eventId + "&event_type=" + sizeType,
                //    "mail1=15985830370%40163.com&mail2=15985830370%40163.com&sbmt=%8E%9F%82%D6&event_id=5393381489&event_type=6"
                ref client.cookieContainer,
                false
                );

            respHtml = Form1.weLoveYue(
                form1,
                "https://aksale.advs.jp/cp/akachan_sale_pc/mail_confirm.cgi"
                ,
                "POST",
                "https://aksale.advs.jp/cp/akachan_sale_pc/mail_form.cgi",
                false,
                "sbmt=%91%97%90M&mail1=" + mail.address.Replace("@", "%2540").Replace(".", "%252e") + "&mail2=" + mail.address.Replace("@", "%2540").Replace(".", "%252e") + "&event_id=" + eventId + "&event_type=" + sizeType,
                //    sbmt=%91%97%90M&mail1=15985830370%2540163%252ecom&mail2=15985830370%2540163%252ecom&event_id=7938283049&event_type=6
                ref client.cookieContainer,
                false
          );


            if (respHtml.Contains("下記メールアドレスにメールを送信しました"))
            {
                form1.setLogtRed("CardNo" +  ", step1 succeed, checking email: " + mail.address);
            }
            else
            {
                form1.setLogtRed("CardNo" + ", email submitting failed: " + mail.address);
                return;
            }


            keyURL = mail.queery("ご注文予約案内", @"https://aksale(\s|\S)+?(?=\r)");

            setAppointment(mail.address, keyURL);

            return;
        }


    }
}






//static method in form1 ---->  MushroomUtil  --  in environment (using namespace.MushroomUtil)

//delete grib

//adress choosen commbobox

//succesful details

//event_type=6 zhong  , event_type=7 da

//making program to get county details