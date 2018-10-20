using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using brightideas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace brightideas.Controllers
{
    public class HomeController : Controller
    {
        private Context dbContext;
        public HomeController(Context context)
        {
            dbContext = context;
        }
        public ViewResult Index()
        {
            Login login = new Login();
            ViewBag.login = login;
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.User.Any(a => a.email == user.email))
                {
                    ModelState.AddModelError("email", "Email already exists!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.password = Hasher.HashPassword(user, user.password);
                    dbContext.Add(user);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserID", user.UserId);
                    HttpContext.Session.SetString("Name", user.name);
                    return RedirectToAction ("Bright_Ideas");
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login_User(Login user)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.User.FirstOrDefault(u => u.email == user.Login_Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("email", "Invalid Email");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserID", userInDb.UserId);
                HttpContext.Session.SetString("Name", userInDb.name);
                var hasher = new PasswordHasher<Login>();
                var result = hasher.VerifyHashedPassword(user, userInDb.password, user.Login_Password);
                
                if(result == 0)
                {
                    ModelState.AddModelError("password", "Invalid Password");
                    return View("Index");
                }
                 
                else
                {
                    return RedirectToAction ("Bright_Ideas");
                }
            }
            else
            {
                return View("Index");
            }
        }
        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("bright_ideas")]
        public IActionResult Bright_Ideas()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            Post post = new Post();
            ViewBag.post = post;
            ViewBag.UserId = HttpContext.Session.GetInt32("UserID");
            ViewBag.Name = HttpContext.Session.GetString("Name");
            return View(InitializeDashboard());
        }

        [HttpGet("delete_post/{id}")]
        public IActionResult Delete_Post(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            Post delete_post = dbContext.Post.SingleOrDefault(p => p.PostId == id);
            dbContext.Post.Remove(delete_post);
            dbContext.SaveChanges();
            return RedirectToAction("Bright_Ideas");
        }

        [HttpPost("create_post")]
        public IActionResult Create_Post(Post post)
        {
            if(ModelState.IsValid)
            {
                dbContext.Post.Add(post);
                dbContext.SaveChanges();
                return RedirectToAction("Bright_Ideas");
            }
            ViewBag.UserId = HttpContext.Session.GetInt32("UserID");
            ViewBag.Name = HttpContext.Session.GetString("Name");
            return View ("Bright_Ideas");
        }

        [HttpGet("users/{id}")]
        public IActionResult User_Profile(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            User user = dbContext.User.Where(u => u.UserId == id).Include(p => p.Post).Include(l => l.Like).FirstOrDefault();
            return View(user);
        }
        [HttpGet("like/{id}")]
        public IActionResult Like(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            int logged_in_user = (int)HttpContext.Session.GetInt32("UserID");
            Like like = new Like
            {
                UserId = logged_in_user,
                PostId = id
            };
            dbContext.Add(like);
            dbContext.SaveChanges();
            return RedirectToAction("Bright_Ideas");
        }

        [HttpGet("post_info/{id}")]
        public IActionResult Post_Info(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            Post post = dbContext.Post.Where(p => p.PostId == id).Include(l => l.Like).Include(u => u.user).FirstOrDefault();
            List<Post> people_liked = dbContext.Post.Where(p => p.PostId == id).Include(l => l.Like).ThenInclude(u => u.user).ToList();
            ViewBag.people_liked = people_liked;
            return View(post);
        }

        public DashboardModels InitializeDashboard()
        {
            int? logged_in_user = HttpContext.Session.GetInt32("UserID");
            return new DashboardModels
            {
                allPosts = dbContext.Post.Include(u => u.user).Include(l => l.Like).OrderByDescending(l => l.Like.Count).ToList(),
                User = dbContext.User.Where(u => u.UserId ==logged_in_user).FirstOrDefault(),
            };
        }


    }
}
