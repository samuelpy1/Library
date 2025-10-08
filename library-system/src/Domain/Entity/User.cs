using library_system.Domain.ValueObjects;

namespace library_system.Domain.Entity
{
    public class Member
    {
        public Guid MemberId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Email Email { get; private set; } = null!;
        public Password Password { get; private set; } = null!;
        public string Phone { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }

        public Member()
        {
            IsActive = true;
            RegistrationDate = DateTime.UtcNow;
        }

        public Member(Guid id, string name, Email email, Password password, string phone)
        {
            MemberId = id;
            Name = name;
            Email = email;
            Password = password;
            Phone = phone;
            RegistrationDate = DateTime.UtcNow;
            IsActive = true;
        }

        public void UpdateEmail(Email newEmail)
        {
            Email = newEmail;
        }

        public void UpdatePassword(Password newPassword)
        {
            Password = newPassword;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }
}


