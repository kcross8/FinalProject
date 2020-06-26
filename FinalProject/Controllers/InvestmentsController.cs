using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FinalProject.Controllers
{
    public class InvestmentsController : Controller
    {
        private readonly SeamlessDAL sd;
        private readonly InvestmentsDbContext _context;
        public InvestmentsController(IConfiguration configuration, InvestmentsDbContext context)
        {
            sd = new SeamlessDAL(configuration);
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult InvestmentsIndex()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var thisUsersPreferences = _context.UserPreferences.Where(x => x.UserId == id).ToList();
            return View(thisUsersPreferences);

        }
        public IActionResult Search(string companyName, string country, string city, string theme, string technologyAreas, string alignment, int? rating)
        {
            IEnumerable<Record> s = sd.getStart().records;
            Startups.RateStartups(s);
            IEnumerable<Record> found = s.Where(x =>
            (string.IsNullOrWhiteSpace(companyName) || x.startups.CompanyName.ToLower() == companyName.ToLower())
            && (string.IsNullOrWhiteSpace(country) || x.startups.Country.ToLower() == country.ToLower())
            && (string.IsNullOrWhiteSpace(city) || x.startups.City == city)
            && (string.IsNullOrWhiteSpace(theme) || x.startups.Themes != null && x.startups.Themes.Contains(theme))
            && (string.IsNullOrWhiteSpace(technologyAreas) || x.startups.TechnologyAreas != null && x.startups.TechnologyAreas.Contains(technologyAreas))
            && (string.IsNullOrWhiteSpace(alignment) || x.startups.Alignment != null && x.startups.Alignment.Contains(alignment))
            && (rating == null || x.startups.Rating >= rating));
            foreach (Record r in found)
            {
                r.startups.Comments = _context.Comments.Where(x => x.CompanyName == r.startups.CompanyName && x.FavoriteId == null).ToList();
            }
            return View(found);
        }
        [Authorize]
        public IActionResult UserSurvey()
        {
            //UserPreferences thisUsersPreferences = _context.UserPreferences.FirstOrDefault();
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var thisUsersPreferences = _context.UserPreferences.Where(x => x.UserId == id).ToList();
            return View(thisUsersPreferences);
        }
        [Authorize]
        public IActionResult AddUserPreferences(string country, string city, string theme, string technologyAreas, string alignment, int? rating)
        {
            UserPreferences userPreferences = new UserPreferences();
            if (ModelState.IsValid)
            {
                userPreferences.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                userPreferences.Country = country;
                userPreferences.City = city;
                userPreferences.Theme = theme;
                userPreferences.TechArea = technologyAreas;
                userPreferences.Alignment = alignment;
                userPreferences.Rank = rating;
                _context.UserPreferences.Add(userPreferences);
                _context.SaveChanges();
                return RedirectToAction("AutoSearch", userPreferences);
            }
            else
            {
                return RedirectToAction("InvestmentsIndex");
            }
        }
        [Authorize]
        public IActionResult AutoSearch(UserPreferences userPreferences)
        {
            string country = userPreferences.Country;
            string city = userPreferences.City;
            string theme = userPreferences.Theme;
            string technologyAreas = userPreferences.TechArea;
            string alignment = userPreferences.Alignment;
            int? rating = userPreferences.Rank;
            IEnumerable<Record> s = sd.getStart().records;
            Startups.RateStartups(s);
            IEnumerable<Record> found = s.Where(x =>
            (string.IsNullOrWhiteSpace(country) || x.startups.Country.ToLower() == country.ToLower())
            && (string.IsNullOrWhiteSpace(city) || x.startups.City == city)
            && (string.IsNullOrWhiteSpace(theme) || x.startups.Themes != null && x.startups.Themes.Contains(theme))
            && (string.IsNullOrWhiteSpace(technologyAreas) || x.startups.TechnologyAreas != null && x.startups.TechnologyAreas.Contains(technologyAreas))
            && (string.IsNullOrWhiteSpace(alignment) || x.startups.Alignment != null && x.startups.Alignment.Contains(alignment))
            && (rating == null || x.startups.Rating >= rating));
            foreach (Record r in found)
            {
                r.startups.Comments = _context.Comments.Where(x => x.CompanyName == r.startups.CompanyName && x.FavoriteId == null).ToList();
            }
            return View("Search", found);
        }
        [Authorize]
        public IActionResult RemoveUserPreferences(int id)
        {
            UserPreferences found = _context.UserPreferences.Find(id);
            if (found != null)
            {
                _context.UserPreferences.Remove(found);
                _context.SaveChanges();
            }
            return RedirectToAction("UserSurvey");
        }
        [Authorize]
        public IActionResult UpdateUserPreferences(int id)
        {
            UserPreferences found = _context.UserPreferences.Find(id);
            return View(found);
        }
        [Authorize]
        public IActionResult ChangeUserPreferences(int id, string country, string city, string theme, string technologyAreas, string alignment, int? rating)
        {
            UserPreferences found = _context.UserPreferences.Find(id);
            if (found != null)
            {
                found.Country = country;
                found.City = city;
                found.Theme = theme;
                found.TechArea = technologyAreas;
                found.Alignment = alignment;
                found.Rank = rating;
                _context.Entry(found).State = EntityState.Modified;
                _context.Update(found);
                _context.SaveChanges();
            }
            return RedirectToAction("InvestmentsIndex", found);
        }
        [Authorize]
        public IActionResult AddToFavorite(string name, int rating, string id)
        {
            Favorite favorite = new Favorite
            {
                Rank = rating,
                StartupName = name,
                StartupId = id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            };
            if (_context.Favorite.Where(x => (x.StartupName == name) && (x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)).ToList().Count > 0)
            {
                return RedirectToAction("Favorites");
            }
            if (ModelState.IsValid)
            {
                _context.Favorite.Add(favorite);
                _context.SaveChanges();
            }
            return RedirectToAction("Favorites");
        }
        [Authorize]
        public IActionResult Favorites()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var thisUsersFavorites = _context.Favorite.Where(x => x.UserId == id).ToList();

            foreach (Favorite f in thisUsersFavorites)
            {
                f.Comments = _context.Comments.Where(x => x.FavoriteId == f.Id).ToList();
            }

            return View(thisUsersFavorites);
        }
        [Authorize]
        public IActionResult RemoveFavorite(int id)
        {
            Favorite found = _context.Favorite.Find(id);

            List<Comments> toDelete = _context.Comments.Where(x => x.FavoriteId == id).ToList();
            foreach (Comments c in toDelete)
            {
                _context.Comments.Remove(c);
            }

            if (found != null)
            {
                _context.Favorite.Remove(found);
                _context.SaveChanges();
            }
            return RedirectToAction("Favorites", new { id = found.Id });
        }

        [Authorize]
        public IActionResult ConfirmFavRemove(int id)
        {
            Favorite toRemove = _context.Favorite.Find(id);
            return View(toRemove);
        }

        public IActionResult Individual(string id)
        {
            Record r = sd.GetRecord(id);
            Startups.RateIndividual(r);
            
                r.startups.Comments = _context.Comments.Where(x => x.CompanyName == r.startups.CompanyName && x.FavoriteId == null).ToList();
            
            return View(r);
        }
        [Authorize]
        public IActionResult AddComment(int id, string comment)
        {
            Favorite found = _context.Favorite.Find(id);
            if (found != null)
            {
                found.PrivateComments = comment;
                _context.Entry(found).State = EntityState.Modified;
                _context.Update(found);
                _context.SaveChanges();

                _context.Comments.Add(new Comments
                {
                    Comment = comment,
                    FavoriteId = id
                });

                _context.SaveChanges();
            }
            return RedirectToAction("Favorites");
        }
        [Authorize]
        public IActionResult RemoveComment(int id)
        {
            Comments found = _context.Comments.Find(id);
            if (found != null)
            {
                _context.Comments.Remove(found);
                _context.SaveChanges();

            }
            return RedirectToAction("Favorites");
        }
        [Authorize]
        public IActionResult EditComment(int id, string comment)
        {
            Favorite found = _context.Favorite.Find(id);
            if (found != null)
            {
                found.PrivateComments = comment;
                _context.Entry(found).State = EntityState.Modified;
                _context.Update(found);
                _context.SaveChanges();

            }
            return RedirectToAction("Favorites");
        }
        public IActionResult ConfirmCommentRemove(int id)
        {
            Comments toRemove = _context.Comments.Find(id);
            return View(toRemove);
        }
       
        public IActionResult AddPublicComment(string companyName)
        {
           
            return View((object) companyName);
        }
        
        public IActionResult AddPublicComments(string companyName, string comment)
        {
            _context.Comments.Add(new Comments
            {
                Comment = comment,
                CompanyName = companyName
            });

            _context.SaveChanges();
            return RedirectToAction("Search");
        }

        public IActionResult About()
        {
            return View();
        }
    }
}