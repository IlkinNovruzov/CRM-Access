namespace CRM_Access.DTO
{
    public class RegisterDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ImageURL { get; set; }
        public string Phone { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDomain { get; set; }
    }
        //public string ConfirmPassword { get; set; }
}
