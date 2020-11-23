using System.Collections.Generic;
using Kztek_Model.Models.WM;

namespace Kztek_Library.Models
{
    public class WM_DiaryCustomViewModel
    {
        public string Username { get; set; }

        public List<WM_Diary> DataByUser { get; set; }  
    }
}