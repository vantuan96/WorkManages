using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Configs;
using Kztek_Library.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Kztek_Library.Helpers
{
    public class PrintHelper
    {
        public static PrintModel Template_Excel_V1(PrintConfig.HeaderType headertype, string title, DateTime datetime, SessionModel user, string companyname, List<SelectListModel_Print_Column_Header> Data_ColumnHeader, int leftcolumn_tocol = 0, int rightcolumn_fromcol = 0, int footer_fromcol = 0)
        {
            //
            var columnCount = Data_ColumnHeader.Count;

            //One column - header
            var Data_Header = new List<SelectListModel_Print_Header>();
            Data_Header.Add(new SelectListModel_Print_Header { ItemText = "Ngày tạo: " + datetime.ToString("dd/MM/yyyy HH:mm:ss") });
            Data_Header.Add(new SelectListModel_Print_Header { ItemText = "Nguời tạo: " + user.Username });
            Data_Header.Add(new SelectListModel_Print_Header { ItemText = "Đơn vị: " + companyname });

            //One column - header position
            var Data_HeaderPosition = new List<Tuple<int, int, int, int>>();
            Data_HeaderPosition.Add(new Tuple<int, int, int, int>(1, 1, 1, columnCount));
            Data_HeaderPosition.Add(new Tuple<int, int, int, int>(2, 1, 2, columnCount));
            Data_HeaderPosition.Add(new Tuple<int, int, int, int>(3, 1, 3, columnCount));

            //Two columns - header
            var Data_Header_Left = new List<SelectListModel_Print_Header>();
            Data_Header_Left.Add(new SelectListModel_Print_Header { ItemText = "Đơn vị: " + companyname });
            Data_Header_Left.Add(new SelectListModel_Print_Header { ItemText = "Ngày: " + datetime.ToString("dd/MM/yyyy HH:mm:ss") });

            var Data_Header_Right = new List<SelectListModel_Print_Header>();
            Data_Header_Right.Add(new SelectListModel_Print_Header { ItemText = "Cộng Hoà Xã Hội Chủ Nghĩa Việt Nam" });
            Data_Header_Right.Add(new SelectListModel_Print_Header { ItemText = "Độc Lập - Tự Do - Hạnh Phúc" });
            Data_Header_Right.Add(new SelectListModel_Print_Header { ItemText = "Số: ...../....." });

            var Data_Headers = new Tuple<List<SelectListModel_Print_Header>, List<SelectListModel_Print_Header>>(Data_Header_Left, Data_Header_Right);

            //Two columns - header positions
            var Data_HeaderPosition_Left = new List<Tuple<int, int, int, int>>();
            Data_HeaderPosition_Left.Add(new Tuple<int, int, int, int>(1, 1, 1, leftcolumn_tocol));
            Data_HeaderPosition_Left.Add(new Tuple<int, int, int, int>(2, 1, 2, leftcolumn_tocol));

            var Data_HeaderPosition_Right = new List<Tuple<int, int, int, int>>();
            Data_HeaderPosition_Right.Add(new Tuple<int, int, int, int>(1, rightcolumn_fromcol, 1, columnCount));
            Data_HeaderPosition_Right.Add(new Tuple<int, int, int, int>(2, rightcolumn_fromcol, 2, columnCount));
            Data_HeaderPosition_Right.Add(new Tuple<int, int, int, int>(3, rightcolumn_fromcol, 3, columnCount));

            var Data_HeaderPositions = new Tuple<List<Tuple<int, int, int, int>>, List<Tuple<int, int, int, int>>>(Data_HeaderPosition_Left, Data_HeaderPosition_Right);

            var printConfig = new PrintModel()
            {
                Title = title,
                Header_Type = headertype,
                ColumnHeader_Data = Data_ColumnHeader,

                Header_OneCol_Data = Data_Header,
                Header_OneCol_FromRow_FromCol_ToRow_ToCol = Data_HeaderPosition,
                Header_OneCol_Align = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left,

                Header_TwoCol_Data = Data_Headers,
                Header_TwoCol_FromRow_FromCol_ToRow_ToCol = Data_HeaderPositions,
                Header_TwoCol_Align = new Tuple<ExcelHorizontalAlignment, ExcelHorizontalAlignment>(OfficeOpenXml.Style.ExcelHorizontalAlignment.Center, OfficeOpenXml.Style.ExcelHorizontalAlignment.Center),

                Footer_FromCol = footer_fromcol
            };

            return printConfig;
        }

        public static Task<bool> Excel_Write<T>(HttpContext context, List<T> data, string filename, PrintModel config)
        {
            try
            {
                byte[] dataContent = null;

                using (var package = new ExcelPackage())
                {
                    var workSheet = package.Workbook.Worksheets.Add("Sheet1");

                    //
                    var workSheetFile = package.Workbook.Worksheets[1];

                    //Header
                    Excel_HeaderRender(workSheetFile, config);

                    //Title
                    Excel_Title(workSheetFile, config);

                    //Column header
                    Excel_ColumnHeaderRender(workSheetFile, config);

                    //Data
                    Excel_DataRender<T>(workSheetFile, config, data);

                    //Footer
                    Excel_FooterRender<T>(workSheetFile, config, data);

                    dataContent = package.GetAsByteArray();
                }

                Excel_Execute(context, dataContent, filename);

                return Task.FromResult(true);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private static void Excel_HeaderRender(ExcelWorksheet workSheet, PrintModel config)
        {
            switch (config.Header_Type)
            {
                case PrintConfig.HeaderType.NoHeader:


                    break;

                case PrintConfig.HeaderType.OneColumn:

                    foreach (var item in config.Header_OneCol_FromRow_FromCol_ToRow_ToCol)
                    {
                        workSheet.Cells[item.Item1, item.Item2, item.Item3, item.Item4].Merge = true;
                    }

                    workSheet.Cells.AutoFitColumns();
                    workSheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    for (int i = 0; i < config.Header_OneCol_Data.Count; i++)
                    {
                        workSheet.Cells[i + 1, 1].Style.Font.Name = "Times New Roman";
                        workSheet.Cells[i + 1, 1].Style.Font.Size = 12;
                        workSheet.Cells[i + 1, 1].Value = config.Header_OneCol_Data[i].ItemText;
                        workSheet.Cells[i + 1, 1].Style.Font.Bold = true;
                        workSheet.Cells[i + 1, 1].Style.HorizontalAlignment = config.Header_OneCol_Align;
                    }

                    break;

                case PrintConfig.HeaderType.TwoColumns:

                    foreach (var item in config.Header_TwoCol_FromRow_FromCol_ToRow_ToCol.Item1)
                    {
                        workSheet.Cells[item.Item1, item.Item2, item.Item3, item.Item4].Merge = true;
                    }

                    foreach (var item in config.Header_TwoCol_FromRow_FromCol_ToRow_ToCol.Item2)
                    {
                        workSheet.Cells[item.Item1, item.Item2, item.Item3, item.Item4].Merge = true;
                    }

                    workSheet.Cells.AutoFitColumns();
                    workSheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    for (int i = 0; i < config.Header_TwoCol_Data.Item1.Count; i++)
                    {
                        workSheet.Cells[i + 1, 1].Style.Font.Name = "Times New Roman";
                        workSheet.Cells[i + 1, 1].Style.Font.Size = 12;
                        workSheet.Cells[i + 1, 1].Value = config.Header_TwoCol_Data.Item1[i].ItemText;
                        workSheet.Cells[i + 1, 1].Style.Font.Bold = true;
                        workSheet.Cells[i + 1, 1].Style.HorizontalAlignment = config.Header_TwoCol_Align.Item1;
                    }

                    var k = config.Header_TwoCol_FromRow_FromCol_ToRow_ToCol.Item1.Last().Item4;

                    for (int i = 0; i < config.Header_TwoCol_Data.Item2.Count; i++)
                    {
                        workSheet.Cells[i + 1, k + 1].Style.Font.Name = "Times New Roman";
                        workSheet.Cells[i + 1, k + 1].Style.Font.Size = 12;
                        workSheet.Cells[i + 1, k + 1].Value = config.Header_TwoCol_Data.Item2[i].ItemText;
                        workSheet.Cells[i + 1, k + 1].Style.Font.Bold = true;
                        workSheet.Cells[i + 1, k + 1].Style.HorizontalAlignment = config.Header_TwoCol_Align.Item2;
                    }

                    break;

                default:
                    break;
            }

        }

        private static void Excel_Title(ExcelWorksheet workSheet, PrintModel config)
        {
            var row = 0;
            var count = config.ColumnHeader_Data.Count;

            switch (config.Header_Type)
            {
                case PrintConfig.HeaderType.NoHeader:
                    row = 2;

                    break;

                case PrintConfig.HeaderType.OneColumn:
                    row = config.Header_OneCol_Data.Count + 2;

                    workSheet.Cells[row, 1, row, count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    break;

                case PrintConfig.HeaderType.TwoColumns:
                    row = Math.Max(config.Header_TwoCol_Data.Item1.Count, config.Header_TwoCol_Data.Item2.Count) + 2;

                    workSheet.Cells[row, 1, row, count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    break;

                default:
                    row = 2;
                    break;
            }

            workSheet.Cells[row, 1, row, count].Merge = true;
            workSheet.Cells[row, 1, row, count].Style.Font.Size = 16;
            workSheet.Cells[row, 1, row, count].Style.Font.Bold = true;
            workSheet.Cells[row, 1, row, count].Style.Font.Name = "Times New Roman";

            workSheet.Cells[row, 1, row, count].Value = config.Title;

            workSheet.Cells[row, 1, row, count].AutoFitColumns();
        }

        private static void Excel_ColumnHeaderRender(ExcelWorksheet workSheet, PrintModel config)
        {
            var count = 0;
            var row = 0;

            switch (config.Header_Type)
            {
                case PrintConfig.HeaderType.NoHeader:
                    row = 4;

                    break;

                case PrintConfig.HeaderType.OneColumn:
                    row = config.Header_OneCol_Data.Count + 4;

                    break;

                case PrintConfig.HeaderType.TwoColumns:
                    row = Math.Max(config.Header_TwoCol_Data.Item1.Count, config.Header_TwoCol_Data.Item2.Count) + 4;

                    break;

                default:
                    row = 4;
                    break;
            }

            //
            foreach (var item in config.ColumnHeader_Data)
            {
                count++;

                workSheet.Cells[row, count].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[row, count].Style.Font.Name = "Times New Roman";
                workSheet.Cells[row, count].Style.Font.Size = 14;
                workSheet.Cells[row, count].Value = item.ItemText;
                workSheet.Cells[row, count].Style.Font.Bold = true;
                workSheet.Cells[row, count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            workSheet.Cells.AutoFitColumns();
        }

        private static void Excel_DataRender<T>(ExcelWorksheet workSheet, PrintModel config, List<T> data)
        {
            var row = 0;
            var count = config.ColumnHeader_Data.Count;

            switch (config.Header_Type)
            {
                case PrintConfig.HeaderType.NoHeader:

                    row = 5;
                    break;

                case PrintConfig.HeaderType.OneColumn:

                    row = config.Header_OneCol_Data.Count + 5;


                    break;

                case PrintConfig.HeaderType.TwoColumns:

                    row = Math.Max(config.Header_TwoCol_Data.Item1.Count, config.Header_TwoCol_Data.Item2.Count) + 5;

                    break;

                default:

                    row = 5;

                    break;
            }

            //FromRow_FromCol_ToRow_ToCol
            workSheet.Cells[row, 1, row + data.Count - 1, count].Style.Font.Name = "Times New Roman";
            workSheet.Cells[row, 1, row + data.Count - 1, count].Style.Font.Size = 12;

            workSheet.Cells[row, 1].LoadFromCollection<T>(data, false);

            //workSheet.Cells.AutoFitColumns();
        }

        private static void Excel_FooterRender<T>(ExcelWorksheet workSheet, PrintModel config, List<T> data)
        {
            var row = 0;
            var count = config.ColumnHeader_Data.Count;

            switch (config.Header_Type)
            {
                case PrintConfig.HeaderType.NoHeader:

                    row = data.Count + 6;
                    break;

                case PrintConfig.HeaderType.OneColumn:

                    row = config.Header_OneCol_Data.Count + data.Count + 6;


                    break;

                case PrintConfig.HeaderType.TwoColumns:

                    row = Math.Max(config.Header_TwoCol_Data.Item1.Count, config.Header_TwoCol_Data.Item2.Count) + data.Count + 6;

                    break;

                default:

                    row = data.Count + 6;

                    break;
            }

            //FromRow_FromCol_ToRow_ToCol
            workSheet.Cells[row, config.Footer_FromCol, row, count].Merge = true;
            workSheet.Cells[row, config.Footer_FromCol, row, count].Style.Font.Name = "Times New Roman";
            workSheet.Cells[row, config.Footer_FromCol, row, count].Style.Font.Size = 14;
            workSheet.Cells[row, config.Footer_FromCol, row, count].Value = "Nguời lập";
            workSheet.Cells[row, config.Footer_FromCol, row, count].Style.Font.Bold = true;
            workSheet.Cells[row, config.Footer_FromCol, row, count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        private static void Excel_Execute(HttpContext context, byte[] data, string filename)
        {
            //package.Workbook.Properties.Title = "Attempts";
            context.Response.OnStarting(state =>
            {
                var httpContext = (HttpContext)state;

                httpContext.Response.Headers.Add(
                     "content-disposition",
                      string.Format("attachment; filename={0}.xlsx", filename)
                );

                httpContext.Response.ContentLength = data.Length;
                httpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpContext.Response.Body.Write(data);

                return Task.FromResult(0);
            }, context);
        }

        public static void Text_Execute(HttpContext context, byte[] data, string filename)
        {
            //package.Workbook.Properties.Title = "Attempts";
            context.Response.OnStarting(state =>
            {
                var httpContext = (HttpContext)state;

                httpContext.Response.Headers.Add(
                     "content-disposition",
                      string.Format("attachment; filename={0}.txt", filename)
                );

                httpContext.Response.ContentLength = data.Length;
                httpContext.Response.ContentType = "text/plain";
                httpContext.Response.Body.Write(data);

                return Task.FromResult(0);
            }, context);
        }
    }
}