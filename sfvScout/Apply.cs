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

        string Ghtml = "";


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







        public int createNewFormPage()
        {
            string respHtml;


            //for零星ONLY
            int resultO = obtainStatus();
            if (resultO != -99)//有表, obtainStatus函数处理剩余步骤,不需要本函数处理
            {
                return resultO;
            }
            //返回-99说明该用户无表, 在本函数内继续尝试建表

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
            

            /*
            //default page
            if (TS0120d49b_cr == "")
            {
                respHtml = Form1.weLoveYue(
              form1,
              "http://onlineservices.immigration.govt.nz/migrant/default.htm",
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
              "http://onlineservices.immigration.govt.nz/migrant/default.htm",
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
              "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Create.aspx?CountryId=" + (Form1.debug || Form1.testButton ? "82" : "46"), //82 for Germany, 46 for China
         //     "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Create.aspx?CountryId=104", //82 for Germany, 46 for China
              "GET",
              "https://onlineservices.immigration.govt.nz/secure/Login+Working+Holiday.htm",
              false,
              "",

             ref client.cookieContainer,
              true);
            }
            else
            {
                respHtml = Form1.weLoveYue(
              form1,
              "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Create.aspx?CountryId=" + (Form1.debug || Form1.testButton ? "82" : "46"), //82 for Germany, 46 for China
        //      "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Create.aspx?CountryId=104" , //82 for Germany, 46 for China
              "POST",
              "https://onlineservices.immigration.govt.nz/secure/Login+Working+Holiday.htm",
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

            if (respHtml.Contains("no longer a place available for you"))
            {
                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": there is no longer a place available for you, retry...");
                return -5;
            }

            rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            else
            {
                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": server response error, retry...");
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
            return clickCreateNow();
        }



        public int clickCreateNow(){


            string respHtml;
            HttpWebResponse resp = null;

            clickCreateNow:
            resp= Form1.weLoveYueer(
                form1,
                "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Create.aspx?CountryId=" + (Form1.debug || Form1.testButton ? "82" : "46"), //82 for Germany, 46 for China
        //        "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Create.aspx?CountryId=104", //82 for Germany, 46 for China
                "POST",
                "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Create.aspx?CountryId=" + (Form1.debug || Form1.testButton ? "82" : "46"), //82 for Germany, 46 for China
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
                if (obtainStatus() == -99)
                {
                    form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + "Error: can not get the form");
                }
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
                //获取表格第一个页面的参数
                respHtml = Form1.weLoveYue(
                              form1,
                              "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Personal1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                              "POST",
                              "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Create.aspx?CountryId=82",
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
                    form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": server response error, retry...");
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

            client.nextStep = "personalDetails";
            return personalDetails();
            
        }
        public int deleteForms()
        {
            deleteForms:
            form1.setLogT("delete form begins, detect application ID..");
            string respHtml = Form1.weLoveYue(
              form1,
              "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/default.aspx",
              "POST",
              "https://onlineservices.immigration.govt.nz/secure/Login+Working+Holiday.htm",
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
                          "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Delete.aspx?ApplicationId=" + client.ApplicationId,
                          "POST",
                          "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/default.aspx",
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
                    form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": server response error, retry...");
                    return -1;
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
                          "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Delete.aspx?ApplicationId=" + client.ApplicationId,
                          "POST",
                          "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Delete.aspx?ApplicationId=" + client.ApplicationId,
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
                return 1;

            }
            else
            {
                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": The application do not exist.");
                return -1;
            }
        }
        public int obtainStatus()
        {
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ":check Status...");

            string respHtml = Form1.weLoveYue(
              form1,
              "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/default.aspx",
              "POST",
              "https://onlineservices.immigration.govt.nz/secure/Login+Working+Holiday.htm",
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


            //status页显示没有表格, 此页面无法判断是否有名额
            //如果是建表前的确认(零星), 则回到建表函数, 如果是建表后查表格id,则出错.
            if (respHtml.Contains("Please select your country from the list below"))
            {
                return -99;
            }



            //status页显示表格已填写完毕,点击提交
            if (respHtml.Contains("ctl00_ContentPlaceHolder1_applicationList_applicationsDataGrid_ctl02_submitHyperlink"))
            {
                rgx = @"(?<=id=""ctl00_ContentPlaceHolder1_applicationList_applicationsDataGrid_ctl02_submitHyperlink"" href=""Application\/Submit\.aspx\?ApplicationId=)\d+?(?="")";
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
                client.nextStep = "submit";
                return submit();
            }



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
                          "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Personal1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                          "POST",
                          "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/default.aspx",
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
                    form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": server response error, retry...");
                    return -1;
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

                if (!form1.replaceInfo)
                {
                    rgx = @"class\=current.+?icon_tick\.gif";
                    myMatch = (new Regex(rgx)).Match(respHtml);
                    if (myMatch.Success) //本页图标为绿色
                    {
                        rgx = @"class\=""tabcontainer""(\s|\S)+?icon_error\.gif";
                        myMatch = (new Regex(rgx)).Match(respHtml);
                        if (!myMatch.Success) //所有图标都是绿色
                        {
                            client.nextStep = "submit";
                            return submit();
                        }
                        else
                        {
                            rgx = @"icon_error.+?\s+?.+?Health";
                            myMatch = (new Regex(rgx)).Match(respHtml);
                            if (myMatch.Success) //medical是红色
                            {
                                client.nextStep = "medical";
                                return medical();
                            }

                            rgx = @"icon_error.+?\s+?.+?Character";
                            myMatch = (new Regex(rgx)).Match(respHtml);
                            if (myMatch.Success) //Character是红色
                            {
                                client.nextStep = "character";
                                return character();
                            }

                            rgx = @"icon_error.+?\s+?.+?Working Holiday Specific";
                            myMatch = (new Regex(rgx)).Match(respHtml);
                            if (myMatch.Success) //Working Holiday Specific是红色
                            {
                                client.nextStep = "workingHolidaySpecific";
                                return workingHolidaySpecific();
                                
                            }
                        }
                    }
                }
                client.nextStep = "personalDetails";
                return personalDetails();

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
                return pay();
            }

            //contain nothing
            return -1;
        }

        public int personalDetails()
        {
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": personal 1...");


            string respHtml = Form1.weLoveYue(
                form1,
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Personal1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Personal1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
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
   //             "&ctl00%24ContentPlaceHolder1%24personDetails%24CountryDropDownList=104" + //82 for Germany, 46 for China
                "&ctl00%24ContentPlaceHolder1%24addressContactDetails%24address%24address1TextBox=" + Form1.ToUrlEncode(client.Street) +
                "&ctl00%24ContentPlaceHolder1%24addressContactDetails%24address%24cityTextBox=" + client.City +
                "&ctl00%24ContentPlaceHolder1%24addressContactDetails%24address%24countryDropDownList=" + (Form1.debug || Form1.testButton ? "82" : "46") + //82 for Germany, 46 for China
    //            "&ctl00%24ContentPlaceHolder1%24addressContactDetails%24address%24countryDropDownList=104" + //82 for Germany, 46 for China
                "&ctl00%24ContentPlaceHolder1%24addressContactDetails%24contactDetails%24emailAddressTextBox=" + client.Email.Replace("@", "%40") +
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
            return identificationDetails();

        }


        public int identificationDetails()
        {
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": personal 2...");

            //必须先拿到person2这一页的viewstate及其generator, 否则提交的数据不会被保存
            string respHtml;
            respHtml = Form1.weLoveYue(
                form1,
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Personal2.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Personal1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
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
                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": server response error, retry...");
                return -1;
            }
            rgx = @"(?<=id=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATEGENERATOR = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            //本页无 __EVENTVALIDATION

            if (!form1.replaceInfo)
            {
                rgx = @"class\=current.+?icon_tick\.gif";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success) //本页图标为绿色
                {
                    rgx = @"class\=""tabcontainer""(\s|\S)+?icon_error\.gif";
                    myMatch = (new Regex(rgx)).Match(respHtml);
                    if (!myMatch.Success) //所有图标都是绿色
                    {
                        client.nextStep = "submit";
                        return submit();
                    }
                    else
                    {
                        rgx = @"icon_error.+?\s+?.+?Health";
                        myMatch = (new Regex(rgx)).Match(respHtml);
                        if (myMatch.Success) //medical是红色
                        {
                            client.nextStep = "medical";
                            return medical();
                        }

                        rgx = @"icon_error.+?\s+?.+?Character";
                        myMatch = (new Regex(rgx)).Match(respHtml);
                        if (myMatch.Success) //Character是红色
                        {
                            client.nextStep = "character";
                            return character();
                        }

                        rgx = @"icon_error.+?\s+?.+?Working Holiday Specific";
                        myMatch = (new Regex(rgx)).Match(respHtml);
                        if (myMatch.Success) //Working Holiday Specific是红色
                        {
                            client.nextStep = "workingHolidaySpecific";
                            return workingHolidaySpecific();
                        }
                    }
                }
            }
           
            


            respHtml = Form1.weLoveYue(
                form1,
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Personal2.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Personal2.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
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
           //     "&ctl00%24ContentPlaceHolder1%24identification%24passportCountryDropDownList=104" + //82 for Germany, 46 for China
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
            return medical();

        }


        public int medical()
        {
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": medical...");

            string respHtml;
            respHtml = Form1.weLoveYue(
                form1,
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Medical1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Personal2.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
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
                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": server response error, retry...");
                return -1;
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


            if (!form1.replaceInfo)
            {
                rgx = @"class\=current.+?icon_tick\.gif";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success) //本页图标为绿色
                {
                    rgx = @"class\=""tabcontainer""(\s|\S)+?icon_error\.gif";
                    myMatch = (new Regex(rgx)).Match(respHtml);
                    if (!myMatch.Success) //所有图标都是绿色
                    {
                        client.nextStep = "submit";
                        return submit();
                    }
                    else
                    {
                        rgx = @"icon_error.+?\s+?.+?Character";
                        myMatch = (new Regex(rgx)).Match(respHtml);
                        if (myMatch.Success) //Character是红色
                        {
                            client.nextStep = "character";
                            return character();
                        }

                        rgx = @"icon_error.+?\s+?.+?Working Holiday Specific";
                        myMatch = (new Regex(rgx)).Match(respHtml);
                        if (myMatch.Success) //Working Holiday Specific是红色
                        {
                            client.nextStep = "workingHolidaySpecific";
                            return workingHolidaySpecific();
                        }
                    }
                }
            }



            respHtml = Form1.weLoveYue(
                form1,
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Medical1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Medical1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                false,
                "ctl00%24ContentPlaceHolder1%24medicalConditions%24renalDialysisDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24tuberculosisDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24cancerDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24heartDiseaseDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24disabilityDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24hospitalisationDropDownList=No"+
                "&ctl00%24ContentPlaceHolder1%24medicalConditions%24residentailCareDropDownList=No"+

                (
                    client.Gender == "M" ? "" : "&ctl00%24ContentPlaceHolder1%24medicalConditions%24pregnancy%24pregnancyStatusDropDownList=No" 
                ) +

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
            return character();


        }

        public int character()
        {
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": character...");

            string respHtml;
            respHtml = Form1.weLoveYue(
                form1,
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Character.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Medical1.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
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
                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": server response error, retry...");
                return -1;
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


            if (!form1.replaceInfo)
            {
                rgx = @"class\=current.+?icon_tick\.gif";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success) //本页图标为绿色
                {
                    
                    rgx = @"class\=""tabcontainer""(\s|\S)+?icon_error\.gif";
                    myMatch = (new Regex(rgx)).Match(respHtml);
                    if (!myMatch.Success) //所有图标都是绿色
                    {
                        client.nextStep = "submit";
                        return submit();
                    }
                    else
                    {
                        rgx = @"icon_error.+?\s+?.+?Working Holiday Specific";
                        myMatch = (new Regex(rgx)).Match(respHtml);
                        if (myMatch.Success) //Working Holiday Specific是红色
                        {
                            client.nextStep = "workingHolidaySpecific";
                            return workingHolidaySpecific();
                        }
                    }
                }
            }

            respHtml = Form1.weLoveYue(
                form1,
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Character.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Character.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
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
            return workingHolidaySpecific();


        }
        

        public int workingHolidaySpecific()
        {
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": workingHolidaySpecific...");

            string respHtml;
            respHtml = Form1.weLoveYue(
                form1,
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/WorkingHolidaySpecific.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/Character.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
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
                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": server response error, retry...");
                return -1;
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


            if (!form1.replaceInfo)
            {
                rgx = @"class\=current.+?icon_tick\.gif";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success) //本页图标为绿色
                {
                    rgx = @"class\=""tabcontainer""(\s|\S)+?icon_error\.gif";
                    myMatch = (new Regex(rgx)).Match(respHtml);
                    if (!myMatch.Success) //所有图标都是绿色
                    {
                        client.nextStep = "submit";
                        return submit();
                    }
                }
            }

            respHtml = Form1.weLoveYue(
                form1,
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/WorkingHolidaySpecific.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
                "POST",
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/WorkingHolidaySpecific.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
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
            return submit();


        }


        public int submit()
        {
            form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": submit...");

            string respHtml;
            respHtml = Form1.weLoveYue(
                form1,
                "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Submit.aspx?ApplicationId=" + client.ApplicationId,
                "POST",
                "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/WorkingHolidaySpecific.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
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
                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": server response error, retry...");
                return -1;
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
                "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Submit.aspx?ApplicationId=" + client.ApplicationId,
                "POST",
                "https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Submit.aspx?ApplicationId=" + client.ApplicationId,
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

            if (respHtml.Contains("there is no longer a place available for you"))
            {
                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": there is no longer a place available for you, retry...");
                return -5;
            }

            rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }
            else
            {
                form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": server response error, retry...");
                return -1;
            }

            rgx = @"(?<=id=""__VIEWSTATEGENERATOR"" value="")(\s|\S)+?(?="")";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                client.__VIEWSTATEGENERATOR = Form1.ToUrlEncode(myMatch.Groups[0].Value);
            }

            client.nextStep = "pay";
            return pay();


        }

        public int pay()
        {

            lock (form1.rate) //防止本地其它线程支付
            {
                form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": pay...");

                //get https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Pay.aspx?ApplicationId=1451998
                //有收到cookie, 也许需要

                string respHtml;
                respHtml = Form1.weLoveYue(
                    form1,
                    "https://onlineservices.immigration.govt.nz/PaymentGateway/OnLinePayment.aspx?SourceUrl=https://onlineservices.immigration.govt.nz/WorkingHoliday/Application/SubmitConfirmation.aspx?ApplicationId=" + client.ApplicationId + "&ApplicationId=" + client.ApplicationId + "&ProductId=2",
                    "POST",
                    "https://onlineservices.immigration.govt.nz/PaymentGateway/OnLinePayment.aspx?SourceUrl=https://onlineservices.immigration.govt.nz/WorkingHoliday/Application/SubmitConfirmation.aspx?ApplicationId=" + client.ApplicationId + "&ApplicationId=" + client.ApplicationId + "&ProductId=2",
                    false,
                    "TS8e49d4_id=3&TS8e49d4_md=1&TS8e49d4_ct=0&TS8e49d4_pd=0" +
                    "&TS8e49d4_rf=" + Form1.ToUrlEncode("https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Pay.aspx?ApplicationId=" + client.ApplicationId),
                    ref client.cookieContainer,
                    true);

                if (respHtml.Contains("there is no longer a place available for you"))
                {
                    form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + "there is no longer a place available for you, retry...");
                    return -5;
                }

                rgx = @"(?<=id=""__VIEWSTATE"" value="")(\s|\S)+?(?="")";
                myMatch = (new Regex(rgx)).Match(respHtml);
                if (myMatch.Success)
                {
                    client.__VIEWSTATE = Form1.ToUrlEncode(myMatch.Groups[0].Value);
                }
                else
                {
                    form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": server response error, retry...");
                    return -1;
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
                form1.setLogT(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": PaymentGateway...");
                //如果不使用信用卡余额控制支付, 则最好每次提交都去确认一下beingPaid参数. 那么就必须要有clientId(8位随机字符)来识别不同线程创建的client. 
                //于是, 支付函数中的每一次提交之前,都去判断一下client列表,如果(护照号和这个client相同, clientId不同, beingPaid为true) 那么直接return, 同一台机器上不同的进程也可以适用该规则
                //至于这个client列表, 对应的是一个本地文件, 主键是clientId, 有一个进程不停地同步服务器上的client和本地client.
                resp = Form1.weLoveYueer(
                    form1,
                    "https://onlineservices.immigration.govt.nz/PaymentGateway/OnLinePayment.aspx?SourceUrl=https%3a%2f%2fonlineservices.immigration.govt.nz%2fWorkingHoliday%2fApplication%2fSubmitConfirmation.aspx%3fApplicationId%3d" + client.ApplicationId + "&ApplicationId=" + client.ApplicationId + "&ProductId=2",
                    "POST",
                    "https://onlineservices.immigration.govt.nz/WorkingHoliday/Wizard/WorkingHolidaySpecific.aspx?ApplicationId=" + client.ApplicationId + "&IndividualType=Primary&IndividualIndex=1",
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
                    "https://onlineservices.immigration.govt.nz/PaymentGateway/OnLinePayment.aspx?SourceUrl=https%3a%2f%2fonlineservices.immigration.govt.nz%2fWorkingHoliday%2fApplication%2fSubmitConfirmation.aspx%3fApplicationId%3d" + client.ApplicationId + "&ApplicationId="+ client.ApplicationId +"&ProductId=2",
                    "POST",
                    "https://onlineservices.immigration.govt.nz/PaymentGateway/OnLinePayment.aspx?SourceUrl=https%3a%2f%2fonlineservices.immigration.govt.nz%2fWorkingHoliday%2fApplication%2fSubmitConfirmation.aspx%3fApplicationId%3d" + client.ApplicationId + "&ApplicationId=" + client.ApplicationId + "&ProductId=2",
                    false,
                    "TS8e49d4_id=3&TS8e49d4_md=2&TS8e49d4_ct=application%2Fx-onlineservices-form-urlencoded" +
                    "&TS8e49d4_rf=" + Form1.ToUrlEncode("https://onlineservices.immigration.govt.nz/WORKINGHOLIDAY/Application/Pay.aspx?ApplicationId=" + client.ApplicationId )+
                    
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
                            "https://onlineservices.immigration.govt.nz/PaymentGateway/OnLinePayment.aspx?SourceUrl=https%3a%2f%2fonlineservices.immigration.govt.nz%2fWorkingHoliday%2fApplication%2fSubmitConfirmation.aspx%3fApplicationId%3d" + client.ApplicationId + "&ApplicationId=" + client.ApplicationId + "&ProductId=2",
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
                           //     if(false)
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
                                        true,
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
                    if (respHtml.Contains("there is no longer a place available for you"))
                    {
                        form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + ": there is no longer a place available for you, retry...");
                        goto PaymentGateway;
                    }
                    else
                    {
                        form1.setLogtRed(client.FamilyName + " " + client.GivenName + " " + client.PassportNo + "unknow PaymentGateway page, retry...");
                        return -1;
                    }
                }

            
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

            Form1.gForceToStop = false;
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