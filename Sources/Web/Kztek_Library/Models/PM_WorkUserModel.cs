using System;
using System.Collections.Generic;
using Kztek_Model.Models;
using Kztek_Model.Models.PM;

namespace Kztek_Library.Models
{
    public class PM_WorkUserModel
    {
        public List<PM_Work> Data_Work { get; set; }

        public List<SY_User> Data_User { get; set; }

    }
}
