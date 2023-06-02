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

    [HttpPost]
    public ActionResult Edit(Flavor flavor)
    {
      _db.Flavors.Update(flavor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public async Task<ActionResult> Delete(int id)
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

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Flavor thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == id);
      _db.Flavors.Remove(thisFlavor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public async Task<ActionResult> AddTreat(int id)
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

      List<Treat> userTreats = _db.Treats
                                .Where(entry =>entry.User.Id == currentUser.Id)
                                .OrderBy(treat => treat.TreatName)
                                .ToList();

      Flavor thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == id);
      
      ViewBag.TreatId = new SelectList(userTreats, "TreatId", "TreatName");
      
      return View(thisFlavor);
    }

    [HttpPost]
    public ActionResult AddTreat(Flavor flavor, int treatId)
    {
      #nullable enable
      FlavorTreat? joinEntity = _db.FlavorTreats.FirstOrDefault(join => (join.TreatId == treatId && join.FlavorId == flavor.FlavorId));
      #nullable disable

      if (joinEntity == null && treatId != 0)
      {
        _db.FlavorTreats.Add(new FlavorTreat() { TreatId = treatId, FlavorId = flavor.FlavorId });
        _db.SaveChanges();
      }

      return RedirectToAction("Details", new { id = flavor.FlavorId });
    } 

    [HttpPost]
    public async Task<ActionResult> DeleteJoin(int joinId)
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

      FlavorTreat joinEntry = _db.FlavorTreats.FirstOrDefault(entry => entry.FlavorTreatId == joinId);
      Flavor thisFlavor = _db.Flavors.FirstOrDefault(entry => entry.FlavorId == joinEntry.FlavorId);
      if (thisFlavor.User == currentUser)
      {
        _db.FlavorTreats.Remove(joinEntry);
        _db.SaveChanges();
        return RedirectToAction("Details", new {id = thisFlavor.FlavorId});
      }
      else
      {
        return RedirectToAction("Details", new {id = thisFlavor.FlavorId});
      }
    }
  }
}