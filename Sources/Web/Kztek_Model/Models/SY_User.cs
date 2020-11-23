using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kztek_Model.Models
{
    [Table("SY_User")]
    public class SY_User
    {
        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage = "Nhập tên người dùng")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Nhập tên tài khoản")]
        public string Username { get; set; }

        public string Password { get; set; }

        public string PasswordSalat { get; set; }

        [Column("isAdmin", TypeName = "bit")]
        [DefaultValue(false)]
        public bool isAdmin { get; set; }

        [Column("Active", TypeName = "bit")]
        [DefaultValue(true)]
        public bool Active { get; set; }

        public string Avatar { get; set; }

        public string Phone { get; set; }

        public string CodeReset { get; set; } = "";
    }

    public class SY_User_Submit
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string OldPassword { get; set; }

        public string Password { get; set; }

        public string RePassword { get; set; }

        public bool isAdmin { get; set; }

        public bool Active { get; set; } = true;

        public List<string> Roles { get; set; } = new List<string>();

        public string RoleIds { get; set; }

        public List<SY_Role> Data_Role { get; set; } = new List<SY_Role>();

        public string Avatar { get; set; } = "";

        public string Phone { get; set; } = "";
    }

    public class SY_User_Selected
    {
        public List<string> Selected { get; set; } = new List<string>();

        public List<SY_Role> Data_Role { get; set; } = new List<SY_Role>();

    }
}