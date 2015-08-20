using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using drnosv6.Models;
using PagedList;
using System.Data.Objects.SqlClient;

namespace drnosv6.Controllers
{
    public class drnosController : Controller
    {
        private DrNOSv2Entities db = new DrNOSv2Entities();

        //
        // GET: /drnos/
      [HttpGet]
        public ActionResult Index(Doctor doctor, string sortOrder,  string currentFilter, string searchString, int? page)
        {

         


            string IDstring = "RVH_ID_".ToString();

            ViewBag.CurrentSort = sortOrder;
       

            ViewBag.LastSortParm = sortOrder == "LastName" ? "LastDesc" : "LastName";

          
            ViewBag.FirstSortParm = sortOrder == "FirstName" ? "FirstDesc" : "FirstName";


            ViewBag.IDSortParm = sortOrder == "IDstring" ? "RVHDesc" : "IDstring";

            ViewBag.QCPRSortParm = sortOrder == "QCPR" ? "QCPRDesc" : "QCPR";

            ViewBag.KeaneSortParm = sortOrder == "Keane" ? "KeaneDesc" : "Keane";

            ViewBag.OrsosSortParm = sortOrder == "Orsos" ? "OrsosDesc" : "Orsos";

            ViewBag.SoftSortParm = sortOrder == "Soft" ? "SoftDesc" : "Soft";

            ViewBag.threeMSortParm = sortOrder == "3M" ? "3MDesc" : "3M";

            var doctors = from d in db.Doctors
                          select d;


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            

            if (!String.IsNullOrEmpty(searchString)) //creates the code for the search bar
            {


                doctors = doctors.Where(d => d.Last_Name.ToUpper().Contains(searchString.ToUpper())
                    || d.First_Name.ToUpper().Contains(searchString.ToUpper())
                    || SqlFunctions.StringConvert((double)d.RVH_ID_).Contains(searchString));

                //d.RVH_ID_.ToString().ToUpper().Contains(searchString.ToUpper()));
            }
           

            switch (sortOrder) //creates a switch statement that decides which to order by. will default to the last name. this is attached to the search bar
            {
                case "LastName":
                    doctors = doctors.OrderBy(d => d.Last_Name);
                    break;

                case "LastDesc":
                    doctors = doctors.OrderByDescending(d => d.Last_Name);
                    break;

                case "FirstName":
                    doctors = doctors.OrderBy(d => d.First_Name);
                    break;

                case "FirstDesc":
                    doctors = doctors.OrderByDescending(d => d.First_Name);
                    break;

                case "RVH_ID_":
                    doctors = doctors.OrderBy(d => d.RVH_ID_);
                    break;

                case "RVHDesc":
                    doctors = doctors.OrderByDescending(d => d.RVH_ID_);

                    break;

                case "QCPRDesc":
                    doctors = doctors.OrderBy(d => d.QCPR);
                    break;

                case "KeaneDesc":
                    doctors = doctors.OrderBy(d => d.Keane);
                    break;

                case "OrsosDesc":
                    doctors = doctors.OrderBy(d => d.Orsos);
                    break;

                case "SoftDesc":
                    doctors = doctors.OrderBy(d => d.Orsos);
                    break;

                case "3MDesc":
                    doctors = doctors.OrderBy(d => d.C3M);
                    break;

                default: //automatically defautls to being ordered by last name. 
                    doctors = doctors.OrderBy(d => d.Last_Name);
                    break;
            }



            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(doctors.ToPagedList(pageNumber, pageSize));
        }
     

       

    

        [HttpPost]
        public ActionResult save(List<Doctor> drnos)
        {
           
            System.Diagnostics.Debug.WriteLine("Save Called");
            DrNOSv2Entities db = new DrNOSv2Entities();
            foreach (Doctor doc in drnos)
            {
               
                Doctor existing = db.Doctors.Find(doc.RVH_ID_);
                if (existing != null)
                {
                    System.Diagnostics.Debug.WriteLine("passed if");
                    existing.AdmPriv = doc.AdmPriv;
                    existing.QCPR = doc.QCPR;
                    existing.Keane = doc.Keane;
                    existing.Orsos = doc.Orsos;
                    existing.Soft = doc.Soft;
                    existing.C3M = doc.C3M;
                    
                }
            }
            db.SaveChanges();
            this.RedirectToAction("Index");
           //int pageSize = 100;
           //int pageNumber = (page ?? 1); 
            //return View(drnos.ToPagedList(pageNumber, pageSize));
            return View("Index");
        }
        
        public ActionResult Details(int id = 0)
        {
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }
        
        //
        // GET: /drnos/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /drnos/Create

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Doctors.Add(doctor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(doctor);
        }
        
        //
        // GET: /drnos/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        //
        // POST: /drnos/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }

        //
        // GET: /drnos/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        //
        // POST: /drnos/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Doctor doctor = db.Doctors.Find(id);
            db.Doctors.Remove(doctor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}