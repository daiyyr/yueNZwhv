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
        public string Email { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string BithDateYear { get; set; }
        public string BithDateMonth { get; set; }
        public string BithDateDay { get; set; }
        public string PassportExpiryDateYear { get; set; }
        public string PassportExpiryDateMonth { get; set; }
        public string PassportExpiryDateDay { get; set; }
        public string NationalIdIssueDateYear { get; set; }
        public string NationalIdIssueDateMonth { get; set; }
        public string NationalIdIssueDateDay { get; set; }
        public string NationalIdExpiryDateYear { get; set; }
        public string NationalIdExpiryDateMonth { get; set; }
        public string NationalIdExpiryDateDay { get; set; }
        public string IntendedTravelDateYear { get; set; }
        public string IntendedTravelDateMonth { get; set; }
        public string IntendedTravelDateDay { get; set; }
        public string BeenToNz { get; set; }
        public string BeenToNzDateYear { get; set; }
        public string BeenToNzDateMonth { get; set; }
        public string BeenToNzDateDay { get; set; }
        public string PayerName { get; set; }
        
        public string __VIEWSTATE { get; set; }
        public string __VIEWSTATEGENERATOR { get; set; }
        public string __CMS_CurrentUrl { get; set; }
        public string __EVENTVALIDATION { get; set; }
        public string ApplicationId { get; set; }
        public string ctl00_ContentPlaceHolder1_personDetails_dateOfBithDatePicker_ControlState { get; set; }
        public CookieCollection cookieContainer = null;

        public Client(string userName, string password, string passportNo, string familyName, string givenName, string gender, 
            string email, string city, string street, string year, string month, string day, 
            string passportExpiryDateYear, string passportExpiryDateMonth, string passportExpiryDateDay,
            string nationalIdIssueDateYear, string nationalIdIssueDateMonth, string nationalIdIssueDateDay,
            string nationalIdExpiryDateYear, string nationalIdExpiryDateMonth, string nationalIdExpiryDateDay,
            string intendedTravelDateYear, string intendedTravelDateMonth, string intendedTravelDateDay,
            string beenToNz,
            string beenToNzDateYear, string beenToNzDateMonth, string beenToNzDateDay,
            string payerName
        )

        {
            UserName = userName;
            Password = password;
            PassportNo = passportNo;
            FamilyName = familyName;
            GivenName = givenName;
            Gender = gender; // F OR M
            Email = email;
            City = city;//居住城市英文名
            Street = street;//居住街道英文名
            BithDateYear = year;//出生年,4位
            BithDateMonth = month;//出生月, 不要使用占位0
            BithDateDay = day;//出生日, 不要使用占位0
            PassportExpiryDateYear = passportExpiryDateYear; //护照失效年,4位
            PassportExpiryDateMonth = passportExpiryDateMonth; //护照失效月, 不要使用占位0
            PassportExpiryDateDay = passportExpiryDateDay; //护照失效日, 不要使用占位0
            NationalIdIssueDateYear = nationalIdIssueDateYear; //身份证发放年, 4位
            NationalIdIssueDateMonth = nationalIdIssueDateMonth; //身份证发放月, 不要使用占位0
            NationalIdIssueDateDay = nationalIdIssueDateDay; 
            NationalIdExpiryDateYear = nationalIdExpiryDateYear; //身份证失效年, 4位
            NationalIdExpiryDateMonth = nationalIdExpiryDateMonth; //身份证失效月, 不要使用占位0
            NationalIdExpiryDateDay = nationalIdExpiryDateDay; //身份证失效日, 不要使用占位0
            IntendedTravelDateYear = intendedTravelDateYear;//计划入境日期年, 不要使用占位0
            IntendedTravelDateMonth = intendedTravelDateMonth;//计划入境日期月, 不要使用占位0
            IntendedTravelDateDay = intendedTravelDateDay;//计划入境日期日, 不要使用占位0
            BeenToNz = beenToNz;//是否到过纽西兰, 填 Yes 或者 No 注意大小写
            BeenToNzDateYear = beenToNzDateYear;//如果到过纽西兰,上一次入境时间年, 不要使用占位0
            BeenToNzDateMonth = beenToNzDateMonth;//如果到过纽西兰,上一次入境时间月, 不要使用占位0
            BeenToNzDateDay = beenToNzDateDay;//如果到过纽西兰,上一次入境时间日, 不要使用占位0
            PayerName = payerName;//付款人姓名, 随便填写
        }
    }
}
