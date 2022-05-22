using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Login.Models;
using System.Data.SqlClient;
using System.Data;
using OfficeOpenXml;
using System.Drawing;
using System.IO;
using Microsoft.Win32;
using System.Text;
using System.Web.UI;
using Microsoft.SharePoint.Client.Search.Query;
using System.Windows.Controls;
using System.Web.Security;
using System.Globalization;

namespace Login.Controllers
{

    public class AccountController : Controller
    {
        
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        void connectionString()
        {
            //con.ConnectionString = "Data Source=PERSONALSRV-KAR\\SQL2016;Initial Catalog=of1; Persist Security Info=True;User ID=sa;Password=12341234; MultipleActiveResultSets=True;";
            //con.ConnectionString = "Data Source=(local);Initial Catalog=of1;Integrated Security=True;";
            con.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=of1;Integrated Security=True;";

        }
        [HttpGet]
        
        public ActionResult Login()
        {
            
            return View();
        }
       
        public ActionResult Login2()
        {
            return View();
        }
      
        public ActionResult logout()
        {

            return View();
        }
        
       
        public ActionResult timeout()
        {

            return View();
        }
     

 
        [HttpPost]
        public ActionResult verify(UserAccounts acc)
        {
            DataTable dt = new DataTable();
            connectionString();
            con.Open();
            com.Connection = con;
            var ff = acc.Username;
            TempData["idd"] = ff;
            TempData.Keep("idd");
            ViewBag.username1 = TempData["idd"];
            //usertable
            com.CommandText = "SELECT * FROM [of1].[dbo].[User] where Username='" + acc.Username + "' and Pass='" + acc.Pass + "'";
           //
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                con.Close();
                getDropDown();
                return View("getDropDown");
            }
            else
            {
                con.Close();
                return View("Error");
            }

        }
        //conectionchange
        of1Entities1 db = new of1Entities1();
        
        //
        [Authorize]
        public ActionResult getDropDown()
        {
            if (Request.QueryString["action"] != null)
            {
                Response.Clear();
                Session.Abandon();
                Response.Write("Success");
                Response.End();
            }
            List<year> yearlist = db.years.ToList();
            List<month> monthlist = db.months.ToList();
            ViewBag.yearlist = new SelectList(yearlist, "id", "yearname", "yearnum");
            ViewBag.monthlist = new SelectList(monthlist, "id", "monthname", "monthnum");
            ViewBag.username = TempData["idd"];
            return View();
        }
        
        [HttpPost]
        public void submit( Yearmonth yearmonthh)
        {

            //conectionchange
            of1Entities1 db = new of1Entities1();
            //
            ViewBag.mmm = yearmonthh.selmonthId;
            ViewBag.yyy = yearmonthh.selyearId;
            var t = yearmonthh.selyearId;
            var t2 = yearmonthh.selmonthId;

           string yenum= (from years in db.years
            where
              years.id == t
             select new
            {
                yenum=years.yearnum
             }).ToList().FirstOrDefault().yenum;
            string monum = (from months in db.months
                       where
                         months.id == t2
                       select new
                       {
                           monthnum = months.monthnum
                       }).ToList().FirstOrDefault().monthnum;
            string conc = yenum + monum;
            ViewBag.uu= conc;
            var Userid = TempData["idd"];
            TempData.Keep("idd");
            DataTable dt = new DataTable();
            //usertable
            var compst = (from Users in db.Users
                          where
                            Users.Username == Userid
                          select new
                          {
                              Users.CompanyStatus
                          }).FirstOrDefault().ToString();
            //
            TempData["compst"] = compst;
            TempData.Keep("compst");
            //empltable+ usertable
            if (compst.Contains("1"))
            {
                List<Employee> query1 = (from Employees in db.Employees
                                        where
                                              (from Users in db.Users
                                               where
                                     Users.Username == Userid
                                               select new
                                               {
                                                   Users.CompanyCode
                                               }).Contains(new { CompanyCode = Employees.COMPANY_CODE }) &&
              Employees.year.Contains(conc)
                                        select Employees).ToList();
               
                //return View(query1);
                ExcelPackage p1 = new ExcelPackage();
                ExcelWorksheet ew = p1.Workbook.Worksheets.Add("Report");
                ew.Cells["A2"].Value = "Report";
                ew.Cells["B2"].Value = "Report1";
                ew.Cells["A3"].Value = DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("fa-IR"));
                ew.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);
                //emptable
                ew.Cells["A6"].Value = "1001-کد ملی";
                ew.Cells["B6"].Value = "2001-سطح پست";
                ew.Cells["C6"].Value = "2002-مدرک تحصیلی";
                ew.Cells["D6"].Value = "2003-نوع استخدام";
                ew.Cells["E6"].Value = "2004-رتبه شغلی";
                ew.Cells["F6"].Value = "2005-سوابق خدمت به سال";
                ew.Cells["G6"].Value = "6626-حقوق پایه";
                ew.Cells["H6"].Value = "6352-دستمزد پایه";
                ew.Cells["I6"].Value = "5184-تفاوت تطبیق";
                ew.Cells["J6"].Value = "6373-کسر مازاد بر سقف حقوق";
                ew.Cells["K6"].Value = "6356-فوق العاده کارگاهی";
                ew.Cells["L6"].Value = "5185-فوق العاده ویژه";
                ew.Cells["M6"].Value = "6357-جذب 8%";
                ew.Cells["N6"].Value = "6358-فوق العاده نوع برنامه کار";
                ew.Cells["O6"].Value = "6369-فوق العاده نوع شغل";
                ew.Cells["P6"].Value = "6359-فوق العاده شرایط محیط کار";
                ew.Cells["Q6"].Value = "6362-فوق العاده جذب و نگهداری ممتازین و مشاغل ویژه";
                ew.Cells["R6"].Value = "6363-فوق العاده محل خدمت و زندگی";
                ew.Cells["S6"].Value = "5181-فوق العاده ایثارگری";
                ew.Cells["T6"].Value = "6299-فوق العاده ماموریت";
                ew.Cells["U6"].Value = "6178-کمک هزینه مسکن";
                ew.Cells["V6"].Value = "6177-کمک هزینه اقلام مصرفی خانوار";
                ew.Cells["W6"].Value = "6364-کمک هزینه عائله مندی و اولاد";
                ew.Cells["X6"].Value = "6365-حق الزحمه / کارانه مصوب";
                ew.Cells["Y6"].Value = "6353-کسر مازاد بر سقف";
                ew.Cells["Z6"].Value = "5192-سایر";
                ew.Cells["AA6"].Value = "6366-عیدی سالانه";
                ew.Cells["AB6"].Value = "6262-پاداش عملکرد";
                ew.Cells["AC6"].Value = "6367-جبران هزینه های کارکنان";
                ew.Cells["AD6"].Value = "6207-بیمه تامین اجتماعی";
                ew.Cells["AE6"].Value = "6206-بازنشستگی";
                ew.Cells["AF6"].Value = "6372-بیمه درمان تکمیلی";
                ew.Cells["AG6"].Value = "6368-بیمه عمر و تامین آتیه";
                ew.Cells["AH6"].Value = "5207-مالیات";

                int rowStart = 7;
                foreach (var item in query1)
                {
                    ew.Cells[String.Format("A{0}", rowStart)].Value = item.NATIONAL_NO;
                    ew.Cells[String.Format("B{0}", rowStart)].Value = item.Post_Grade;
                    ew.Cells[String.Format("C{0}", rowStart)].Value = item.EDUCAT_LEVEL;
                    ew.Cells[String.Format("D{0}", rowStart)].Value = item.EMPLOYM_TYPE;
                    ew.Cells[String.Format("E{0}", rowStart)].Value = item.Job_Grade;
                    ew.Cells[String.Format("F{0}", rowStart)].Value = item.Service_Year;
                    ew.Cells[String.Format("G{0}", rowStart)].Value = item.C6626;
                    ew.Cells[String.Format("H{0}", rowStart)].Value = item.C6352;
                    ew.Cells[String.Format("I{0}", rowStart)].Value = item.C5184;
                    ew.Cells[String.Format("J{0}", rowStart)].Value = item.C6373;
                    ew.Cells[String.Format("K{0}", rowStart)].Value = item.C6356;
                    ew.Cells[String.Format("L{0}", rowStart)].Value = item.C5185;
                    ew.Cells[String.Format("M{0}", rowStart)].Value = item.C6357;
                    ew.Cells[String.Format("N{0}", rowStart)].Value = item.C6358;
                    ew.Cells[String.Format("O{0}", rowStart)].Value = item.C6369;
                    ew.Cells[String.Format("P{0}", rowStart)].Value = item.C6359;
                    ew.Cells[String.Format("Q{0}", rowStart)].Value = item.C6362;
                    ew.Cells[String.Format("R{0}", rowStart)].Value = item.C6363;
                    ew.Cells[String.Format("S{0}", rowStart)].Value = item.C5181;
                    ew.Cells[String.Format("T{0}", rowStart)].Value = item.C6299;
                    ew.Cells[String.Format("U{0}", rowStart)].Value = item.C6178;
                    ew.Cells[String.Format("V{0}", rowStart)].Value = item.C6177;
                    ew.Cells[String.Format("W{0}", rowStart)].Value = item.C6364;
                    ew.Cells[String.Format("X{0}", rowStart)].Value = item.C6365;
                    ew.Cells[String.Format("Y{0}", rowStart)].Value = item.C6353;
                    ew.Cells[String.Format("Z{0}", rowStart)].Value = item.C5192;
                    ew.Cells[String.Format("AA{0}", rowStart)].Value = item.C6366;
                    ew.Cells[String.Format("AB{0}", rowStart)].Value = item.C6262;
                    ew.Cells[String.Format("AC{0}", rowStart)].Value = item.C6367;
                    ew.Cells[String.Format("AD{0}", rowStart)].Value = item.C6207;
                    ew.Cells[String.Format("AE{0}", rowStart)].Value = item.C6206;
                    ew.Cells[String.Format("AF{0}", rowStart)].Value = item.C6372;
                    ew.Cells[String.Format("AG{0}", rowStart)].Value = item.C6368;
                    ew.Cells[String.Format("AH{0}", rowStart)].Value = item.C5207;
                    rowStart++;
                }
                //
                ew.Cells["A:AZ"].AutoFitColumns();
                string filename = "Results_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
                Response.Clear();
                Response.ContentType = "application/vnd.ms-excel";
                Response.ContentType = "application/vnd.ms-excel";
                   ;
                Response.AddHeader("Content-Disposition", "attachment; filename=Report" + DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")) + ".xlsx");

                Response.ContentEncoding = Encoding.UTF8;
                StringWriter stringWriter = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(stringWriter);
                Response.Write(stringWriter.ToString());
                Response.BinaryWrite(p1.GetAsByteArray());
                Response.End();
            }

            else
            {
                //empltable+ usertable
                List<Employee> query2 = (from Employees in db.Employees
                                        where
                                              (from Users in db.Users
                                               where
                                                Users.Username == Userid
                                               select new
                                               {
                                                   Users.PayrollCode
                                               }).Contains(new { PayrollCode = Employees.PYRLCMP_CODE }) &&
              Employees.year.Contains(conc)
                                        select Employees).ToList();
                //
                //return View(query2);
                ExcelPackage p1 = new ExcelPackage();
                ExcelWorksheet ew = p1.Workbook.Worksheets.Add("Report");
                ew.Cells["A2"].Value = "Report";
                ew.Cells["B2"].Value = "Report1";
                ew.Cells["A3"].Value = DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("fa-IR"));
                ew.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);
                //emptable
                ew.Cells["A6"].Value = "1001-کد ملی";
                ew.Cells["B6"].Value = "2001-سطح پست";
                ew.Cells["C6"].Value = "2002-مدرک تحصیلی";
                ew.Cells["D6"].Value = "2003-نوع استخدام";
                ew.Cells["E6"].Value = "2004-رتبه شغلی";
                ew.Cells["F6"].Value = "2005-سوابق خدمت به سال";
                ew.Cells["G6"].Value = "6626-حقوق پایه";
                ew.Cells["H6"].Value = "6352-دستمزد پایه";
                ew.Cells["I6"].Value = "5184-تفاوت تطبیق";
                ew.Cells["J6"].Value = "6373-کسر مازاد بر سقف حقوق";
                ew.Cells["K6"].Value = "6356-فوق العاده کارگاهی";
                ew.Cells["L6"].Value = "5185-فوق العاده ویژه";
                ew.Cells["M6"].Value = "6357-جذب 8%";
                ew.Cells["N6"].Value = "6358-فوق العاده نوع برنامه کار";
                ew.Cells["O6"].Value = "6369-فوق العاده نوع شغل";
                ew.Cells["P6"].Value = "6359-فوق العاده شرایط محیط کار";
                ew.Cells["Q6"].Value = "6362-فوق العاده جذب و نگهداری ممتازین و مشاغل ویژه";
                ew.Cells["R6"].Value = "6363-فوق العاده محل خدمت و زندگی";
                ew.Cells["S6"].Value = "5181-فوق العاده ایثارگری";
                ew.Cells["T6"].Value = "6299-فوق العاده ماموریت";
                ew.Cells["U6"].Value = "6178-کمک هزینه مسکن";
                ew.Cells["V6"].Value = "6177-کمک هزینه اقلام مصرفی خانوار";
                ew.Cells["W6"].Value = "6364-کمک هزینه عائله مندی و اولاد";
                ew.Cells["X6"].Value = "6365-حق الزحمه / کارانه مصوب";
                ew.Cells["Y6"].Value = "6353-کسر مازاد بر سقف";
                ew.Cells["Z6"].Value = "5192-سایر";
                ew.Cells["AA6"].Value = "6366-عیدی سالانه";
                ew.Cells["AB6"].Value = "6262-پاداش عملکرد";
                ew.Cells["AC6"].Value = "6367-جبران هزینه های کارکنان";
                ew.Cells["AD6"].Value = "6207-بیمه تامین اجتماعی";
                ew.Cells["AE6"].Value = "6206-بازنشستگی";
                ew.Cells["AF6"].Value = "6372-بیمه درمان تکمیلی";
                ew.Cells["AG6"].Value = "6368-بیمه عمر و تامین آتیه";
                ew.Cells["AH6"].Value = "5207-مالیات";

                int rowStart = 7;
                foreach (var item in query2)
                {
                    ew.Cells[String.Format("A{0}", rowStart)].Value = item.NATIONAL_NO;
                    ew.Cells[String.Format("B{0}", rowStart)].Value = item.Post_Grade;
                    ew.Cells[String.Format("C{0}", rowStart)].Value = item.EDUCAT_LEVEL;
                    ew.Cells[String.Format("D{0}", rowStart)].Value = item.EMPLOYM_TYPE;
                    ew.Cells[String.Format("E{0}", rowStart)].Value = item.Job_Grade;
                    ew.Cells[String.Format("F{0}", rowStart)].Value = item.Service_Year;
                    ew.Cells[String.Format("G{0}", rowStart)].Value = item.C6626;
                    ew.Cells[String.Format("H{0}", rowStart)].Value = item.C6352;
                    ew.Cells[String.Format("I{0}", rowStart)].Value = item.C5184;
                    ew.Cells[String.Format("J{0}", rowStart)].Value = item.C6373;
                    ew.Cells[String.Format("K{0}", rowStart)].Value = item.C6356;
                    ew.Cells[String.Format("L{0}", rowStart)].Value = item.C5185;
                    ew.Cells[String.Format("M{0}", rowStart)].Value = item.C6357;
                    ew.Cells[String.Format("N{0}", rowStart)].Value = item.C6358;
                    ew.Cells[String.Format("O{0}", rowStart)].Value = item.C6369;
                    ew.Cells[String.Format("P{0}", rowStart)].Value = item.C6359;
                    ew.Cells[String.Format("Q{0}", rowStart)].Value = item.C6362;
                    ew.Cells[String.Format("R{0}", rowStart)].Value = item.C6363;
                    ew.Cells[String.Format("S{0}", rowStart)].Value = item.C5181;
                    ew.Cells[String.Format("T{0}", rowStart)].Value = item.C6299;
                    ew.Cells[String.Format("U{0}", rowStart)].Value = item.C6178;
                    ew.Cells[String.Format("V{0}", rowStart)].Value = item.C6177;
                    ew.Cells[String.Format("W{0}", rowStart)].Value = item.C6364;
                    ew.Cells[String.Format("X{0}", rowStart)].Value = item.C6365;
                    ew.Cells[String.Format("Y{0}", rowStart)].Value = item.C6353;
                    ew.Cells[String.Format("Z{0}", rowStart)].Value = item.C5192;
                    ew.Cells[String.Format("AA{0}", rowStart)].Value = item.C6366;
                    ew.Cells[String.Format("AB{0}", rowStart)].Value = item.C6262;
                    ew.Cells[String.Format("AC{0}", rowStart)].Value = item.C6367;
                    ew.Cells[String.Format("AD{0}", rowStart)].Value = item.C6207;
                    ew.Cells[String.Format("AE{0}", rowStart)].Value = item.C6206;
                    ew.Cells[String.Format("AF{0}", rowStart)].Value = item.C6372;
                    ew.Cells[String.Format("AG{0}", rowStart)].Value = item.C6368;
                    ew.Cells[String.Format("AH{0}", rowStart)].Value = item.C5207;
                    rowStart++;
                }
                //
                ew.Cells["A:AZ"].AutoFitColumns();
                string filename = "Results_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
                Response.Clear();
                Response.ContentType = "application/vnd.ms-excel";
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment; filename=Report" + DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")) + ".xlsx");
                Response.ContentEncoding = Encoding.UTF8;
                StringWriter stringWriter = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(stringWriter);
                Response.Write(stringWriter.ToString());
                Response.BinaryWrite(p1.GetAsByteArray());
                Response.End();
            }
            ViewBag.yy = yenum;
            ViewBag.yy2 = monum;
            //return View();
        }

        public void submit2(Yearmonth yearmonthh)
        {

            //conectionchange
            of1Entities1 db = new of1Entities1();
            //
            ViewBag.mmm = yearmonthh.selmonthId;
            ViewBag.yyy = yearmonthh.selyearId;
            var t = yearmonthh.selyearId;
            var t2 = yearmonthh.selmonthId;

            string yenum = (from years in db.years
                            where
                              years.id == t
                            select new
                            {
                                yenum = years.yearnum
                            }).ToList().FirstOrDefault().yenum;
            string monum = (from months in db.months
                            where
                              months.id == t2
                            select new
                            {
                                monthnum = months.monthnum
                            }).ToList().FirstOrDefault().monthnum;
            string conc = yenum + monum;
            ViewBag.uu = conc;

            var Userid = TempData["idd"];
            TempData.Keep("idd");
            DataTable dt = new DataTable();
            //usertable
            var compst = (from Users in db.Users
                          where
                            Users.Username == Userid
                          select new
                          {
                              Users.CompanyStatus
                          }).FirstOrDefault().ToString();
            //
            TempData["compst"] = compst;
            TempData.Keep("compst");

            //empltable+ usertable
            if (compst.Contains("1"))
            {
                List<Employee> query1 = (from Employees in db.Employees
                                         where
                                               (from Users in db.Users
                                                where
                                      Users.Username == Userid
                                                select new
                                                {
                                                    Users.CompanyCode
                                                }).Contains(new { CompanyCode = Employees.COMPANY_CODE }) &&
               Employees.year.Contains(conc)
                                         select Employees).ToList();

                //return View(query1);
                ExcelPackage p1 = new ExcelPackage();
                ExcelWorksheet ew = p1.Workbook.Worksheets.Add("Report");
                ew.Cells["A2"].Value = "Report";
                ew.Cells["B2"].Value = "Report1";
                ew.Cells["A3"].Value = DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("fa-IR"));
                ew.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);
                //emptable
                ew.Cells["A6"].Value = " 1001 - کدملی";
                ew.Cells["B6"].Value = "1002 - نام";
                ew.Cells["C6"].Value = "1003 - نام خانوادگی";
                ew.Cells["D6"].Value = "1004 - نام پدر";
                ew.Cells["E6"].Value = "1005 - شماره شناسنامه";
                ew.Cells["F6"].Value = "1006 - جنسیت";
                ew.Cells["G6"].Value = "1007 - تاریخ تولد";
                int rowStart = 7;
                foreach (var item in query1)
                {
                    ew.Cells[String.Format("A{0}", rowStart)].Value = item.NATIONAL_NO;
                    ew.Cells[String.Format("B{0}", rowStart)].Value = item.NAME;
                    ew.Cells[String.Format("C{0}", rowStart)].Value = item.Fname;
                    ew.Cells[String.Format("D{0}", rowStart)].Value = item.Father_Name;
                    ew.Cells[String.Format("E{0}", rowStart)].Value = item.ID_No;
                    ew.Cells[String.Format("F{0}", rowStart)].Value = item.Sex_Code;
                    ew.Cells[String.Format("G{0}", rowStart)].Value = item.Birth_Date;
                    rowStart++;
                }
                //
                ew.Cells["A:AZ"].AutoFitColumns();
                string filename = "Results_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
                Response.Clear();
                Response.ContentType = "application/vnd.ms-excel";
                Response.ContentType = "application/vnd.ms-excel";
                
                Response.AddHeader("Content-Disposition", "attachment; filename=Report" + DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")) + ".xlsx");

                Response.ContentEncoding = Encoding.UTF8;
                StringWriter stringWriter = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(stringWriter);
                Response.Write(stringWriter.ToString());
                Response.BinaryWrite(p1.GetAsByteArray());
                Response.End();
            }

            else
            {
                //empltable+ usertable
                List<Employee> query2 = (from Employees in db.Employees
                                         where
                                               (from Users in db.Users
                                                where
                                                 Users.Username == Userid
                                                select new
                                                {
                                                    Users.PayrollCode
                                                }).Contains(new { PayrollCode = Employees.PYRLCMP_CODE }) &&
               Employees.year.Contains(conc)
                                         select Employees).ToList();
                //
                //return View(query2);
                ExcelPackage p1 = new ExcelPackage();
                ExcelWorksheet ew = p1.Workbook.Worksheets.Add("Report");
                ew.Cells["A2"].Value = "Report";
                ew.Cells["B2"].Value = "Report1";
                ew.Cells["A3"].Value = DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("fa-IR"));
                ew.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);
                //emptable
                ew.Cells["A6"].Value = "NATIONAL_NO";
                ew.Cells["B6"].Value = "NAME";
                ew.Cells["C6"].Value = "FNAME";
                ew.Cells["D6"].Value = "Father_Name";
                ew.Cells["E6"].Value = "ID_No";
                ew.Cells["F6"].Value = "Sex_Code";
                ew.Cells["G6"].Value = "Birth_Date";
                int rowStart = 7;
                foreach (var item in query2)
                {
                    ew.Cells[String.Format("A{0}", rowStart)].Value = item.NATIONAL_NO;
                    ew.Cells[String.Format("B{0}", rowStart)].Value = item.NAME;
                    ew.Cells[String.Format("C{0}", rowStart)].Value = item.Fname;
                    ew.Cells[String.Format("D{0}", rowStart)].Value = item.Father_Name;
                    ew.Cells[String.Format("E{0}", rowStart)].Value = item.ID_No;
                    ew.Cells[String.Format("F{0}", rowStart)].Value = item.Sex_Code;
                    ew.Cells[String.Format("G{0}", rowStart)].Value = item.Birth_Date;
                    rowStart++;
                }
                //
                ew.Cells["A:AZ"].AutoFitColumns();
                string filename = "Results_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
                Response.Clear();
                Response.ContentType = "application/vnd.ms-excel";
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment; filename=Report" + DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")) + ".xlsx");

                Response.ContentEncoding = Encoding.UTF8;
                StringWriter stringWriter = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(stringWriter);
                Response.Write(stringWriter.ToString());
                Response.BinaryWrite(p1.GetAsByteArray());
                Response.End();
            }
            ViewBag.yy = yenum;
            ViewBag.yy2 = monum;
            //return View();
        }
    }
}