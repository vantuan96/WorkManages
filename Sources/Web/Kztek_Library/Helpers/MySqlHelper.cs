using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Kztek_Library.Helpers
{
    public class MySqlHelper
    {
        public static bool ExcuteCommandToBool(string connect, string command)
        {
            var result = false;

            var k = new MySqlConnection(connect);

            using (MySqlConnection conn = k)
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(command, conn);

                var dt = cmd.ExecuteReader();

                result = dt.HasRows;
            }

            return result;
        }

        public static List<T> ExcuteCommandToList<T>(string connect, string command)
        {
            List<T> list = new List<T>();

            var k = new MySqlConnection(connect);

            using (MySqlConnection conn = k)
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(command, conn);

                var dt = cmd.ExecuteReader();

                list = DataReaderMapToList<T>(dt);
            }

            return list;
        }

        public static T ExcuteCommandToModel<T>(string connect, string command)
        {
            var list = new List<T>();

            var k = new MySqlConnection(connect);

            using (MySqlConnection conn = k)
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(command, conn);

                var dt = cmd.ExecuteReader();

                list = DataReaderMapToList<T>(dt);
            }

            return list.FirstOrDefault();
        }

        private static List<T> DataReaderMapToList<T>(MySqlDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (prop.PropertyType.FullName == "System.Boolean")
                    {
                        prop.SetValue(obj, Convert.ToBoolean(dr[prop.Name]), null);
                    }
                    else
                    {
                        if (!object.Equals(dr[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(obj, dr[prop.Name], null);
                        }
                    }
                }
                list.Add(obj);
            }
            return list;
        }
    }
}