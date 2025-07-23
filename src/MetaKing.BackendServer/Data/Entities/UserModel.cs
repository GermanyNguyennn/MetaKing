using MetaKing.BackendServer.Data.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace MetaKing.BackendServer.Data
{
    public class UserModel : IdentityUser, IDateTracking
    {
        public string FullName { get; set; }
        public string Dob {  get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public UserModel() { }  

        public UserModel(string id, string userName, string email, string phoneNumber, string fullName, string dob, string address, string city, string district, string ward, DateTime createdDate)
        {
            Id = id;
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            FullName = fullName;
            Dob = dob;
            Address = address;
            City = city;
            District = district;
            Ward = ward;
            CreatedDate = createdDate;
        }
    }
}
