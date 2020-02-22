using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CSharpBeltExam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace CSharpBeltExam.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)       // User class
        {
            if(ModelState.IsValid)
            {
                if( dbContext.Users.Any( u => u.Email == newUser.Email ) )
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    dbContext.Users.Add(newUser);
                    dbContext.SaveChanges();

                    int userId = newUser.UserId;
                    HttpContext.Session.SetInt32("userId", userId);

                    return RedirectToAction("Dashboard");
                }
            }
            
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(Login user)         // Login data type
        {
            if(ModelState.IsValid)
            {
                User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == user.LoginEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email");
                    return View("Index");
                }

                var Hasher = new PasswordHasher<Login>();
                var result = Hasher.VerifyHashedPassword(user, userInDb.Password, user.LoginPassword);

                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Password");
                    return View("Index");
                }

                int userId = userInDb.UserId;
                HttpContext.Session.SetInt32("userId", userId);

                return RedirectToAction("Dashboard");
            }

            return View("Index");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetInt32("userId") == null)
            {
                // return RedirectToAction("Index");
                return View("Index");
            }

            List<Affair> AllAffairs = dbContext.Affairs
                .Include(w => w.ReserveSpot)
                .ThenInclude(r => r.User)
                .Include(affair => affair.Creator)
                .ToList();

            List<int> AffairsToDelete = new List<int>();
            DateTime CurrentTime = DateTime.Now;

            foreach(var affair in AllAffairs)
            {
                if(affair.AffairDate < CurrentTime)
                {
                    AffairsToDelete.Add(affair.AffairId);
                }
            }
            if(AffairsToDelete.Count > 0)
            {
                foreach(var affair in AffairsToDelete)
                {
                    Affair AffairToDelete = dbContext.Affairs.FirstOrDefault(aff => aff.AffairId == affair);
                    dbContext.Affairs.Remove(AffairToDelete);
                    dbContext.SaveChanges();
                }
            }

            int userId = (int)HttpContext.Session.GetInt32("userId");
            ViewBag.UserId = userId;
            ViewBag.User = dbContext.Users.FirstOrDefault(u => u.UserId == userId);

            int count = dbContext.Users
                .Include(u => u.Affairs)
                .ThenInclude(s => s.Affair)
                .Where(e => e.Affairs.Any(user => user.UserId == userId))
                .Count();

            ViewBag.Count = count;

            return View("Dashboard", AllAffairs); 
        }

        [HttpGet("activity/new")]
        public IActionResult CreateAffairView()
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            
            if( userId == null )
            {
                return View("Index");    // kick user out if no session id
            }

            return View("CreateAffair");
        }

        [HttpPost("activity/new")]
        public IActionResult CreateAffair(Affair newAffair)     // processes the form object from CreateWedding.cshtml
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            
            if( userId == null)
            {
                return RedirectToAction("Index");
            }
            if(ModelState.IsValid)
            {
                User user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
                newAffair.Creator = user;
                dbContext.Affairs.Add(newAffair);
                user.CreatedAffairs.Add(newAffair);
                dbContext.SaveChanges();

                ViewBag.UserId = userId;
                int affairId = newAffair.AffairId;

                return RedirectToAction("Details", new { affairId = affairId });
            }
            else
            {
                if(newAffair.AffairDate.Year == 1)
                {
                    if(ModelState.ContainsKey("AffairDate") == true)
                    {
                        ModelState["AffairDate"].Errors.Clear();
                    }
                    ModelState.AddModelError("AffairDate", "Date is Invalid");
                }

                return View("CreateAffair");
            }
        }

        [HttpGet("activity/details")]
        public IActionResult DetailsView()
        {
            if(HttpContext.Session.GetInt32("userId") == null)
            {
                return RedirectToAction("Index");
            }

            return View("Details");
        } 

        [HttpGet("activity/{affairId}")]
        public IActionResult Details(int affairId)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            User user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);

            if(userId == null)
            {
                return RedirectToAction("Index");
            }
            
            Affair OneAffair = dbContext.Affairs.FirstOrDefault(w => w.AffairId == affairId);
            User creator = OneAffair.Creator;
            
            List<User> usersThatReserved = dbContext.Users
                .Include(u => u.Affairs)
                .ThenInclude(v => v.Affair)
                .Where(w => w.Affairs.Any(wed => wed.AffairId == affairId))
                .ToList();

            ViewBag.usersThatReserved = usersThatReserved;
            ViewBag.User = user;
            ViewBag.FirstName = creator;
            ViewBag.UserID = userId;

            return View(OneAffair);
        }

        [HttpPost("activity/delete/{affairId}")]
        public IActionResult Delete(int affairId)
        {
            if (HttpContext.Session.GetInt32("userId") == null)
            {
                return RedirectToAction("Index");
            }

            Affair AffairToDelete = dbContext.Affairs.FirstOrDefault(w => w.AffairId == affairId);
            dbContext.Affairs.Remove(AffairToDelete);
            dbContext.SaveChanges();
            
            return RedirectToAction("Dashboard");
        }

        // Join 
        [HttpPost("activity/join/{affairId}")]
        public IActionResult Join(int affairId)
        {
            if (HttpContext.Session.GetInt32("userId") == null)
            {
                return RedirectToAction("Index");
            }

            Affair AffairToJoin = dbContext.Affairs.FirstOrDefault(w => w.AffairId == affairId);
            int userId = (int)HttpContext.Session.GetInt32("userId");
            User user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            
            UserAffair newUserAffair = new UserAffair
            {
                UserId = userId,
                AffairId = affairId,
                User = user,
                Affair = AffairToJoin,
            };

            dbContext.UsersAffairs.Add(newUserAffair);
            dbContext.SaveChanges();
            
            return RedirectToAction("Dashboard");
        }

        // Leave
        [HttpPost("activity/leave/{affairId}")]
        public IActionResult Leave(int affairId)
        {
            int userId = (int)HttpContext.Session.GetInt32("userId");
            
            if(HttpContext.Session.GetInt32("userId") == null)
            {
                return RedirectToAction("Index");
            }

            Affair AffairToReserve = dbContext.Affairs.FirstOrDefault(w => w.AffairId == affairId);
            User user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);

            UserAffair UserAffairToDelete = dbContext.UsersAffairs.FirstOrDefault(uw => uw.AffairId == affairId && uw.UserId == userId);
            dbContext.UsersAffairs.Remove(UserAffairToDelete);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }
    }
}