using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace ArunServiceStation.Controllers
{
    public class CustomersController : Controller
    {
        private  readonly ServiceStationEntities1 db = new ServiceStationEntities1();
        public static IEnumerable<Customer> Customer { get; set; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // GET: Customers
        public ActionResult Index()
        {
            var customers = db.Customers.Include(c => c.Category1);
            Customer = customers.ToList();
            return View(customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create(string AppointmentDate)
        {
            var customers = db.Customers.Include(c => c.Category1);
            Customer = customers.ToList();

            ViewBag.Category = new SelectList(db.Categories, "ID", "Catagory");
            ViewBag.CustData = Customer;

            //
            //if (AppointmentDate!=null)
            //{

            ViewBag.Timeslots = TimeslotCalcualte(AppointmentDate);
            //}
            //

            return View();
        }


        [HttpGet]
        public ActionResult Timeslot(string AppointmentDate)
        {
            if (!string.IsNullOrWhiteSpace(AppointmentDate))
            {
                IEnumerable<SelectListItem> regions = TimeslotCalcualte(AppointmentDate);
                var customers = db.Customers.Include(c => c.Category1);
                Customer = customers.ToList();


                //
                DateTime start = new DateTime(2020, 12, 17, 8, 0, 0);
                DateTime end = new DateTime(2020, 12, 17, 23, 0, 0);
                int i = 0;
                var cx = Customer;
                List<SelectListItem> list = new List<SelectListItem>();
                while (start.AddMinutes(60) <= end)
                {
                    var isdisable = false;
                    var slot = start.ToString("t") + " - " + start.AddMinutes(60).ToString("t");
                    if (AppointmentDate != "")
                    {
                        cx = Customer.Where(x => x.AppointmentDate == Convert.ToDateTime(AppointmentDate)).ToList();

                        var zz = Customer.Where(m => Regex.IsMatch(m.Timeslot, slot) && Regex.IsMatch(m.AppointmentDate.ToString(), AppointmentDate)).ToList();
                        if (Customer.Where(m => Regex.IsMatch(m.Timeslot, slot) && m.AppointmentDate == Convert.ToDateTime(AppointmentDate)).Count() > 0)
                        {
                            isdisable = true;

                        }
                        else
                        {
                            isdisable = false;
                        }
                    }
                    list.Add(new SelectListItem() { Text = slot, Value = slot, Disabled = isdisable });
                    start = start.AddMinutes(60);
                    i += 1;
                }
                var timeslot = list.Where(x => x.Disabled == false).ToList();
                IEnumerable<SelectListItem> timeSlots = timeslot
                                               .Select(iz => new SelectListItem()
                                               {
                                                   Text = iz.ToString(),
                                                   Value = iz.Value
                                               });
                return Json(timeslot, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        // GET: Customers/Create
        public IEnumerable<SelectListItem> TimeslotCalcualte(string AppointmentDate)
        {
            

            //
            DateTime start = new DateTime(2020, 12, 17, 8, 0, 0);
            DateTime end = new DateTime(2020, 12, 17, 23, 0, 0);
            int i = 0;
            // var cx= Customer;
            List<SelectListItem> list = new List<SelectListItem>();
            while (start.AddMinutes(60) <= end)
            {
                var isdisable = false;
                var slot = start.ToString("t") + " - " + start.AddMinutes(60).ToString("t");
                if (AppointmentDate != null)
                {
                   
                }
                list.Add(new SelectListItem() { Text = slot, Value = slot, Disabled = isdisable });
                start = start.AddMinutes(60);
                i += 1;
            }

            if (AppointmentDate != null)
            {
                list = list.Where(x => x.Disabled == false).Take(5).ToList();
                return list;
            }
            else
            {
                return list.Where(x => x.Disabled == false).ToList();
            }



        }
      

        public  IEnumerable<SelectListItem> GetAppointedDates(string AppointmentDate)
        {
            var customers = db.Customers.Include(c => c.Category1);
            Customer = customers.ToList();


            //
            DateTime start = new DateTime(2020, 12, 17, 8, 0, 0);
            DateTime end = new DateTime(2020, 12, 17, 23, 0, 0);
            int i = 0;
            var cx = Customer;
            List<SelectListItem> list = new List<SelectListItem>();
            while (start.AddMinutes(60) <= end)
            {
                var isdisable = false;
                var slot = start.ToString("t") + " - " + start.AddMinutes(60).ToString("t");
                if (AppointmentDate != "")
                {
                    cx = Customer.Where(x => x.AppointmentDate == Convert.ToDateTime(AppointmentDate)).ToList();

                    var zz = Customer.Where(m => Regex.IsMatch(m.Timeslot, slot) && Regex.IsMatch(m.AppointmentDate.ToString(), AppointmentDate)).ToList();
                    if (Customer.Where(m => Regex.IsMatch(m.Timeslot, slot) && m.AppointmentDate == Convert.ToDateTime(AppointmentDate)).Count() > 0)
                    {
                        isdisable = true;

                    }
                    else
                    {
                        isdisable = false;
                    }
                }
                list.Add(new SelectListItem() { Text = slot, Value = slot, Disabled = isdisable });
                start = start.AddMinutes(60);
                i += 1;
            }
            var Timeslots = list.Where(x => x.Disabled == false).ToList();
            IEnumerable<SelectListItem> myCollection = Timeslots
                                           .Select(iz => new SelectListItem()
                                           {
                                               Text = iz.ToString(),
                                               Value = iz.Value
                                           });
             return myCollection;
        }
        public void SendMail(Customer customer)
        {
            try {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("arunservicestation@gmail.com");
                mail.To.Add(customer.Email);
                mail.Subject = "Appointment Service Station";
                mail.Body = "<h1>Your Appointment is Confirmed</n></h1><h2>Vehile number: " + customer.VehicleNumber
                        +" </n>Date: " +customer.AppointmentDate
                        +" </n>Time: " +customer.Timeslot 
                        +"</h2>";
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {

                    smtp.Credentials = new NetworkCredential("arunservicestation@gmail.com", "1234qwer$");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }

            }
            }
            catch (Exception ex) 
            { 
                //LOG IT!!!   
                log.Error(string.Format("Mail Send Faild", customer.ID), ex);  
                throw;
            }
        }
       
        // POST: Customers/Create   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,VehicleNumber,MobileNumber,Email,Category,Provider,AppointmentDate,Timeslot")] Customer customer)
        {

            try { 
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                    SendMail(customer);
                    log.Info(string.Format("Appoinment Creation Completed : "+customer.VehicleNumber));

                    return RedirectToAction("Index");
            }

            ViewBag.Category = new SelectList(db.Categories, "ID", "Catagory", customer.Category);
            }
            catch (Exception e) {
                //LOG IT!!!  
                log.Error(string.Format("Create Failed Plese Check log", customer.ID), e);   
                throw;
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category = new SelectList(db.Categories, "ID", "Catagory", customer.Category);
            return View(customer);
        }

        // POST: Customers/Edit/5 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,VehicleNumber,MobileNumber,Email,Category,Provider,AppointmentDate,Timeslot")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category = new SelectList(db.Categories, "ID", "Catagory", customer.Category);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
