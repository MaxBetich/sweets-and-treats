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
  public class TreatsController : Controller
  {
    private readonly SweetsAndTreatsContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public TreatsController(UserManager<ApplicationUser> userManager, SweetsAndTreatsContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    [AllowAnonymous]
    public ActionResult Index()
    {
      List<Treat> allTreats = _db.Treats
      .OrderBy(treat => treat.TreatName).ToList();                            
      return View(allTreats);
    }

    public async Task<ActionResult> MyTreats()
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

      List<Treat> userTreats = _db.Treats
      .Where(entry => entry.User.Id == currentUser.Id)
      .OrderBy(treat => treat.TreatName).ToList();                            
      return View(userTreats);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Treat treat)
    {
      if (!ModelState.IsValid)
      {
        return View(treat);
      }
      else
      {
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
        treat.User = currentUser;

        _db.Treats.Add(treat);
        _db.SaveChanges();
        return RedirectToAction("Index");
      } 
    }

    [AllowAnonymous]
    public ActionResult Details(int id)
    {
      Treat thisTreat = _db.Treats
                            .Include(treat => treat.JoinEntities)
                            .ThenInclude(join => join.Flavor)
                            .FirstOrDefault(treat => treat.TreatId == id);
      return View(thisTreat);
    }

    public async Task<ActionResult> Edit(int id)
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

      Treat thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      if (thisTreat.User == currentUser)
      {
        return View(thisTreat);
      }
      else
      {
        return RedirectToAction("Details", new {id = thisTreat.TreatId});
      }
    }

    [HttpPost]
    public ActionResult Edit(Treat treat)
    {
      _db.Treats.Update(treat);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public async Task<ActionResult> Delete(int id)
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

      Treat thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      if (thisTreat.User == currentUser)
      {
        return View(thisTreat);
      }
      else
      {
        return RedirectToAction("Details", new {id = thisTreat.TreatId});
      } 
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Treat thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      _db.Treats.Remove(thisTreat);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public async Task<ActionResult> AddFlavor(int id)
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

      List<Flavor> userFlavors = _db.Flavors
                                    .Where(entry =>entry.User.Id == currentUser.Id)
                                    .OrderBy(flavor => flavor.FlavorName)
                                    .ToList();

      Treat thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      
      ViewBag.FlavorId = new SelectList(userFlavors, "FlavorId", "FlavorName");
      
      return View(thisTreat);
    }

    [HttpPost]
    public ActionResult AddFlavor(Treat treat, int flavorId)
    {
      #nullable enable
      FlavorTreat? joinEntity = _db.FlavorTreats.FirstOrDefault(join => (join.FlavorId == flavorId && join.TreatId == treat.TreatId));
      #nullable disable

      if (joinEntity == null && flavorId != 0)
      {
        _db.FlavorTreats.Add(new FlavorTreat() { FlavorId = flavorId, TreatId = treat.TreatId });
        _db.SaveChanges();
      }

      return RedirectToAction("Details", new { id = treat.TreatId });
    } 

    [HttpPost]
    public async Task<ActionResult> DeleteJoin(int joinId)
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

      FlavorTreat joinEntry = _db.FlavorTreats.FirstOrDefault(entry => entry.FlavorTreatId == joinId);
      Treat thisTreat = _db.Treats.FirstOrDefault(entry => entry.TreatId == joinEntry.TreatId);
      if (thisTreat.User == currentUser)
      {
        _db.FlavorTreats.Remove(joinEntry);
        _db.SaveChanges();
        return RedirectToAction("Details", new {id = thisTreat.TreatId});
      }
      else
      {
        return RedirectToAction("Details", new {id = thisTreat.TreatId});
      }
    }
  }
}