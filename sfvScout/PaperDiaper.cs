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
    public class PaperDiaper
    {
        static string rgx;
        static Match myMatch;

        string verificationCode;
        CookieCollection cookieContainer = null;

        Mail163<PaperDiaper> mail;
        Appointment appointment;

        string sizeType = null;

        public List<string> gFriends = new List<string>();

        string gFileName = null;

        string eventId = "";
        Form1 form1;
        string keyURL;
        bool requireVeriCode = false;
        string countyForMulti, shopForMulti;

        public PaperDiaper(Form1 f, Appointment app, Mail163<PaperDiaper> em)
        {
            form1 = f;
            appointment = app;
            mail = em;
            sizeType = f.selectedType;
        }

        public PaperDiaper(Form1 f, Appointment app, Mail163<PaperDiaper> em, string countyForMulti, string shopForMulti, string eventId, bool requireVeriCode)
        {
            form1 = f;
            appointment = app;
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

                "GET", "", false, "", ref  cookieContainer,
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








        public int probe(string county, string shop)
        {
            string respHtml = Form1.weLoveYue(
                form1,
                "http://aksale.advs.jp/cp/akachan_sale_pc/search_event_list.cgi?area2=" + county + "&event_type=" + sizeType + "&sid=" + shop + "&kmws=",

                "GET", "", true, "", ref  cookieContainer,
                false
                );



            if (respHtml.Equals("Found"))
            {
                form1.setLogT("CardNo" + appointment.CardNo + ", first GET eccur an error!");
       //         return -1;
            }

            //       <input type="submit" name="sbmt" value="予約" >
            rgx = @"<input type=""submit"" name=""sbmt"" value=""";
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (!myMatch.Success)
            {
                form1.setLogT("CardNo" + appointment.CardNo + ", no available appointment.");
                return -5;// no available appointment
            }



            if (respHtml.Contains("eventId") && !respHtml.Contains("captcha"))
            {
                rgx = @"(?<=\[\]\,{""eventId"":)\d+?(?=\,)";  //there will be no veri code
                requireVeriCode = false;
            }
            else
            {
                rgx = @"(?<=予約"" >\n.+event_id"" value="")\d+?(?="")";  //there will be a veri code, and we can cheeck up with captcha
                //name="event_id" value="2543729870"
                requireVeriCode = true;
            }
            myMatch = (new Regex(rgx)).Match(respHtml);
            if (myMatch.Success)
            {
                eventId = myMatch.Groups[0].Value;
            }

            //one day's book in one card, for gong cheng county
            myMatch = myMatch.NextMatch();
            while (myMatch.Success)
            {
                PaperDiaper paper = new PaperDiaper(form1, appointment, mail, county, shop , myMatch.Groups[0].Value, requireVeriCode);
                Thread t = new Thread(paper.multiBook);
                t.Start();
                myMatch = myMatch.NextMatch();
            }

            if (!respHtml.Contains("captcha"))//        daiyyr
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
                "http://aksale.advs.jp/cp/akachan_sale_pc/search_event_list.cgi?area2=" + county + "&event_type=" + sizeType + "&sid=" + shop + "&kmws=", 
                false,
                "sbmt=%97%5C%96%F1&event_id="+eventId+"&event_type=6",
               ref  cookieContainer,
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
                            form1.label9.Text = "cardNo" + appointment.CardNo + ":请输入验证码";
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
                        form1.label9.Text = "cardNo" + appointment.CardNo + ":请输入验证码";
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
                            form1.label9.Visible = false;
                            form1.pictureBox1.ImageLocation = @"";
                        });
                        form1.textBox2.Invoke(sl);
                    }
                    else
                    {
                        form1.textBox2.ReadOnly = true;
                        form1.label9.Visible = false;
                        form1.pictureBox1.ImageLocation = @"";
                    }
                }// end of lock picturebox1

                
                //submit the veri code
                respHtml = Form1.weLoveYue(
                form1,
                "http://aksale.advs.jp/cp/akachan_sale_pc/_mail.cgi",
                "POST",
                "http://aksale.advs.jp/cp/akachan_sale_pc/captcha.cgi",
                false,
                "input_captcha=" + verificationCode + "&sbmt=%8E%9F%82%D6&event_id=" + eventId + "&event_type=" + sizeType ,
               ref  cookieContainer,
                false
                );
            



                while (respHtml.Contains("captcha"))
                {
                    form1.setLogT("CardNo" + appointment.CardNo + ", 验证码错误！请重新输入");
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
                                form1.label9.Text = "CardNo" + appointment.CardNo + ":请输入验证码";
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
                            form1.label9.Text = "CardNo" + appointment.CardNo + ":请输入验证码";
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
                                form1.label9.Visible = false;
                                form1.pictureBox1.ImageLocation = @"";
                            });
                            form1.textBox2.Invoke(sl);
                        }
                        else
                        {
                            form1.textBox2.ReadOnly = true;
                            form1.label9.Visible = false;
                            form1.pictureBox1.ImageLocation = @"";
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
                    ref cookieContainer,
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
                ref cookieContainer,
                false
                );

            //post email again
            respHtml = Form1.weLoveYue(
                form1,
                "https://aksale.advs.jp/cp/akachan_sale_pc/mail_confirm.cgi"
                ,
                "POST", 
                "https://aksale.advs.jp/cp/akachan_sale_pc/mail_form.cgi",
                false,
                "sbmt=%91%97%90M&mail1=" + mail.address.Replace("@", "%2540").Replace(".", "%252e") + "&mail2=" + mail.address.Replace("@", "%2540").Replace(".", "%252e") + "&event_id=" + eventId + "&event_type=" + sizeType ,
          //    sbmt=%91%97%90M&mail1=15985830370%2540163%252ecom&mail2=15985830370%2540163%252ecom&event_id=7938283049&event_type=6
                ref cookieContainer,
                false
          );

            
            if (respHtml.Contains("下記メールアドレスにメールを送信しました"))
            {               
                form1.setLogT("CardNo" + appointment.CardNo+ ", step1 succeed, checking email: " + mail.address);
            }
            else
            {
                form1.setLogtRed("CardNo" + appointment.CardNo + ", appointments closed: " + mail.address);
                return -1;
            }


            keyURL = mail.queery("ご注文予約案内", @"https://aksale(\s|\S)+?(?=\r)");
            if (keyURL == null | keyURL == "")
            {
                form1.setLogT("NULL url from email");
            }
            setAppointment(mail.address, keyURL);

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

            form1.setLogT("CardNo" + appointment.CardNo + ", start setting appointment from " + email);

            //https://aksale.advs.jp/cp/akachan_sale_pc/reg?id=fJrKmJSkXbhtEuAEFWSpeZJ4lfwyV93d
            string result = Form1.weLoveMuYue(
                form1,
                url,
                "GET",
                "https://aksale.advs.jp/cp/akachan_sale_pc/mail_form.cgi",
                true,
                "",
                ref cookieContainer
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
                "card_no=" + appointment.CardNo + "&sbmt=%8E%9F%82%D6",
                ref cookieContainer,
                false
                );

            if (html.Contains("恐れ入りますが、もう一度最初から操作してください"))
            {
                form1.setLogtRed("no available quota, url from: " + email);
                return -2;
            }
            if (html.Contains("正しいカード番号を入力してください"))
            {
                form1.setLogtRed("invalid cardNo: " + appointment.CardNo);
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
                "password="+appointment.CardPassword

                +"&sei="+ Form1.ToUrlEncode(appointment.ChineseName.Substring(0, 1), System.Text.Encoding.GetEncoding("shift-jis"))
                +"&mei="+ Form1.ToUrlEncode(appointment.ChineseName.Substring(1, appointment.ChineseName.Length - 1), System.Text.Encoding.GetEncoding("shift-jis"))
                +"&sei_kana="+Form1.ToUrlEncode(appointment.JapaneseName.Substring(0, 1), System.Text.Encoding.GetEncoding("shift-jis"))
                +"&mei_kana="+Form1.ToUrlEncode(appointment.JapaneseName.Substring(1, appointment.JapaneseName.Length - 1), System.Text.Encoding.GetEncoding("shift-jis"))
                +"&tel1="+Regex.Match(appointment.Phone, @"\d+(?=\-)").Value
                +"&tel2="+Regex.Match(appointment.Phone, @"(?<=\d+\-)\d+(?=-)").Value
                +"&tel3="+Regex.Match(appointment.Phone, @"(?<=\d+\-\d+\-)\d+").Value

                //"&sei=%9B%C1&mei=%94%F2%94%F2&sei_kana=%83T&mei_kana=%83C%83q%83q&tel1=090&tel2=8619&tel3=3569"

                +"&sbmt=%8E%9F%82%D6",
                ref cookieContainer,
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
                ref cookieContainer,
                false
                );
            
            if(html.Contains("ご予約ありがとうございます")){
                form1.setLogT("CardNo" + appointment.CardNo + ", Setting appointment succeed!");//daiyyr details
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
            form1.setLogT("probe started. CardNo:" + appointment.CardNo + ", " + form1.selecteCounty.Name
                + " " + form1.selecteCounty.Shops[form1.selectedShop]);
            while (true)
            {
                if (Form1.gForceToStop)
                {
                    break;
                }

                int r1 = 0;
            //新潟亀田ｱﾋﾟﾀ店  M
            //   http://aksale.advs.jp/cp/akachan_sale_pc/search_event_list.cgi?area2=%90V%8a%83%8c%a7&event_type=6&sid=37140&kmws=

                //ｱﾘｵ上田店 M
            //   http://aksale.advs.jp/cp/akachan_sale_pc/search_event_list.cgi?area2=%92%b7%96%ec%8c%a7&event_type=6&sid=37194&kmws=
            //songben M
            //   http://aksale.advs.jp/cp/akachan_sale_pc/search_event_list.cgi?area2=%92%b7%96%ec%8c%a7&event_type=6&sid=37196&kmws=

                //ららぽｰと横浜店
           // http://aksale.advs.jp/cp/akachan_sale_pc/search_event_list.cgi?area2=%90_%93%de%90%ec%8c%a7&event_type=6&sid=37139&kmws=

                //春日井店
                //http://aksale.advs.jp/cp/akachan_sale_pc/search_event_list.cgi?area2=%88%a4%92m%8c%a7&event_type=6&sid=37038&kmws=
                while ((r1 = this.probe(
                    Form1.ToUrlEncode(
                        form1.selecteCounty.Shops[form1.selectedShop], 
                        System.Text.Encoding.GetEncoding("shift-jis")
                     ),
                    form1.selecteCounty.Sids[form1.selectedShop])
                    ) == -1)   //"%88%a4%92m%8c%a7", "37038"
                {
  
                }
                if (r1 == -5) //no available appointment
                {
                    goto delay;
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
               ref  cookieContainer,
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
                            form1.label9.Text = "cardNo" + appointment.CardNo + ":请输入验证码";
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
                        form1.label9.Text = "cardNo" + appointment.CardNo + ":请输入验证码";
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
               ref  cookieContainer,
                false
                );




                while (respHtml.Contains("captcha"))
                {
                    form1.setLogT("CardNo" + appointment.CardNo + ", 验证码错误！请重新输入");
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
                                form1.label9.Text = "CardNo" + appointment.CardNo + ":请输入验证码";
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
                            form1.label9.Text = "CardNo" + appointment.CardNo + ":请输入验证码";
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
                    ref cookieContainer,
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
                ref cookieContainer,
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
                ref cookieContainer,
                false
          );


            if (respHtml.Contains("下記メールアドレスにメールを送信しました"))
            {
                form1.setLogtRed("CardNo" + appointment.CardNo + ", step1 succeed, checking email: " + mail.address);
            }
            else
            {
                form1.setLogtRed("CardNo" + appointment.CardNo + ", email submitting failed: " + mail.address);
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