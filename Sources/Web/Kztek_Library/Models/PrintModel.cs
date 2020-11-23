using System;
using System.Collections.Generic;
using Kztek_Library.Configs;
using OfficeOpenXml.Style;

namespace Kztek_Library.Models
{
    public class PrintModel
    {
        public PrintConfig.HeaderType Header_Type { get; set; }

        //Title
        public string Title { get; set; }

        //Column header
        public List<SelectListModel_Print_Column_Header> ColumnHeader_Data { get; set; }

        //Loại 1 cột
        public List<SelectListModel_Print_Header> Header_OneCol_Data { get; set; }

        public List<Tuple<int, int, int, int>> Header_OneCol_FromRow_FromCol_ToRow_ToCol { get; set; }

        public ExcelHorizontalAlignment Header_OneCol_Align { get; set; }

        //Loại 2 cột
        public Tuple<List<SelectListModel_Print_Header>, List<SelectListModel_Print_Header>> Header_TwoCol_Data { get; set; }

        public Tuple<List<Tuple<int, int, int, int>>, List<Tuple<int, int, int, int>>> Header_TwoCol_FromRow_FromCol_ToRow_ToCol { get; set; }

        public Tuple<ExcelHorizontalAlignment, ExcelHorizontalAlignment> Header_TwoCol_Align { get; set; }

        //Footer
        public int Footer_FromCol { get; set; }
    }
}