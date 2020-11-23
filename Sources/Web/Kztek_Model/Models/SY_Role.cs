using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kztek_Model.Models
{
    [Table("SY_Role")]
    public class SY_Role
    {
        public string Id { get; set; }

        public string RoleName { get; set; }

        public string Description { get; set; }

        [Column("Active", TypeName = "bit")]
        [DefaultValue(true)]
        public bool Active { get; set; }
    }

    public class SY_Role_Submit
    {
        public string Id { get; set; }

        public string RoleName { get; set; }

        public string Description { get; set; }

        public List<string> MenuFunctions { get; set; } = new List<string>();

        public string MenuFunctionIds { get; set; } = "";

        public bool Active { get; set; } = true;

        public List<SY_MenuFunction> Data_Tree { get; set; }
    }

    public class SY_Role_Selected
    {
        public List<string> Selected { get; set; } = new List<string>();

        public List<SY_MenuFunction> Data_Tree { get; set; } = new List<SY_MenuFunction>();

        public List<SY_MenuFunction> Data_Child { get; set; } = new List<SY_MenuFunction>();
    }
}