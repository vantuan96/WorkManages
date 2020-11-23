using System;
using System.Collections.Generic;
using Kztek_Core.Models;

namespace Kztek_Library.Helpers
{
    public class GridModelHelper<T> where T : class
    {
        public static GridModel<T> GetPage(List<T> list, int currentPage, int itemPerPage, int TotalItemCount)
        {
            var PageCount = TotalItemCount > 0
                        ? (int)Math.Ceiling(TotalItemCount / (double)itemPerPage)
                        : 0;

            var PageModel = new GridModel<T>
            {
                Data = list,
                PageIndex = currentPage,
                PageSize = itemPerPage,
                TotalPage = PageCount,
                TotalIem = TotalItemCount,
            };

            return PageModel;
        }

        public static GridModel<T> GetPage(List<T> list, int currentPage, int itemPerPage, int TotalItemCount, int PageCount)
        {

            var PageModel = new GridModel<T>
            {
                Data = list,
                PageIndex = currentPage,
                PageSize = itemPerPage,
                TotalPage = PageCount,
                TotalIem = TotalItemCount,
            };

            return PageModel;
        }
    }
}