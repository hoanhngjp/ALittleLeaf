namespace ALittleLeaf.ViewModels
{
    public class EditUserViewModel
    {
        public long UserId { get; set; }

        public string UserEmail { get; set; } = null!;

        public string? UserNewPassword { get; set; } = null!;

        public string UserFullname { get; set; } = null!;

        public bool UserSex { get; set; }
        public DateOnly UserBirthday { get; set; }
        public bool UserIsActive { get; set; }
        public string UserRole { get; set; } = null!;
    }
}
