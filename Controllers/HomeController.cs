using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wedding_Planner2.Models;

namespace Wedding_Planner2.Controllers
{
    public class HomeController : Controller
    {
        private WeddingPlannerContext dbContext;
        public HomeController (WeddingPlannerContext context)
        {
            dbContext = context;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use!");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                // Save your user object to database
                User NewUser = new User
                {
                    First_Name = @user.First_Name,
                    Last_Name = @user.Last_Name,
                    Email = @user.Email,
                    Password = @user.Password,
                };
                var userEntity = dbContext.Add(NewUser).Entity;
                dbContext.SaveChanges();
                return RedirectToAction("Welcome");
            }

            return View("Index");
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(User userSubmission)
        {
            // if initial ModelState is valid, query for user with the provided email.
            var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
            // if no user exisits with provided email....
            if(userInDb == null)
            {
                // Add an error to ModelState and return to View!
                ModelState.AddModelError("Email", "This email does not exisit within our records :(");
                return View("Index");
            }
            // Initialize hasher object.
            var hasher = new PasswordHasher<User>();
            // Verify provided password againest hash store in DB.
            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
            if(result == 0)
            {
                Console.WriteLine("Invaild Password");
                ModelState.AddModelError("Password", "Invaild Password");
                return View("Index");
            }
            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            return RedirectToAction("Welcome");
        }
        [HttpGet]
        [Route("Welcome")]
        public IActionResult Welcome()
        {
            var weddings = dbContext.Weddings
            .Include(w => w.RSVPs)
            .OrderByDescending(w => w.Date);

            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");

            var responded = weddings.Where(w => w.RSVPs.Any(r => r.UserId == 1)); 

            return View("Welcome", weddings);
        }
        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("NewWedding")]
        public IActionResult NewWedding()
        {
            return View("NewWedding");
        }
        [HttpPost]
        [Route("CreateWedding")]
        public IActionResult CreateWedding(Wedding new_wedding)
        {
            if(ModelState.IsValid)
            {
                if(new_wedding.Date < DateTime.Today)
                {
                    ModelState.AddModelError("Date", "Date must be in the future!");
                    return View("NewWedding");
                }
                else
                {
                    Wedding this_wedding = new Wedding
                    {
                        Address = new_wedding.Address,
                        Date = new_wedding.Date,
                        WedderOne = new_wedding.WedderOne,
                        WedderTwo = new_wedding.WedderTwo,
                        UserId = (int) HttpContext.Session.GetInt32("UserId")
                    };
                    dbContext.Add(this_wedding);
                    dbContext.SaveChanges();
                    return RedirectToAction("Welcome");
                }
            }
            else
            {
                if(new_wedding.Date < DateTime.Today)
                {
                    ModelState.AddModelError("Date","Date must be in the future!");
                }
                return View("NewWedding");
            }
        }
        [HttpGet]
        [Route("ViewWedding/{weddingId}")]
        public IActionResult ViewWedding(int weddingId)
        {
            Wedding wedding = dbContext.Weddings
            .Include(r => r.RSVPs)
            .ThenInclude(u => u.User)
            .Where(w => w.WeddingId == weddingId)
            .SingleOrDefault();

            ViewBag.Wedding = wedding;
            ViewBag.Address = wedding.Address;
            return View("ViewWedding");

        }
        [Route("RSVP")]
        public IActionResult RSVP(int weddingId)
        {
            RSVP new_rsvp = new RSVP
            {
                UserId = (int) HttpContext.Session.GetInt32("UserId"),
                WeddingId = weddingId
            };
            dbContext.Add(new_rsvp);
            dbContext.SaveChanges();

            return RedirectToAction("Welcome");

        }
        [Route("UnRSVP")]
        public IActionResult UnRSVP(int weddingId)
        {
            RSVP this_attender = dbContext.RSVP
            .SingleOrDefault(u => u.UserId == HttpContext.Session
            .GetInt32("UserId") && u.WeddingId == weddingId);

            dbContext.RSVP.Remove(this_attender);
            dbContext.SaveChanges();
            return RedirectToAction("Welcome");
        }
        [Route("Delete")]
        public IActionResult Delete(int weddingId) 
        {
            Wedding this_wedding = dbContext.Weddings
            .SingleOrDefault(w => w.WeddingId == weddingId);

            List<RSVP> rsvps = dbContext.RSVP
            .Where(a => a.WeddingId == weddingId)
            .ToList();

            foreach(var attender in rsvps)
            {
                dbContext.RSVP.Remove(attender);
            } 
            dbContext.Weddings.Remove(this_wedding);
            dbContext.SaveChanges();

            return RedirectToAction("Welcome");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
