using System.Collections.Generic;
using System.Data;
using System.Linq;
using Kztek_Core.Models;

namespace Kztek_Library.Helpers
{
    public class DatabaseHelper
    {
        public static bool ExcuteCommandToBool(string command, string db = "DefaultConnection") {
            //Chuyển db
            var connect = AppSettingHelper.GetStringFromFileJson("connectstring", "ConnectionStrings:" + db).Result;
            var connecttype = AppSettingHelper.GetStringFromFileJson("connectstring", "ConnectionStrings:DefaultType").Result;

            switch (connecttype)
            {

                case DatabaseModel.SQLSERVER:

                  return SqlHelper.ExcuteCommandToBool(connect, command);

                case DatabaseModel.MYSQL:

                   return MySqlHelper.ExcuteCommandToBool(connect, command);

                default:

                   return false;
            }
        }

        public static T ExcuteCommandToModel<T>(string command, string db = "DefaultConnection") {
            var list = new List<T>();

            //Chuyển db
            var connect = AppSettingHelper.GetStringFromFileJson("connectstring", "ConnectionStrings:" + db).Result;
            var connecttype = AppSettingHelper.GetStringFromFileJson("connectstring", "ConnectionStrings:DefaultType").Result;

            switch (connecttype)
            {

                case DatabaseModel.SQLSERVER:

                  return SqlHelper.ExcuteCommandToModel<T>(connect, command);

                case DatabaseModel.MYSQL:

                   return MySqlHelper.ExcuteCommandToModel<T>(connect, command);

                default:

                   return list.FirstOrDefault();
            }
        }

        public static List<T> ExcuteCommandToList<T>(string command, string db = "DefaultConnection")
        {
            List<T> list = new List<T>();

            //Chuyển db
            var connect = AppSettingHelper.GetStringFromFileJson("connectstring", "ConnectionStrings:" + db).Result;
            var connecttype = AppSettingHelper.GetStringFromFileJson("connectstring", "ConnectionStrings:DefaultType").Result;

            switch (connecttype)
            {

                case DatabaseModel.SQLSERVER:

                  return SqlHelper.ExcuteCommandToList<T>(connect, command);

                case DatabaseModel.MYSQL:

                   return MySqlHelper.ExcuteCommandToList<T>(connect, command);

                default:

                   return list;
            }
        }

        public static DataSet ExcuteCommandToDataSet(string command, string db = "DefaultConnection")
        {         
            //Chuyển db
            var connect = AppSettingHelper.GetStringFromFileJson("connectstring", "ConnectionStrings:" + db).Result;
            var connecttype = AppSettingHelper.GetStringFromFileJson("connectstring", "ConnectionStrings:DefaultType").Result;

            switch (connecttype)
            {

                case DatabaseModel.SQLSERVER:
                    return SqlHelper.GetDataSet(connect, command);   
                default:

                    return null;
            }
        }      
    }
}