using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kztek_Model.Models
{
    [Table("SY_MenuFunction")]
    public class SY_MenuFunction
    {
        [Key]
        public string Id { get; set; }

        [Required (ErrorMessage = "Nhập tên menu")]
        public string MenuName { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string MenuType { get; set; }

        public string Icon { get; set; }

        [Column("Active", TypeName = "bit")]
        [DefaultValue(true)]
        public bool Active { get; set; }

        public int SortOrder { get; set; }

        public DateTime DateCreated { get; set; }

        public string ParentId { get; set; }

        public int? Dept { get; set; }

        public string Breadcrumb { get; set; }
    }

    public class SY_MenuFunction_Submit
    {
        [Key]
        public string Id { get; set; }

        [Required (ErrorMessage = "Nhập tên menu")]
        public string MenuName { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string MenuType { get; set; }

        public string Icon { get; set; }

        [Column("Active", TypeName = "bit")]
        [DefaultValue(true)]
        public bool Active { get; set; }

        public int SortOrder { get; set; }

        public string ParentId { get; set; }

    }

    public class SY_MenuFunction_Tree 
    {
        public List<SY_MenuFunction> Data_All { get; set; }
        
        public List<SY_MenuFunction> Data_Child { get; set; }
    }
}