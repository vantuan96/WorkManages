using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kztek_Model.Models
{
    [Table("SY_Map_Role_Menu")]
    public class SY_Map_Role_Menu
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string RoleId { get; set; }

        [Required]
        public string MenuId { get; set; }
    }
}