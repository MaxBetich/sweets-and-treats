using Microsoft.AspNetCore.Mvc;
using SweetsAndTreats.Models;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SweetsAndTreats.Controllers
{
    public class HomeController : Controller
    {

      private readonly SweetsAndTreatsContext _db;
      private readonly UserManager<ApplicationUser> _userManager;

      public HomeController(UserManager<ApplicationUser> userManager, SweetsAndTreatsContext db)
      {
        _userManager = userManager;
        _db = db;
      }

      [HttpGet("/")]
      public ActionResult Index()
      {
        Flavor[] flav = _db.Flavors.OrderByDescending(flav => flav.FlavorName).ToArray();
        Treat[] treat = _db.Treats.OrderBy(treat  => treat.TreatName).ToArray();
        Dictionary<string,object[]> model = new Dictionary<string, object[]>();
        model.Add("flavors", flav);
        model.Add("treats", treat);         
        return View(model);
      }      

    }
}