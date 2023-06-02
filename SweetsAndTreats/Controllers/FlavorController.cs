using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SweetsAndTreats.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SweetsAndTreats.Controllers
{
  [Authorize]
  public class FlavorController : Controller
  {
    private readonly SweetsAndTreatsContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public FlavorController(UserManager<ApplicationUser> userManager, SweetsAndTreatsContext db)
    {
      _userManager = userManager;

      _db = db;
    }

    [AllowAnonymous]
    public ActionResult Index()
    {
      List<Flavor> allFlavors = _db.Flavors
      .OrderBy(flavor => flavor.FlavorName).ToList();                            
      return View(allFlavors);
    }

    public async Task<ActionResult> MyFlavors()
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

      List<Flavor> userFlavors = _db.Flavors
      .Where(entry => entry.User.Id == currentUser.Id)
      .OrderBy(flavor => flavor.FlavorName).ToList();                            
      return View(userFlavors);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Flavor flavor)
    {
      if (!ModelState.IsValid)
      {
        return View(flavor);
      }
      else
      {
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
        flavor.User = currentUser;

        _db.Flavors.Add(flavor);
        _db.SaveChanges();
        return RedirectToAction("Index");
      } 
    }

    [AllowAnonymous]
    public ActionResult Details(int id)
    {
      Flavor thisFlavor = _db.Flavors
                             .Include(flavor => flavor.JoinEntities)
                             .ThenInclude(join => join.Treat)
                             .FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }

    public async Task<ActionResult> Edit(int id)
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

      Flavor thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == id);
      if (thisFlavor.User == currentUser)
      {
        return View(thisFlavor);
      }
      else
      {
        ModelState.AddModelError("Only the user who created a flavor may remove it","");
        return RedirectToAction("Details", new {id = thisFlavor.FlavorId});
      }
    }
  }
}