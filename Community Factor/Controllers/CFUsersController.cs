using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Community_Factor;

namespace Community_Factor.Controllers
{
    public class CFUsersController : Controller
    {
        private PowerBI_UTILEntities db = new PowerBI_UTILEntities();

        // GET: CFUsers
        public ActionResult Index()
        {
            return View(db.CFUsers.ToList());
        }

        // GET: CFUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CFUser cFUser = db.CFUsers.Find(id);
            if (cFUser == null)
            {
                return HttpNotFound();
            }
            return View(cFUser);
        }

        // GET: CFUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CFUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,UserName,UserEmail,UserLostLogin,UserNetworkName,EmployeeId")] CFUser cFUser)
        {
            if (ModelState.IsValid)
            {
                db.CFUsers.Add(cFUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cFUser);
        }

        // GET: CFUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CFUser cFUser = db.CFUsers.Find(id);
            if (cFUser == null)
            {
                return HttpNotFound();
            }
            return View(cFUser);
        }

        // POST: CFUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,UserName,UserEmail,UserLostLogin,UserNetworkName,EmployeeId")] CFUser cFUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cFUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cFUser);
        }

        // GET: CFUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CFUser cFUser = db.CFUsers.Find(id);
            if (cFUser == null)
            {
                return HttpNotFound();
            }
            return View(cFUser);
        }

        // POST: CFUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CFUser cFUser = db.CFUsers.Find(id);
            db.CFUsers.Remove(cFUser);
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
