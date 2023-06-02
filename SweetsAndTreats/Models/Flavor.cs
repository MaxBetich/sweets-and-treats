using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SweetsAndTreats.Models
{
  public class Flavor
  {
    public int FlavorId {get;set;}
    [Required(ErrorMessage = "Must enter a name")]
    public string FlavorName {get;set;}
    public List<FlavorTreat> JoinEntities {get;set;}
    public ApplicationUser User {get;set;}
  }
}