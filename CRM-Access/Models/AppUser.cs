using CRM_Access.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace CRM_Access.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CompanyDomain { get; internal set; }
        public string? CompanyName { get; internal set; }
    }
}