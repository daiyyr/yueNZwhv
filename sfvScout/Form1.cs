using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Globalization;
using System.Net.Cache;
using System.Collections;
using System.Xml;

namespace widkeyPaperDiaper
{
    

    public partial class Form1 : Form
    {
        public static bool singleUser = true;

        /*
         * 当该字段为true时, 不验证程序是否过期
         * 当该字段为true时, 开放测试邮件等冗余功能测试按钮
         * 当该字段为true时, 即使点击自动模式, 也会使用德国表格和无效信用卡信息
         * if set true, Print page source to testLog
         */
        public static bool debug = true;



        /* 点击测试按钮时, 将把改字段置为true
         * 点击自动模式时, 将把改字段置为false
         * 当它为true时, 将使用 真实客户资料 + 无效信用卡信息 + 德国表格 进行测试.
         */
        public static bool testButton = false;

        
        public static int retry = 10;
        public static int timeoutTime = 60000;
        static DateTime expireDate = new DateTime(2017, 4, 1);
        public bool replaceInfo = false;


        string[] singleUserDetails = {

               
                "eru1989", "Dd123456", "E7222435", "eru1989", "dai",
                "F", "1646419111@163.com", "eru1989", "ZITENGYICUN5HAO", 
                "1985", "8", "18", //生日
                "2026", "3", "24", //护照失效
                "2013", "9", "29", //身份证发放
                "2033", "9", "29", //身份证失效
                "2017", "8", "1",  //计划入境日期
                "No",
                "", "", "",
                "shasha",
                "visa", "4693960018321975", "111", "04", "2018", "eru"
        
                
                
                



                                     };



        Client tempClient = null;
        public static bool gForceToStop = false;
        public static bool gLoginOkFlag = false;
        static string gHost = "onlineservices.immigration.govt.nz";
        public County selecteCounty = null;
        public int selectedShop = -1;
        public string selectedType = null;

        CookieCollection cookieContainerForTest;

        List<Client> ClientList;
        List<Mail163<Apply>> Maillist;
        List<County> Countylist = new List<County>();
        
        
        public class County
        {
            public string Name;
            public List<string> Shops { get; set; }
            public List<string> Sids { get; set; }
            public County(string name, List<string> shops, List<string> sids)
            {
                Name = name;
                Shops = shops;
                Sids = sids;
            }
        }

        public Form1()
        {
            InitializeComponent();

            //窗体名称
            label6.Text = "expire date: " + expireDate.ToString("yyyy-MM-dd");
            this.Text = (debug == true ? "测试版:" : "")
                + singleUserDetails[4] + singleUserDetails[3] + "-NZwhv";

         //       deleteForms.Visible = true;

            if (debug)
            {
               button1.Visible = true; //测试功能按钮
               // testLog.Visible = true;
               // this.ClientSize = new System.Drawing.Size(1150, 960);
                
            }
            else
            {
                DateTime t = GetNistTime(this);
                if (t == DateTime.MinValue)
                {
                    setLogT("请连接互联网后重新启动程序");
                    autoB.Visible = false;
                }
                else
                {
                    if ((t - expireDate).Days > 0)
                    {
                        setLogT("程序已过期，请联系作者");
                        autoB.Visible = false;
                    }
                }
            }

            tempClient = new Client(singleUserDetails[0], singleUserDetails[1], singleUserDetails[2], singleUserDetails[3], singleUserDetails[4], singleUserDetails[5], singleUserDetails[6], singleUserDetails[7], singleUserDetails[8],
                                            singleUserDetails[9], singleUserDetails[10], singleUserDetails[11],
                                            singleUserDetails[12], singleUserDetails[13], singleUserDetails[14],
                                            singleUserDetails[15], singleUserDetails[16], singleUserDetails[17],
                                            singleUserDetails[18], singleUserDetails[19], singleUserDetails[20],
                                            singleUserDetails[21], singleUserDetails[22], singleUserDetails[23],
                                            singleUserDetails[24],
                                            singleUserDetails[25], singleUserDetails[26], singleUserDetails[27],
                                            singleUserDetails[28],
                                            singleUserDetails[29], singleUserDetails[30], singleUserDetails[31], singleUserDetails[32], singleUserDetails[33], singleUserDetails[34]
                                    );

            //把客户的信用卡信息写到窗口中, 点击run前可修改
            creaditCardNo.Text = singleUserDetails[30];
            cardHolder.Text = singleUserDetails[34];
            cardExpiryMonth.SelectedIndex = Int32.Parse(singleUserDetails[32]) - 1;
            cardExpiryYear.SelectedIndex = 
                (
                    (Int32.Parse(singleUserDetails[33]) - DateTime.Now.Year)
                    < 0 ? (Int32.Parse(singleUserDetails[33]) - DateTime.Now.Year) + 2000  //客户若把2016写成16
                    : (Int32.Parse(singleUserDetails[33]) - DateTime.Now.Year)
                )
                ;
            cardType.SelectedIndex = (singleUserDetails[29].ToUpper() == "VISA") ? 0 : 1;
            cardVerificationCode.Text = singleUserDetails[31];


            if (singleUser)
            {
     //           loginB.Visible = true;
                ClientList = new List<Client>();
                ClientList.Add(tempClient);
                var source = new BindingSource();
                source.DataSource = ClientList;
                appointmentGrid.DataSource = source;
            }
            else
            {
                button3.Visible = true;
                deleteApp.Visible = true;
                checkBox1.Visible = false;

                //需显示结果框

                panel1.Controls.Add(label15);
                panel1.Controls.Add(label16);
                panel1.Controls.Add(label17);
                panel1.Controls.Add(label18);
                panel1.Controls.Add(label19);
                panel1.Controls.Add(cardTypeLabel);
                panel1.Controls.Add(creaditCardNo);
                panel1.Controls.Add(cardType);
                panel1.Controls.Add(cardHolder);
                panel1.Controls.Add(cardExpiryMonth);
                panel1.Controls.Add(cardExpiryYear);
                panel1.Controls.Add(cardVerificationCode);
                panel1.Visible = false;
                appointmentGrid.Height = 350;


            }
        }


        #region Util Functions
        public delegate void setLog(string str1);
        public void setLogT(string s)
        {
            if (logT.InvokeRequired)
            {
                // 实例一个委托，匿名方法，
                setLog sl = new setLog(delegate(string text)
                {
                    logT.AppendText(DateTime.Now.ToString() + " " + text + Environment.NewLine);
                });
                // 把调用权交给创建控件的线程，带上参数
                logT.Invoke(sl, s);
            }
            else
            {
                logT.AppendText(DateTime.Now.ToString() + " " + s + Environment.NewLine);
            }
        }
        public delegate void setLogWithColor(RichTextBox rtb, string str1, Color color1);
        public void setLogtColorful(RichTextBox r, string s, Color c)
        {
            if (r.InvokeRequired)
            {
                setLogWithColor sl = new setLogWithColor(delegate (RichTextBox rtb, string text, Color color)
                {
                    rtb.AppendText(text + Environment.NewLine);
                    int i = 0;
                    if (rtb.Text.Length >= 2)
                    {
                        i = rtb.Text.LastIndexOf("\n", rtb.Text.Length - 2);
                    }
                    if (i < 0)
                    {
                        i = 0;
                    }
                    rtb.Select(i, rtb.Text.Length);
                    rtb.SelectionColor = color;
                    rtb.Select(i, rtb.Text.Length);
                    rtb.SelectionFont = new Font(rtb.Font, FontStyle.Bold);
                });
                r.Invoke(sl, r, s, c);
            }
            else
            {
                r.AppendText(s + Environment.NewLine);
                int i = 0;
                if (r.Text.Length >= 2)
                {
                    i = r.Text.LastIndexOf("\n", r.Text.Length - 2);
                }
                if (i < 0)
                {
                    i = 0;
                }
                r.Select(i, r.Text.Length);
                r.SelectionColor = c;
                r.Select(i, r.Text.Length);
                r.SelectionFont = new Font(r.Font, FontStyle.Bold);
            }
        }
        public void setLogtRed(string s)
        {
            if (logT.InvokeRequired)
            {
                setLog sl = new setLog(delegate (string text)
                {
                    logT.AppendText(text + Environment.NewLine);
                    int i = 0;
                    if (logT.Text.Length >= 2)
                    {
                        i = logT.Text.LastIndexOf("\n", logT.Text.Length - 2);
                    }
                    if (i < 0)
                    {
                        i = 0;
                    }
                    logT.Select(i, logT.Text.Length);
                    logT.SelectionColor = Color.Red;
                    logT.Select(i, logT.Text.Length);
                    logT.SelectionFont = new Font(logT.Font, FontStyle.Bold);
                });
                logT.Invoke(sl, s);
            }
            else
            {
                logT.AppendText(s + Environment.NewLine);
                int i = 0;
                if (logT.Text.Length >= 2)
                {
                    i = logT.Text.LastIndexOf("\n", logT.Text.Length - 2);
                }
                if (i < 0)
                {
                    i = 0;
                }
                logT.Select(i, logT.Text.Length);
                logT.SelectionColor = Color.Red;
                logT.Select(i, logT.Text.Length);
                logT.SelectionFont = new Font(logT.Font, FontStyle.Bold);
            }
        }



        public delegate void DSetTestLog(HttpWebRequest req, string respHtml);
        public delegate void DSetTestLog2(string respHtml);
        public void setTestLog(HttpWebRequest req, string respHtml)
        {
            if (testLog.InvokeRequired)
            {
                DSetTestLog sl = new DSetTestLog(delegate(HttpWebRequest req1, string text)
                {
                    testLog.Text += Environment.NewLine + text;
                });
                testLog.Invoke(sl, req, respHtml);
            }
            else
            {
                testLog.Text += Environment.NewLine + respHtml;
            }
        }
        public void setTestLog(string respHtml)
        {
            if (testLog.InvokeRequired)
            {
                DSetTestLog2 sl = new DSetTestLog2(delegate (string text)
                {
                    testLog.Text += Environment.NewLine + text;
                });
                testLog.Invoke(sl, respHtml);
            }
            else
            {
                testLog.Text += Environment.NewLine + respHtml;
            }
        }
        /*
        public void alarm()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(WHA_avac.Properties.Resources.mtl);
            player.Load();
            player.PlayLooping();
        }
        */
        public int downloadHtml(string prex, string html)
        {
            string fileName = prex + "." + System.DateTime.Now.ToString("yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo) + ".html";
            writeFile(System.Environment.CurrentDirectory + "\\" + fileName, html);
            return 1;
        }


        public static DateTime GetNistTime(Form1 form1)
        {
            DateTime dateTime = DateTime.MinValue;

//            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://nist.time.gov/actualtime.cgi?lzbc=siqm9b");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=1");
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0)";
                                    
            request.ContentType = "application/x-www-form-urlencoded";
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore); //No caching
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException webEx)
            {
                form1.setLogT("WebException: " + webEx.Status.ToString());
                return dateTime;
            }
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader stream = new StreamReader(response.GetResponseStream());
//                string html = stream.ReadToEnd();//<timestamp time=\"1395772696469995\" delay=\"1395772696469995\"/>
//                string time = Regex.Match(html, @"(?<=\btime="")[^""]*").Value;
//                double milliseconds = Convert.ToInt64(time) / 1000.0;
//                dateTime = new DateTime(1970, 1, 1).AddMilliseconds(milliseconds).ToLocalTime();

                string html = stream.ReadToEnd();//0=1443934730460
                string time = Regex.Match(html, @"(?<=0\=)\d{10}").Value;
                dateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                //dateTime = (new DateTime(1970, 1, 1));   //Central Standard Time
                long lTime = long.Parse(time + "0000000");
                TimeSpan toNow = new TimeSpan(lTime);
                dateTime = dateTime.Add(toNow);
                
                //   dateTime = DateTime.ParseExact(time, "MM月dd日", System.Globalization.CultureInfo.InvariantCulture);
            }
            return dateTime;
        }

        public static string ToUrlEncode(string strCode)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(strCode); //默认是System.Text.Encoding.Default.GetBytes(str)  
            System.Text.RegularExpressions.Regex regKey = new System.Text.RegularExpressions.Regex("^[A-Za-z0-9]+$");
            for (int i = 0; i < byStr.Length; i++)
            {
                string strBy = Convert.ToChar(byStr[i]).ToString();
                if (regKey.IsMatch(strBy))
                {
                    //是字母或者数字则不进行转换    
                    sb.Append(strBy);
                }
                else
                {
                    sb.Append(@"%" + Convert.ToString(byStr[i], 16));
                }
            }
            return (sb.ToString());
        }

        public static string ToUrlEncode(string strCode, System.Text.Encoding encode)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = encode.GetBytes(strCode); //默认是System.Text.Encoding.Default.GetBytes(str)  
            System.Text.RegularExpressions.Regex regKey = new System.Text.RegularExpressions.Regex("^[A-Za-z0-9]+$");
            for (int i = 0; i < byStr.Length; i++)
            {
                string strBy = Convert.ToChar(byStr[i]).ToString();
                if (regKey.IsMatch(strBy))
                {
                    //是字母或者数字则不进行转换    
                    sb.Append(strBy);
                }
                else
                {
                    sb.Append(@"%" + Convert.ToString(byStr[i], 16));
                }
            }
            return (sb.ToString());
        }

        public static void writeFile(string file, string content)
        {
            FileStream aFile;
            StreamWriter sw;
            aFile = new FileStream(file, FileMode.Append);
            sw = new StreamWriter(aFile);
            sw.Write(content);
            sw.Close();
        }




        public static void setRequest(HttpWebRequest req, CookieCollection cookies, bool xmlRequest = false)
        {
            //req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //req.Accept = "*/*";
            //req.Connection = "keep-alive";
            //req.KeepAlive = true;
            //req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; .NET4.0E";
            //req.Headers["Accept-Encoding"] = "gzip, deflate";
            //req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;



            req.Timeout = timeoutTime;

            req.Host = gHost;

            req.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.10; rv:40.0) Gecko/20100101 Firefox/40.0";
            req.AllowAutoRedirect = false;
            req.CookieContainer = new CookieContainer();
            req.CookieContainer.PerDomainCapacity = 40;
            if (cookies != null)
            {
                req.CookieContainer.Add(cookies);
            }
            if (xmlRequest)
            {
                req.ContentType = "text/xml; encoding='utf-8'";
            }
            else
            {
                req.ContentType = "application/x-www-form-urlencoded";
            }
        }
        
        public static int writePostData(Form1 form1, HttpWebRequest req, string postData, bool xmlRequest = false)
        {
            byte[] postBytes = Encoding.UTF8.GetBytes(postData);
     //           (xmlRequest ? Encoding.ASCII.GetBytes(postData) : Encoding.UTF8.GetBytes(postData));

            if (xmlRequest)
            {
                req.ContentLength = postBytes.Length;
            }

            //req.ContentLength = postBytes.Length;  // cause InvalidOperationException: 写入开始后不能设置此属性。
            Stream postDataStream = null;
            try
            {
                postDataStream = req.GetRequestStream();
                postDataStream.Write(postBytes, 0, postBytes.Length);
            }
            catch (WebException webEx)
            {
                form1.setLogT("While writing post data," + webEx.Status.ToString());
                return -1;
            }
            
            postDataStream.Close();
            return 1;
        }

        public static string resp2html(HttpWebResponse resp )
        {
            
            if (resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.Found)
            {
                StreamReader stream = new StreamReader(resp.GetResponseStream());
                return stream.ReadToEnd();
                //Shift_JIS
            }
            else
            {
                return resp.StatusDescription;
            }

        }

        public static string resp2html(HttpWebResponse resp, string charSet, Form1 form1)
        {
            var buffer = GetBytes(form1, resp);
            if (resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.Found)
            {
                if (String.IsNullOrEmpty(charSet) || string.Compare(charSet, "ISO-8859-1") == 0)
                {
                    charSet = GetEncodingFromBody(buffer);
                }

                try
                {
                    var encoding = Encoding.GetEncoding(charSet);  //Shift_JIS
                    var str = encoding.GetString(buffer);                

                    return str;
                }
                catch (Exception ex)
                {
                    form1.setLogT("resp2html, " + ex.ToString());
                    return string.Empty;
                }


                /*
                string respHtml = "";
                char[] cbuffer = new char[256];
                Stream respStream = resp.GetResponseStream();
                StreamReader respStreamReader = new StreamReader(respStream, encoding);//respStream,Encoding.UTF8
                int byteRead = 0;
                try
                {
                    byteRead = respStreamReader.Read(cbuffer, 0, 256);

                }
                catch (WebException webEx)
                {
                    setLogT("respStreamReader, " + webEx.Status.ToString());
                    return "";
                }
                while (byteRead != 0)
                {
                    string strResp = new string(cbuffer, 0, byteRead);
                    respHtml = respHtml + strResp;
                    try
                    {
                        byteRead = respStreamReader.Read(cbuffer, 0, 256);
                    }
                    catch (WebException webEx)
                    {
                        setLogT("respStreamReader, " + webEx.Status.ToString());
                        return "";
                    }

                }
                respStreamReader.Close();
                respStream.Close();
                return respHtml;

                */

            }
            else
            {
                return resp.StatusDescription;
            }

        }

        private static byte[] GetBytes(Form1 form1, WebResponse response)
        {
            var length = (int)response.ContentLength;
            byte[] data;

            using (var memoryStream = new MemoryStream())
            {
                var buffer = new byte[0x100];
                try {
                    using (var rs = response.GetResponseStream())
                    {
                        for (var i = rs.Read(buffer, 0, buffer.Length); i > 0; i = rs.Read(buffer, 0, buffer.Length))
                        {
                            memoryStream.Write(buffer, 0, i);
                        }
                    }
                }
                catch (Exception e)
                {
                    form1.setLogT("read ResponseStream: "+ e.ToString()); //500
                }
                

                data = memoryStream.ToArray();
            }

            return data;
        }

        private static string GetEncodingFromBody(byte[] buffer)
        {
            var regex = new Regex(@"<meta(\s+)http-equiv(\s*)=(\s*""?\s*)content-type(\s*""?\s+)content(\s*)=(\s*)""text/html;(\s+)charset(\s*)=(\s*)(?<charset>[a-zA-Z0-9-]+?)""(\s*)(/?)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var str = Encoding.ASCII.GetString(buffer);
            var regMatch = regex.Match(str);
            if (regMatch.Success)
            {
                var charSet = regMatch.Groups["charset"].Value;
                return charSet;
            }

            return Encoding.ASCII.BodyName;
        }

        /* 
         * return response status
         * especially, if found, return"found: http......"
         */
        public static string weLoveMuYue(Form1 form1, string url, string method, string referer, bool allowAutoRedirect, string postData, ref CookieCollection cookies)
        {
            string result;
            for (int i = 0; i < retry;  i++ )
            {
                if (gForceToStop)
                {
                    break;
                }
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = null;
                setRequest(req, cookies);
                req.Method = method;
                req.Referer = referer;
                if (allowAutoRedirect)
                {
                    req.AllowAutoRedirect = true;
                }
                if (method.Equals("POST"))
                {
                    if (writePostData(form1, req, postData) < 0)
                    {
                        continue;
                    }
                }
                string respHtml = "";
                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException webEx)
                {
                    form1.setLogT("GetResponse, " + webEx.Status.ToString());
                    if (webEx.Status == WebExceptionStatus.ConnectionClosed)
                    {
                        form1.setLogT("wrong address"); //地址错误
                    }
                    if (webEx.Status == WebExceptionStatus.ProtocolError)
                    {
                        form1.setLogT("本次请求被服务器拒绝，可尝试调高间隔时间"); //500
                    }
                    continue;
                }
                if (resp != null)
                {
                    result = resp.StatusDescription;
                    if (result == "Found")
                    {
                        result += ":" + resp.Headers["location"];
                    }
                }
                else
                {
                    continue;
                }
                if (debug)
                {
                    respHtml = resp2html(resp);
                    form1.setTestLog(req, respHtml);
                }
                cookies = req.CookieContainer.GetCookies(req.RequestUri);
                resp.Close();
                return result;
            }
            return string.Empty;
        }

        /* unregular host
         * return response status
         * especially, if found, return"found: http......"
         * 
         */
        public static string weLoveMuYue(Form1 form1, string url, string method, string referer, bool allowAutoRedirect, string postData, ref CookieCollection cookies, string host)
        {
            string result;
            for (int i = 0; i < retry; i++)
            {
                if (gForceToStop)
                {
                    break;
                }
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = null;
                setRequest(req, cookies);
                req.Host = host;
                req.Method = method;
                req.Referer = referer;
                if (allowAutoRedirect)
                {
                    req.AllowAutoRedirect = true;
                }
                if (method.Equals("POST"))
                {
                    if (writePostData(form1, req, postData) < 0)
                    {
                        continue;
                    }
                }
                string respHtml = "";
                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException webEx)
                {
                    form1.setLogT("GetResponse, " + webEx.Status.ToString());
                    if (webEx.Status == WebExceptionStatus.ConnectionClosed)
                    {
                        form1.setLogT("wrong address"); //地址错误
                    }
                    if (webEx.Status == WebExceptionStatus.ProtocolError)
                    {
                        form1.setLogT("本次请求被服务器拒绝，可尝试调高间隔时间"); //500
                    }
                    continue;
                }
                if (resp != null)
                {
                    result = resp.StatusDescription;
                    if (result == "Found")
                    {
                        result += ":"+ resp.Headers["location"];
                    }
                }
                else
                {
                    continue;
                }
                if (debug)
                {
                    respHtml = resp2html(resp);
                    form1.setTestLog(req, respHtml);
                }
                cookies = req.CookieContainer.GetCookies(req.RequestUri);
                resp.Close();
                return result;
            }
            return string.Empty;
        }


        /* 
         * return response HTML
         */
        public static string weLoveYue(Form1 form1, string url, string method, string referer, bool allowAutoRedirect, string postData, ref CookieCollection cookies, bool responseInUTF8)
        {
            if (form1 == null)
            {
                return string.Empty;
            }
            string respHtml = "";
            for (int i = 0; i < retry; i++)
            {
                if (gForceToStop)
                {
                    break;
                }
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = null;
                setRequest(req, cookies);
                req.Method = method;
                req.Referer = referer;
                if (allowAutoRedirect)
                {
                    req.AllowAutoRedirect = true;
                }
                if (method.Equals("POST"))
                {
					if (writePostData (form1, req, postData) < 0) {
						continue;
					}
                }
                
                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException webEx)
                {
                    form1.setLogT("GetResponse, " + webEx.Status.ToString());
                    if (webEx.Status == WebExceptionStatus.ConnectionClosed)
                    {
                        return "wrong address"; //地址错误
                    }
                    if (webEx.Status == WebExceptionStatus.ProtocolError)
                    {
                        form1.setLogT( "本次请求被服务器拒绝，可尝试调高间隔时间" ); //500
                    }
                    continue;
                }
                if (resp != null)
                {   
                    if(responseInUTF8)
                    {
                        respHtml = resp2html(resp);
                    }else
                    {
                        respHtml = resp2html(resp, resp.CharacterSet, form1); // like  Shift_JIS
                    }

                    if (respHtml.Equals(""))
                    {
                        continue;
                    }
                    cookies = req.CookieContainer.GetCookies(req.RequestUri);
                    if (debug)
                    {
                        form1.setTestLog(req, respHtml);
                    }
                    resp.Close();
                    return respHtml;
                }
                else
                {
                    continue;
                }
            }
            return respHtml;
        }

        /* 
         * return responsive HTML
         * unregular host
         */
        public static string weLoveYue(Form1 form1, string url, string method, string referer, bool allowAutoRedirect, string postData, ref CookieCollection cookies, string host, bool responseInUTF8, bool xmlRequest = false)
        {
            for (int i = 0; i < retry; i++)
            {
                if (gForceToStop)
                {
                    break;
                }
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = null;
                setRequest(req, cookies, xmlRequest);
                req.Method = method;
                req.Referer = referer;
                if (allowAutoRedirect)
                {
                    req.AllowAutoRedirect = true;
                }
                req.Host = host;
                if (method.Equals("POST"))
                {
                    if (writePostData(form1, req, postData, xmlRequest) < 0)
                    {
                        continue;
                    }
                }
                string respHtml = "";
                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException webEx)
                {
                    form1.setLogT("GetResponse, " + webEx.Status.ToString());
                    if (webEx.Status == WebExceptionStatus.ConnectionClosed)
                    {
                        return "wrong address"; //地址错误
                    }
                    if (webEx.Status == WebExceptionStatus.ProtocolError)
                    {
                        form1.setLogT("本次请求被服务器拒绝，可尝试调高间隔时间"); //500
                    }
                    continue;
                }
                if (resp != null)
                {
                    if(responseInUTF8)
                    {
                        respHtml = resp2html(resp);
                    }else
                    {
                        respHtml = resp2html(resp, resp.CharacterSet, form1); // like  Shift_JIS
                    }
                    if (respHtml.Equals(""))
                    {
                        continue;
                    }
                    cookies = req.CookieContainer.GetCookies(req.RequestUri);
                    if (debug)
                    {
                        form1.setTestLog(req, respHtml);
                    }
                    resp.Close();
                    return respHtml;
                }
                else
                {
                    continue;
                }
            }
            return "";
        }

        /*
         * do not handle the response
         */
        public static HttpWebResponse weLoveYueer(Form1 form1, string url, string method, string referer, bool allowAutoRedirect, string postData, ref CookieCollection cookies)
        {
            HttpWebResponse resp = null;
            for (int i = 0; i < retry; i++)
            {
                if (gForceToStop)
                {
                    break;
                }
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                setRequest(req, cookies);
                req.Method = method;
                req.Referer = referer;
                if (allowAutoRedirect)
                {
                    req.AllowAutoRedirect = true;
                }
                if (method.Equals("POST"))
                {
                    if (writePostData(form1, req, postData) < 0)
                    {
						continue;
					}
                }
                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException webEx)
                {
                    form1.setLogT("GetResponse, " + webEx.Status.ToString());
                    if (webEx.Status == WebExceptionStatus.ConnectionClosed)
                    {
                        form1.setLogT( "wrong address"); //地址错误
                    }
                    if (webEx.Status == WebExceptionStatus.ProtocolError)
                    {
                        form1.setLogT("本次请求被服务器拒绝，可尝试调高间隔时间"); //500
                    }
                    if(webEx.Status == WebExceptionStatus.ReceiveFailure)
                    {
                        Thread.Sleep(1000);
                    }
                    continue;
                }
                if (resp != null)
                {
                    cookies = req.CookieContainer.GetCookies(req.RequestUri);
                    return resp;
                }
                else
                {
                    continue;
                }
            }
            return resp;
        }

        /*
         * do not handle the response
         * with host
         */
        public static HttpWebResponse weLoveYueer(Form1 form1, string url, string method, string referer, bool allowAutoRedirect, string postData, ref CookieCollection cookies, string host)
        {
            HttpWebResponse resp = null;
            for (int i = 0; i < retry; i++)
            {
                if (gForceToStop)
                {
                    break;
                }
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                setRequest(req, cookies);
                req.Method = method;
                req.Referer = referer;
                if (allowAutoRedirect)
                {
                    req.AllowAutoRedirect = true;
                }
                req.Host = host;
                if (method.Equals("POST"))
                {
                    if (writePostData(form1, req, postData) < 0)
                    {
                        continue;
                    }
                }
                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException webEx)
                {
                    form1.setLogT("GetResponse, " + webEx.Status.ToString());
                    if (webEx.Status == WebExceptionStatus.ConnectionClosed)
                    {
                        form1.setLogT("wrong address"); //地址错误
                    }
                    if (webEx.Status == WebExceptionStatus.ProtocolError)
                    {
                        form1.setLogT("本次请求被服务器拒绝，可尝试调高间隔时间"); //500
                    }
                    continue;
                }
                if (resp != null)
                {
                    cookies = req.CookieContainer.GetCookies(req.RequestUri);
                    return resp;
                }
                else
                {
                    continue;
                }
            }
            return resp;
        }
        #endregion
        
        public void loginF()
        {
            Login login = new Login(this, tempClient); // temp
            login.loginT();
        }

        private void loginB_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(loginF);
            t.Start();
        }

        public void testButonClickF()
        {
            Login login = new Login(this, tempClient); //temp
            if (login.loginT() == 1) {
                Apply apply = new Apply(this, tempClient);
                apply.startProbe();
            }

            
        }

        public void auto()
        {

            //setLogtRed("user operation: start probing");

            /*
            if (selecteCounty == null || selectedShop == -1 || selectedType == null)
            {
                this.setLogT("please choose type, county and shop");
                return;
            }
            if (selectedShop >= selecteCounty.Shops.Count)
            {
                this.setLogT("invalid selected shop");
                return;
            }
            */

            if(singleUser){

                Login login = new Login(this, tempClient);
                if (login.loginT() == 1)
                {
                    Apply apply = new Apply(this, tempClient);
                    apply.startProbe();
                }
            }
            else
            {

                Thread[] threads = new Thread[ClientList.Count];

                if (ClientList == null || ClientList.Count < 1)
                {
                    this.setLogT("please import client list");
                    return;
                }
                for (int i = 0; i < ClientList.Count; i++)
                {
                    ClientList[i].form1 = this;
                    Thread t = new Thread(ClientList[i].loginAndApply);
                    t.Start();
                }

            }
        }


        private void autoB_Click(object sender, EventArgs e)
        {
            //测试模式,仍使用德国表格和无效信用卡
            if (debug == true)
            {
                tempClient.CardNumber = "4693960018321975";
                tempClient.CardHolder = "zhanghuimei";
                tempClient.CardVerificationCode = "111";
                tempClient.CardExpiryYear = "2018";
                tempClient.CardExpiryMonth = "04";
                tempClient.CardType = "visa";
            }
            
            //用可能被用户修改过的信用卡信息覆盖client实例
            else if(singleUser){
                if (cardType.SelectedIndex == -1 || cardExpiryYear.SelectedIndex == -1 || cardExpiryMonth.SelectedIndex == -1 || creaditCardNo.Text == "" || cardHolder.Text == "" || cardVerificationCode.Text == "")
                {
                    setLogtColorful(logT, "请输入完整的信用卡信息", Color.Red);
                    return;
                }
                tempClient.CardNumber = creaditCardNo.Text.Replace(" ", "");
                tempClient.CardHolder = cardHolder.Text;
                tempClient.CardVerificationCode = cardVerificationCode.Text;
                tempClient.CardExpiryYear = cardExpiryYear.SelectedItem.ToString();
                tempClient.CardExpiryMonth = cardExpiryMonth.SelectedItem.ToString();
                tempClient.CardType = (cardType.SelectedItem.ToString() == "visa" ? "visa" : "masterCard");
            }
            panel1.Enabled = false;
            cardExpiryMonth.Enabled = false;
            cardExpiryYear.Enabled = false;
            cardVerificationCode.Enabled = false;
            replaceInfo = checkBox1.Checked;
            checkBox1.Enabled = false;
            if (tempClient != null)
            {
                tempClient.cookieContainer = null;
            }
            gForceToStop = false;
            testButton = false;
            button4.Visible = false;
            Thread t = new Thread(auto);
            t.Start();
        }
        
        private void logT_TextChanged(object sender, EventArgs e)
        {
            logT.SelectionStart = logT.Text.Length;
            logT.ScrollToCaret();
        }

        public class EmailForshow{
            public string Email { get; set; }
            public string Password { get; set; }

            public EmailForshow(string email, string password)
            {
                Email = email;
                Password = password;
            }
        }
            
        public delegate void delegate2();

        public void addEmails()
        {
        }

        public void deleteEmails()
        {

        }

        private void addB_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(addEmails);
            t.Start();
        }

        private void deleteB_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(deleteEmails);
            t.Start();
        }

        public void addDetails()
        {
            ClientList = new List<Client>();

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "(*.txt)|*.txt|(*.html)|*.html";

            if (appointmentGrid.InvokeRequired)
            {
                delegate2 sl = new delegate2(delegate()
                {
                    //打开对话框, 判断用户是否正确的选择了文件
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //获取用户选择的文件，并判断文件大小不能超过20K，fileInfo.Length是以字节为单位的
                        FileInfo fileInfo = new FileInfo(fileDialog.FileName);
                        if (fileInfo.Length > 504800)
                        {
                            MessageBox.Show("上传的文件不能大于500K");
                        }
                        else
                        {
                            //在这里就可以写获取到正确文件后的代码了
                            string[] lines = File.ReadAllLines(fileDialog.FileName, System.Text.Encoding.GetEncoding("GB18030"));
                            foreach (string line in lines)
                            {
                                if (line.Length == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    Regex regex = new Regex(@"( ){2,}");
                                    string[] s = regex.Replace(line.Trim(), " ").Split(' ');
                                    if (s.Length != 35)
                                    {
                                        setLogT("ignore invalid line: " + line); //500
                                    }
                                    else
                                    {
                                        ClientList.Add(new Client(s[0], s[1], s[2], s[3], s[4], s[5], s[6], s[7], s[8],
                                            s[9], s[10], s[11],
                                            s[12], s[13], s[14],
                                            s[15], s[16], s[17],
                                            s[18], s[19], s[20],
                                            s[21], s[22], s[23],
                                            s[24], 
                                            s[25], s[26], s[27],
                                            s[28],
                                            s[29],s[30],s[31],s[32],s[33],s[34]

                                            ));
                                    }
                                }
                            }
                            if (ClientList.Count > 0)
                            {
                                var source = new BindingSource();
                                source.DataSource = ClientList;
                                appointmentGrid.DataSource = source;
                            }
                        }
                    }
                });
                appointmentGrid.Invoke(sl);
            }
            else //do not use delegate
            {
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileInfo fileInfo = new FileInfo(fileDialog.FileName);
                    if (fileInfo.Length > 204800)
                    {
                        MessageBox.Show("上传的文件不能大于200K");
                    }
                    else
                    {
                        string[] lines = File.ReadAllLines(fileDialog.SafeFileName);
                        foreach (string line in lines)
                        {
                            if (line.Length == 0)
                            {
                                continue;
                            }
                            else
                            {
                                Regex regex = new Regex(@"( ){2,}");
                                string[] s = regex.Replace(line.Trim(), " ").Split(' ');
                                if (s.Length != 35)
                                {
                                    setLogT("ignore invalid line: " + line); //500
                                }
                                else
                                {
                                    
                                        ClientList.Add(new Client(s[0], s[1], s[2], s[3], s[4], s[5], s[6], s[7], s[8],
                                            s[9], s[10], s[11],
                                            s[12], s[13], s[14],
                                            s[15], s[16], s[17],
                                            s[18], s[19], s[20],
                                            s[21], s[22], s[23],
                                            s[24],
                                            s[25], s[26], s[27],
                                            s[28],
                                            s[29],s[30],s[31],s[32],s[33],s[34]
                                        ));
                                }
                            }
                        }
                        if (ClientList.Count > 0)
                        {
                            var source = new BindingSource();
                            source.DataSource = ClientList;
                            appointmentGrid.DataSource = source;
                //            dataGridView1.
                        }
                    }
                }
            }

        }

        public void deleteDetails()
        {
            /*
            if (appointmentGrid.InvokeRequired)
            {
                delegate2 sl = new delegate2(delegate()
                {
                    for (int i = appointmentGrid.CheckedItems.Count - 1; i >= 0; i--)
                    {
                        appointmentGrid.Items.Remove(appointmentGrid.CheckedItems[i]);
                    }
                });
                appointmentGrid.Invoke(sl);
            }
            else
            {
                for (int i = appointmentGrid.CheckedItems.Count - 1; i >= 0; i--)
                {
                    appointmentGrid.Items.Remove(appointmentGrid.CheckedItems[i]);
                }
            }
             */ 
        }

        private void addDetails_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(addDetails);
            t.Start();
        }

        private void deleteDetails_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(deleteDetails);
            t.Start();
        }


        // for texting indexer begin
        private string[] arr = new string[100];
        public string this[int i]
        {
            get
            {
                // This indexer is very simple, and just returns or sets
                // the corresponding element from the internal array.
                return arr[i];
            }
            set
            {
                arr[i] = value;
            }
        }
        // for texting indexer end

          enum aaaa{
                  a=1,
                  b,
                  c=4,
                  d=a|b,
                  e=c|a,
                  f=b|c
                  }

        // test button
        private void button1_Click(object sender, EventArgs e)
        {
            string maxDate = DateTime.Now.ToString("%d", DateTimeFormatInfo.InvariantInfo) + "+" +
                                DateTime.Now.ToString("MMM", DateTimeFormatInfo.InvariantInfo) + "+" +
                                DateTime.Now.ToString("yyyy", DateTimeFormatInfo.InvariantInfo);
            string minDate = DateTime.Now.ToString("%d", DateTimeFormatInfo.InvariantInfo) + "+" +
                                DateTime.Now.ToString("MMM", DateTimeFormatInfo.InvariantInfo) + "+" +
                                (DateTime.Now.Year - 31).ToString();
            // for texting indexer begin
            Form1 stringCollection = new Form1();
            // Use [] notation on the type.
            stringCollection[0] = "test indexer";
            System.Console.WriteLine(stringCollection[0]);
            // for texting indexer end

            //for safe call window form
            delegate2 tttt8987 = new delegate2(
                delegate() { }
                );
            appointmentGrid.Invoke(tttt8987);

            //new thread to run an anominous function 
            Thread thread1111 = new System.Threading.Thread(
                delegate()
              {


                  string[] aaa = new string[] { "222", "333"};

                  Console.Write((int)aaaa.f);

                

                  //test Mail Reading
                  Mail163<Apply> testMailReading = new Mail163<Apply>("15985830370@163.com", "dyyr7921129", this);
                  //      setLogT(testMailReading.queery("20151117test1", @"(\s|\S)*"));
                  setLogT(testMailReading.queeryReaded(@".*", @"(\s|\S)*"));



                  /*
                    rgx = @"(?<=予約"" >\n.+event_id"" value="")\d+?(?="")";  //there will be a veri code, and we can cheeck up with captcha
                        
                    Match myM33atch = (new Regex(rgx)).Match("<input type=\"submit\" name=\"sbmt\" value=\"予約\" >\n<input type=\"hidden\" name=\"event_id\" value=\"4500175502\" ><input type=\"submit\" name=\"sbmt\" value=\"予約キャンセル\" >\n<input type=\"hidden\" name=\"event_id\" value=\"4500175502\" >");
                    while (myM33atch.Success)
                    {
                        setLogT(myM33atch.Value);
                        myM33atch = myM33atch.NextMatch();
                    }



                  
                  //searchMailDirectely
                  PaperDiaper paper = new PaperDiaper(
                            this,
                            new Appointment("2800056599132", "guoguo01", "李花", "りはな", "090-1234-2580"),
                            new Mail163<PaperDiaper>("15985830370@163.com", "dyyr7921129", this));
                  //paper.searchMailDirectely();
                  paper.searchMailDirectelyFromReaded();
                  */

                  /*
   // let code help me making code -- get counties
                   //<option value="%96k%8aC%93%b9" >北海道</option>
                   string result22="";
                   string html = weLoveYue(
                       this,
                   "http://aksale.advs.jp/cp/akachan_sale_pc/search_shop_top.cgi?event_type=7",
                   "GET",
                   "",
                   false,
                   "",
                   ref cookieContainerForTest,
                   false);
                                           //      <option value="%96k%8aC%93%b9" >北海道</option>
                   Match match = (new Regex(@"(?<=\<option value=""(\s|\S)+?"" \>).+?(?=\<\/option)")).Match(html);
                   while(match.Success){
                       string t_county = match.Groups[0].Value;
                       result22+="Countylist.Add(new County("+"\n\""+t_county+"\",\n";
                       html = weLoveYue(
                       this,
                       "http://aksale.advs.jp/cp/akachan_sale_pc/search_shop_area3.cgi",
                       "POST",
                       "",
                       false,
                       "area2="
                           +Form1.ToUrlEncode(t_county, System.Text.Encoding.GetEncoding("shift-jis")) 
                           + "&sbmt=%81%40%8C%9F%8D%F5%81%40&event_type=7",
                       ref cookieContainerForTest,
                       false
                       );
                       //<a href="./search_event_list.cgi?area2=%96k%8aC%93%b9&event_type=7&sid=37116&kmws=">旭川店</a><br />
                       string r1 = @"(?<=area2=(\s|\S)+?\&event_type=7\&sid=)\d+";
                       Match match2 = (new Regex(r1)).Match(html);
                       {
                           string t_shop = "";
                           string t_sid = "";
                           while (match2.Success)
                           {
                               t_sid += "\""+ match2.Groups[0].Value+"\", ";
                               string r2 = @"(?<=" + match2.Groups[0].Value + @"\&kmws=""\>).+?(?=\<\/a\>)";
                               Match match3 = (new Regex(r2)).Match(html);
                               t_shop += "\""+ match3.Groups[0].Value+"\", ";
                               
                               match2 = match2.NextMatch();
                           }

                           t_shop = t_shop.Substring(0,t_shop.Length-2);
                           t_sid = t_sid.Substring(0, t_sid.Length - 2);

                           result22 += "new List<string> { " + t_shop + " },\n";
                           result22 += "new List<string> { " + t_sid + " })\n);\n";
                           
                       }

                       match = match.NextMatch();
                   }
                   setLogT(result22);
            


             //       setLogT(Form1.ToUrlEncode("北海道", System.Text.Encoding.GetEncoding("shift-jis")));

                    //     %9b%c1     %94%f2%94%f2          %83t          %83C%83q%83q      090      8619      3569
                    //"&sei=%9B%C1&mei=%94%F2%94%F2&sei_kana=%83T&mei_kana=%83C%83q%83q&tel1=090&tel2=8619&tel3=3569"

                    string x1 = Form1.ToUrlEncode("崔飛飛".Substring(0, 1), System.Text.Encoding.GetEncoding("shift-jis")),
                            x2 = Form1.ToUrlEncode("崔飛飛".Substring(1, "崔飛飛".Length - 1), System.Text.Encoding.GetEncoding("shift-jis")),
                            y1 = Form1.ToUrlEncode("サイヒヒ".Substring(0, 1), System.Text.Encoding.GetEncoding("shift-jis")),
                            y2 = Form1.ToUrlEncode("サイヒヒ".Substring(1, "サイヒヒ".Length - 1), System.Text.Encoding.GetEncoding("shift-jis")),
                            z1 = Regex.Match("090-8619-3569", @"\d+(?=\-)").Value,
                            z2 = Regex.Match("090-8619-3569", @"(?<=\d+\-)\d+(?=-)").Value,
                            z3 = Regex.Match("090-8619-3569", @"(?<=\d+\-\d+\-)\d+").Value;
                    setLogT(x1 + " " + x2 + " " + y1 + " " + y2 + " " + z1 + " " + z2 + " " + z3);




                    string pattern = @"^";
                    string replacement = "1-1-";
                    string result = Regex.Replace("12345", pattern, replacement);
                    setLogT(result);

                    rgx = @"(?<=aa).*?(?=aa)";
                    myMatch = (new Regex(rgx)).Match("qqqqqaaqwdsfaafferaafe222aa2222444aa444444222faaloveaa");
                    while (myMatch.Success)
                    {
                        setLogT(myMatch.Groups[0].Value);
                        myMatch = myMatch.NextMatch();
                    }

                    string message = "4344.34334.23.24.";
                    Regex rex = new Regex(@"^(\.|\d)+$");
                    if (rex.IsMatch(message))
                    {
                        //float result2 = float.Parse(message);
                        setLogT("match");
                    }
                    else
                        setLogT("not match");

                    int aa;
                    if ((aa = 4) == 4)
                    {
                        setLogT(aa.ToString());
                    }

                    Regex regex = new Regex(@"( ){2,}");
                    setLogT(regex.Replace("22      22", " "));
                    string[] s = regex.Replace(" abc def kkk   333 ppp ".Trim(), " ").Split(' ');
                    setLogT(s.Length.ToString());

                    string test2 = "abc";
                    //       testCall(test2);
                    setLogT(test2);

                    Appointment test3 = new Appointment("159", "", "", "", "");
                    testCall(ref test3);
                    setLogT(test3.CardNo);

                    setLogT("崔飛飛 " + Form1.ToUrlEncode("崔飛飛", System.Text.Encoding.GetEncoding("shift-jis")));//Shift_JIS     ??
                    setLogT("サイヒヒ " + Form1.ToUrlEncode("サイヒヒ"));
                    setLogT("090-8619-3569 " + Form1.ToUrlEncode("090-8619-3569"));


                string respHtml = Form1.weLoveYue(
                    this,
                    "https://aksale.advs.jp/cp/akachan_sale_pc/form_card_no.cgi"
                    ,
                    "POST",
                    "https://aksale.advs.jp/cp/akachan_sale_pc/mail_form.cgi",
                    false,
                    "card_no=" + "1234567890123" + "&sbmt=%8E%9F%82%D6",
                    ref cookieContainerForTest,
                    false
                    );
                       
                setLogT("崔飛飛 ".Length.ToString());


                    */


              });
            thread1111.Start();
           

        }
        void testCall(ref Client t)
        {
           // t = new Client("152", "", "", "", "","");
        }

        private void textBox1_keyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                autoB.PerformClick();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            gForceToStop = true;
            setLogtColorful(logT, "user operation: stop probing", Color.Red);
        }

        private void rate_Validating(object sender, CancelEventArgs e)
        {
            string str = rate.Text;
            if (str.Length > 0)
            {
                if (!Regex.Match(str, @"^\d+$").Success)
                {
                    e.Cancel = true;
                    MessageBox.Show("频率只能填写数字");
                }
            }
        }

        private void timeoutBox_Validating(object sender, CancelEventArgs e)
        {
            string str = timeoutBox.Text;
            if (str.Length > 0)
            {
                if (!Regex.Match(str, @"^\d+$").Success)
                {
                    e.Cancel = true;
                    MessageBox.Show("超时时间只能填写数字");
                    return;
                }
            }

            if (timeoutBox.Text.Equals(""))
            {
                
            }
            else
            {
                try
                {
                    if (Convert.ToInt32(timeoutBox.Text) > 100)
                    {
                        timeoutTime = Convert.ToInt32(timeoutBox.Text);
                        setLogT("set timeout time " + timeoutTime +"ms");
                    }
                    else
                    {
                        //维持程序默认值
                    }
                }
                catch (Exception)
                {
                    //维持程序默认值
                }
            }
        }

        

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex < Countylist.Count)
            {
                selecteCounty = Countylist[comboBox1.SelectedIndex];
                comboBox2.Items.Clear();
                for (int i = 0; i < selecteCounty.Shops.Count; i++)
                {
                    comboBox2.Items.Add(selecteCounty.Shops[i]);
                }
                comboBox2.SelectedIndex = 0;
            }
            else
            {
                selecteCounty = null;
            }
            
                
            //comboBox1.SelectedItem.ToString()
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedShop = comboBox2.SelectedIndex;
            showNextAppTime();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedType = comboBox3.SelectedIndex == 0 ? "6" : "7";//only made 6 / 7 for temp
            showNextAppTime();
        }

        private void showNextAppTime()
        {
            
        }

        private void appointmentGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public void deleteFormsF()
        {
            Login login = new Login(this, tempClient); //temp
            if (login.loginT() == 1)
            {
                Apply apply = new Apply(this, tempClient);
                apply.deleteForms();
            }
        }

        private void deleteForms_Click(object sender, EventArgs e)
        {
            if (tempClient != null)
            {
                tempClient.cookieContainer = null;
            }
            gForceToStop = false;
            Thread t = new Thread(deleteFormsF);
            t.Start();
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //使用无效信用卡
            tempClient.CardNumber = "4693960018321975";
            tempClient.CardHolder = "zhanghuimei";
            tempClient.CardVerificationCode = "111";
            tempClient.CardExpiryYear = "2018";
            tempClient.CardExpiryMonth = "04";
            tempClient.CardType = "visa" ;
            panel1.Enabled = false;
            cardExpiryMonth.Enabled = false;
            cardExpiryYear.Enabled = false;
            cardVerificationCode.Enabled = false;

            replaceInfo = checkBox1.Checked;
            checkBox1.Enabled = false;
            if (tempClient != null)
            {
                tempClient.cookieContainer = null;
            }
            gForceToStop = false;
            testButton = true;
            autoB.Visible = false;
            if (singleUser)
            {
                Thread t = new Thread(testButonClickF);
                t.Start();
            }
            else
            {
                Thread t = new Thread(auto);
                t.Start();
            }
            
        }
        string txnref = "5f0c6fc4_1_20170706";
        string TxnType = "Purchase";
        //string TxnType = "Void";


        //string TxnType = "Refund";
        private void button5_Click(object sender, EventArgs e)
        {
            CookieCollection cookie = new CookieCollection();
            
            string xml =
                 @"<Scr action=""doScrHIT"" user=""MomoTea_dev"" key=""25afcf399bff452ccc1971c151c3ee53a9e581dc5b3b718afe11283451aa9f42"">"
                + "<Amount>50.21</Amount>"
                + "<Cur>NZD</Cur>"
                + "<TxnType>"+ TxnType + "</TxnType>"
                + "<Station>4071309676</Station>"
                + "<TxnRef>"+ txnref + "</TxnRef>"
                + "<DeviceId>4071309676</DeviceId>"
                + "<PosName>MyPosName</PosName>"
                + "<PosVersion>MyPosVersion</PosVersion>"
                + "<VendorId>MyVendorId</VendorId>"
                + "<MRef>"+ txnref + "</MRef>"
                + "</Scr>";
            string respose = weLoveYue(
                this,
                "https://uat.paymentexpress.com/pxmi3/pos.aspx",
                "POST",
                "https://uat.paymentexpress.com/pxmi3/pos.aspx",
                true,
                xml,
                ref cookie,
                "uat.paymentexpress.com",
                true,
                true

                );

            this.setLogT(PrintXML(respose));
        }

        private void GetPosStatus_Click(object sender, EventArgs e)
        {
            CookieCollection cookie = new CookieCollection();

            XmlDocument xDoc = new XmlDocument();
            string xml =
                 @"<Scr action=""doScrHIT"" user=""MomoTea_dev"" key=""25afcf399bff452ccc1971c151c3ee53a9e581dc5b3b718afe11283451aa9f42"">"
                + "<TxnType>Status</TxnType>"
                + "<Station>4071309676</Station>"
                + "<TxnRef>"+ txnref + "</TxnRef>"
                + "</Scr>";
            xDoc.Load("C:\\SMS\\test.xml");
            string respose = weLoveYue(
                this,
                "https://uat.paymentexpress.com/pxmi3/pos.aspx",
                "POST",
                "https://uat.paymentexpress.com/pxmi3/pos.aspx",
                true,
                xml,
                ref cookie,
                "uat.paymentexpress.com",
                true,
                true

                );

            this.setLogT(PrintXML(respose));
        }
        public static String PrintXML(String XML)
        {
            String Result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(XML);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String FormattedXML = sReader.ReadToEnd();

                Result = FormattedXML;
            }
            catch (XmlException)
            {
            }

            mStream.Close();
            writer.Close();

            return Result;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CookieCollection cookie = new CookieCollection();

            string xml =
                 @"<Scr action=""doScrHIT"" user=""MomoTea_dev"" key=""25afcf399bff452ccc1971c151c3ee53a9e581dc5b3b718afe11283451aa9f42"">";
            xml += "<TxnType>UI</TxnType>"
                + "<UiType>Bn</UiType>"
                + "<Name>B2</Name>"
                + "<Val>NO</Val>"
                + "<Station>4071309676</Station>"
                + "<TxnRef>" + txnref + "</TxnRef>"
                + "</Scr>";
            string respose = weLoveYue(
                this,
                "https://uat.paymentexpress.com/pxmi3/pos.aspx",
                "POST",
                "https://uat.paymentexpress.com/pxmi3/pos.aspx",
                true,
                xml,
                ref cookie,
                "uat.paymentexpress.com",
                true,
                true

                );

            this.setLogT(PrintXML(respose));
        }

        /*
         // 只能控制写入尾部的字符 
        private void rate_TextChanged(object sender, EventArgs e)
        {
            string str = rate.Text;
            int len = str.Length;
            if (len > 0)
            {
                if (!Regex.Match(str.Substring(len - 1, 1), @"\d").Success)
                {
                    rate.Text = str.Substring(0, len - 1);
                }
            }
        }

        
         // 将导致输入内容无法删除
        private void rate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
             
        }
        */
    }

    









}
