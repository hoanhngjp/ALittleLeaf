using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.DTOs.Admin
{
    /// <summary>User record returned to the admin panel.</summary>
    public class AdminUserDto
    {
        public long     UserId       { get; set; }
        public string   UserEmail    { get; set; } = string.Empty;
        public string   UserFullname { get; set; } = string.Empty;
        public bool     UserSex      { get; set; }
        public DateOnly UserBirthday { get; set; }
        public bool     UserIsActive { get; set; }
        public string   UserRole     { get; set; } = string.Empty;
        public DateTime CreatedAt    { get; set; }
        public DateTime UpdatedAt    { get; set; }
    }

    /// <summary>Payload for updating a user from the admin panel.</summary>
    public class AdminUpdateUserDto
    {
        [MaxLength(255)]
        public string? UserFullname { get; set; }

        public bool? UserSex      { get; set; }
        public DateOnly? UserBirthday { get; set; }
        public bool? UserIsActive { get; set; }

        /// <summary>Allowed values: "admin", "customer".</summary>
        public string? UserRole { get; set; }
    }

    /// <summary>Payload for creating a new user from the admin panel.</summary>
    public class AdminCreateUserDto
    {
        [Required, EmailAddress, MaxLength(255)]
        public string UserEmail { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string UserFullname { get; set; } = string.Empty;

        public bool     UserSex      { get; set; }
        public DateOnly? UserBirthday { get; set; }
        public bool     UserIsActive { get; set; } = true;

        /// <summary>Allowed values: "admin", "customer". Defaults to "customer".</summary>
        public string UserRole { get; set; } = "customer";
    }
}
