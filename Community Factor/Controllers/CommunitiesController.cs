using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;

namespace Community_Factor.Controllers
{
    [AllowAnonymous]
    public class CommunitiesController : Controller
    {
        private PowerBI_UTILEntities db = new PowerBI_UTILEntities();
        private Caliber_AMSNorthwestEntities dbCaliber = new Caliber_AMSNorthwestEntities();
        private ProcessRenewalDBEntities dbRenewal = new ProcessRenewalDBEntities();


        //[Authorize(Roles = "Assistant, SuperUser, Manager")]
        //public ActionResult SignIn()
        //{
        //    return RedirectToAction("Index", "Communities");
        //}

        // GET: Communities

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string refresh)
        {
            PRUser user = dbRenewal.PRUsers.SingleOrDefault(u => u.UserNetworkName == User.Identity.Name);

            if (user != null)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "CommunityName" : "";
                ViewBag.DateSortParm = sortOrder == "Factor" ? "Factor_Desc" : "Factor";

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                List<Client> clients = dbCaliber.Clients.Where(r => r.IsDeleted == false && r.InActive == false).ToList();
                List<Community> communities = db.Communities.ToList();
                var results = clients.Where(p => !communities.Any(p2 => p2.ClientId == p.ClientID));
                if (refresh != null && results.Count() > 0)
                {
                    DataTable destinationDataTable = ToDataTable(results.ToList());
                    string consString = ConfigurationManager.ConnectionStrings["AutomationDBConnection"].ConnectionString;
                    string TableName = "";
                    using (SqlConnection con = new SqlConnection(consString))
                    {
                        TableName = "Community";
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con, SqlBulkCopyOptions.FireTriggers, null) { DestinationTableName = "dbo." + TableName })
                        {
                            sqlBulkCopy.DestinationTableName = "dbo." + TableName;
                            using (DbContextTransaction transaction = db.Database.BeginTransaction())
                            {
                                sqlBulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("ClientId", "ClientId"));
                                con.Open();
                                sqlBulkCopy.WriteToServer(destinationDataTable);
                                transaction.Commit();
                            }
                        }

                    }

                }
                List<Community> listCommunities = db.Communities.ToList().
                                            Join(dbCaliber.Clients, u => u.ClientId, uir => uir.ClientID,
                                            (u, uir) => new { u, uir })
                                            .Where(r => r.uir.IsDeleted == false && r.uir.InActive == false)
                                            .Select(m => new Community
                                            {
                                                ClientId = m.u.ClientId,
                                                CommunityName = m.uir.ClientName,
                                                Factor = m.u.Factor,
                                                ID = m.u.ID
                                            }).ToList();
                if (!String.IsNullOrEmpty(searchString))
                {
                    listCommunities = listCommunities.Where(s => s.CommunityName.Contains(searchString)
                                           || s.CommunityName.Contains(searchString)).ToList();
                }
                switch (sortOrder)
                {
                    case "CommunityName":
                        listCommunities = listCommunities.OrderByDescending(s => s.CommunityName).ToList();
                        break;
                    case "Factor":
                        listCommunities = listCommunities.OrderBy(s => s.Factor).ToList();
                        break;
                    case "Factor_Desc":
                        listCommunities = listCommunities.OrderByDescending(s => s.Factor).ToList();
                        break;
                    default:  // Name ascending 
                        listCommunities = listCommunities.OrderBy(s => s.CommunityName).ToList();
                        break;
                }

                int pageSize = 50;
                int pageNumber = (page ?? 1);
                return View(listCommunities.ToPagedList(pageNumber, pageSize));
            }

            else
            {
                return RedirectToAction("Index", "AccessDenied");
            }



        }

        public static DataTable ToDataTable<Client>(List<Client> items)
        {
            DataTable dataTable = new DataTable(typeof(Client).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(Client).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (Client item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }


        // GET: Communities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<Community> communities = db.Communities.ToList()
                                        .Join(dbCaliber.Clients, u => u.ClientId, uir => uir.ClientID,
                                        (u, uir) => new { u, uir })
                                       .Where(r => r.u.ID == id)
                                        .Select(m => new Community
                                        {
                                            ClientId = m.u.ClientId,
                                            CommunityName = m.uir.ClientName,
                                            Factor = m.u.Factor,
                                            ID = m.u.ID
                                        }).ToList();//db.Communities.Find(id);
            Community community = communities.FirstOrDefault();

            if (community == null)
            {
                return HttpNotFound();
            }
            return View(community);
        }

        // GET: Communities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Communities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ClientId,Factor")] Community community)
        {
            if (ModelState.IsValid)
            {
                db.Communities.Add(community);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(community);
        }

        // GET: Communities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<Community> communities = db.Communities.ToList()
                                         .Join(dbCaliber.Clients, u => u.ClientId, uir => uir.ClientID,
                                         (u, uir) => new { u, uir })
                                        .Where(r => r.u.ID == id)
                                         .Select(m => new Community
                                         {
                                             ClientId = m.u.ClientId,
                                             CommunityName = m.uir.ClientName,
                                             Factor = m.u.Factor,
                                             ID = m.u.ID
                                         }).ToList();//db.Communities.Find(id);
            Community community = communities.FirstOrDefault();
            if (community == null)
            {
                return HttpNotFound();
            }
            return View(community);
        }

        // POST: Communities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ClientId,Factor")] Community community)
        {
            if (ModelState.IsValid)
            {
                db.Entry(community).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(community);
        }

        // GET: Communities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Community community = db.Communities.Find(id);
            if (community == null)
            {
                return HttpNotFound();
            }
            return View(community);
        }

        // POST: Communities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Community community = db.Communities.Find(id);
            db.Communities.Remove(community);
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
