using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SweetsAndTreats.Models
{
  public class Treat
  {
    public int TreatId {get;set;}
    [Required(ErrorMessage = "Must enter a name")]
    public string TreatName {get;set;}
    public List<FlavorTreat> JoinEntities {get;set;}
    public ApplicationUser User {get;set;}
  }
}