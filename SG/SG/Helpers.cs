using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Routing;

namespace SG
{
    public static class Helpers
    {
        /// <summary>
        /// Easy way to Clear ASP NET Cache
        /// </summary>
        /// <param name="context">Pass current HttpContext</param>
        public static void SGClearAspNetCache(HttpContext context)
        {
            foreach (DictionaryEntry entry in context.Cache)
            {
                context.Cache.Remove((string)entry.Key);
            }
        }

        //public static string DataTableToGoogleDataTable(this SqlDataReader sd)
        //{
        //    System.Data.DataTable dt = new System.Data.DataTable();

        //    dt.Load(sd);
        //    var dTable = Google.DataTable.Net.Wrapper.SystemDataTableConverter.Convert(dt);

        //    return dTable.GetJson();
        //}


        /// <summary>
        /// Returns Empty string if null value or if Not a String Value it provides an easy way troubleshoot error 
        /// </summary>
        /// <param name="reader">Object Passing</param>
        /// <returns>String</returns>
        public static string SGGetString(this object reader)
        {

            if (reader != null)
            {
                if (reader.GetType() == typeof(string))
                {
                    return reader.ToString();
                }
                else
                {
                    return "Object Type is not of type String. Type of object is " + reader.GetType().ToString();
                    //return "SGGetString reports value is NOT of Type String";
                }
            }
            else
            {
                return string.Empty;
            }
        }



        /// <summary>
        /// Returns Empty string if null value or if Not a String Value it provides an easy way troubleshoot error
        /// </summary>
        /// <param name="reader">SqlDataReader Passing</param>
        /// <param name="colIndex">Int for the Column Number</param>
        /// <returns>String</returns>
        public static string SGGetString(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                try
                {
                    return reader.GetString(colIndex);
                }
                catch (Exception ex)
                {
                    return "SGGetString reports value is NOT of Type String with name of " + reader.GetName(colIndex) + " and columnm index of " + colIndex.ToString();
                }
            }
            else
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// Returns Empty string if null value or if Not a String Value it provides an easy way troubleshoot error 
        /// </summary>
        /// <param name="reader">SqlDataReader Passing</param>
        /// <param name="name">String value of the Column Name</param>
        /// <returns>String</returns>
        public static string SGGetString(this SqlDataReader reader, string name)
        {
            var col = reader.GetOrdinal(name);
            if (!reader.IsDBNull(col))
            {
                try
                {
                    return reader.GetString(col);

                }
                catch (Exception ex)
                {
                    return "SGGetString reports value is NOT of Type String with name of " + name + " and columnm index of " + col.ToString();
                }
            }
            else
            {
                return string.Empty;
            }

        }


        /// <summary>
        /// Provides a way to return the name of the objects properties and the corresponding values 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dynamicObject"></param>
        /// <returns>In the event of an error it will provide the Error along with the results</returns>
        public static NameValueCollection SGToNameValueCollection<T>(this T dynamicObject)
        {
            var nameValueCollection = new NameValueCollection();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(dynamicObject))
            {
                string value = string.Empty;
                try
                {


                    value = propertyDescriptor.GetValue(dynamicObject).SGGetString();

                    nameValueCollection.Add(propertyDescriptor.Name, value);
                }
                catch (Exception ex)
                {
                    nameValueCollection.Add("ERROR MESSAGE: ", ex.Message);
                }
            }
            return nameValueCollection;
        }



        public static DateTime SGGetDateTime(this SqlDataReader reader, string name)
        {
            var col = reader.GetOrdinal(name);
            return (DateTime)reader.GetDateTime(col);
        }

        public static DateTime? SGGetNullableDateTime(this SqlDataReader reader, string name)
        {
            var col = reader.GetOrdinal(name);
            return reader.IsDBNull(col) ?
                        (DateTime?)null :
                        (DateTime?)reader.GetDateTime(col);
        }

        public static DateTime? SGGetNullableDateTime(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetDateTime(colIndex);
            else
                return (DateTime?)null;

        }


        public static Boolean? SGGetNullableBoolean(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetBoolean(colIndex);
            else
                return (Boolean?)null;

        }




        public static string SGHyperLinkStringBuilder(this string helper, string url, string id, string name, object htmlAttributes)
        {
            // Create tag builder
            var builder = new TagBuilder("a");

            // Create valid id
            builder.GenerateId(id);
            builder.MergeAttribute("name", name);


            // Add attributes
            builder.MergeAttribute("href", url);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            // Render tag
            return builder.ToString(TagRenderMode.SelfClosing);
        }


        public static string SGLeft(this string value, int length)
        {
            return value.Substring(0 - length);
        }

        public static string SGRight(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }

        public static string SGToMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
        }

        public static string SGToShortMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
        }


        public static String SGMonthsBetween(DateTime d0, DateTime d1)
        {
            var enumValue = Enumerable.Range(0, (d1.Year - d0.Year) * 12 + (d1.Month - d0.Month + 1))
                            .Select(m => new DateTime(d0.Year, d0.Month, 1).AddMonths(m).ToString("MMMM"));

            return String.Join(",", enumValue.ToArray());
        }

        public static string SGMonthRange(string CableMonth, string CableYear)
        {


            String monthsbetweenarray;
            string yearsubmit = string.Empty;
            if (CableMonth.Contains(","))
            {
                string[] months = CableMonth.Split(',');
                if (CableYear.Contains(","))
                {
                    string tt = months[0];
                    string[] years = CableYear.Split(',');

                    monthsbetweenarray = SGMonthsBetween(DateTime.Parse(months[0].ToString() + "/01/" + years[0].ToString()), DateTime.Parse(months[1].ToString() + "/01/" + years[1].ToString()));
                }
                else
                {
                    yearsubmit = CableYear;

                    string tt = months[0];
                    string[] years = CableYear.Split(',');

                    monthsbetweenarray = SGMonthsBetween(DateTime.Parse(months[0].ToString() + "/01/" + yearsubmit), DateTime.Parse(months[1].ToString() + "/01/" + yearsubmit));
                }

            }
            else
            {

                if (CableYear.Contains(","))
                {
                    string[] years = CableYear.Split(',');

                    monthsbetweenarray = SGMonthsBetween(DateTime.Parse(CableMonth + "/01/" + years[0].ToString()), DateTime.Parse(CableMonth + "/01/" + years[1].ToString()));
                }
                else
                {
                    yearsubmit = CableYear;
                    string[] years = CableYear.Split(',');

                    monthsbetweenarray = SGMonthsBetween(DateTime.Parse(CableMonth + "/01/" + yearsubmit), DateTime.Parse(CableMonth + "/01/" + yearsubmit));
                }
            }

            CableMonth = "'" + monthsbetweenarray.Replace(",", "','") + "'";

            return CableMonth;
        }
        public static string SGYearRange(string CableMonth, string CableYear)
        {


            String monthsbetweenarray;
            string yearsubmit = string.Empty;
            if (CableMonth.Contains(","))
            {
                string[] months = CableMonth.Split(',');
                if (CableYear.Contains(","))
                {
                    string tt = months[0];
                    string[] years = CableYear.Split(',');

                    monthsbetweenarray = SGMonthsBetween(DateTime.Parse(months[0].ToString() + "/01/" + years[0].ToString()), DateTime.Parse(months[1].ToString() + "/01/" + years[1].ToString()));
                }
                else
                {
                    yearsubmit = CableYear;

                    string tt = months[0];
                    string[] years = CableYear.Split(',');

                    monthsbetweenarray = SGMonthsBetween(DateTime.Parse(months[0].ToString() + "/01/" + yearsubmit), DateTime.Parse(months[1].ToString() + "/01/" + yearsubmit));
                }

            }
            else
            {

                if (CableYear.Contains(","))
                {
                    string[] years = CableYear.Split(',');

                    monthsbetweenarray = SGMonthsBetween(DateTime.Parse(CableMonth + "/01/" + years[0].ToString()), DateTime.Parse(CableMonth + "/01/" + years[1].ToString()));
                }
                else
                {
                    yearsubmit = CableYear;
                    string[] years = CableYear.Split(',');

                    monthsbetweenarray = SGMonthsBetween(DateTime.Parse(CableMonth + "/01/" + yearsubmit), DateTime.Parse(CableMonth + "/01/" + yearsubmit));
                }
            }

            CableMonth = "'" + monthsbetweenarray.Replace(",", "','") + "'";
            // CableYear = "'" + CableYear.Replace(",", "','") + "'";
            return CableYear;
        }

    }
}
