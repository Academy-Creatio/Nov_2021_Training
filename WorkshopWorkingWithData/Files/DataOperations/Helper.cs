using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using Terrasoft.Common;
using Terrasoft.Core.Entities;

namespace WorkshopWorkingWithData.Files.DataOperations
{
    public static class Helper
    {
        public static string GetHtmlPage(this DataTable dt, long timer = 0, string queryParam = "", string title = "")
        {
            const string style = "{border: 1px solid black;}";
            const string meta1 = "<meta charset=\"utf - 8\">";
            const string meta2 = "<meta name=\"viewport\" content=\"width = device - width, initial - scale = 1\">";
            const string link = "<link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css\">";
            const string lang = "\"en\"";

            const string script1 = "<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js\"></script>";
            const string script2 = "<script src=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js\"></script>";

            StringBuilder sb = new StringBuilder();
            sb.Append($@"
                <!DOCTYPE html>
                <html lang={lang}>
                <head>
                {meta1}{meta2}{link}{script1}{script2}
                <style>
                table, th, td {style}
                </style>
                </head>
                <body>
                {GetRQueryHtmlComponent(queryParam, title)}
                <br/>
                {dt.GetHtmlComponent(timer)}
                </body>
                </html>
            ");
            return sb.ToString();
        }
        public static DataTable ConvertToDataTable(this EntityCollection entities)
        {
            var columnNames = entities[0].GetColumnValueNames();
            
            IEnumerator<string> i = columnNames.GetEnumerator();
            DataTable dt = new DataTable(entities[0].SchemaName);

            while (i.MoveNext())
            {
                object columnValue = entities[0].GetColumnValue(i.Current);

                if (columnNames != null)
                {
                    DataColumn dc = new DataColumn(i.Current, columnValue.GetType());
                    dt.Columns.Add(dc);
                }
            }
            foreach (var entity in entities)
            {
                DataRow row = dt.NewRow();
                foreach (DataColumn dc in dt.Columns)
                {

                    object columnValue = entity.GetColumnValue(dc.ColumnName);

                    if (columnValue != null)
                    {
                        try
                        {
                            row[dc.ColumnName] = columnValue;
                        }
                        catch(Exception e) 
                        {
                            //Sorry
                        };
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
        private static string GetRQueryHtmlComponent(string query, string title)
        {
            if (string.IsNullOrEmpty(query)) return "";

            const string blockquote = "<blockquote class=\"blockquote\">";
            StringBuilder sb = new StringBuilder();
            sb.Append($" <h1 class=\"display-1\">Request Query ({title})</h1>");
            sb.Append(blockquote);
            sb.Append("<p class=\"mb-0\">");
            sb.Append(query.Replace("\n","<br/>"));
            sb.Append("</p>");
            sb.Append("</blockquote>");
            return sb.ToString();
        }
        public static string GetHtmlComponent(this DataTable dt, long timer = 0)
        {
            StringBuilder sb = new StringBuilder();
            if (timer != 0)
            {
                sb.Append($" <h1 class=\"display-1\">Execution Time in ticks: {timer}</h1>");
            }
            sb.Append($"<h1 class=\"display-3\">Table Name: {dt.TableName}</h1>");
            sb.Append("<table class=\"table\">");
            
            //Table Header
            sb.Append("<thead><tr>");
            foreach(DataColumn dc in dt.Columns)
            {
                sb.Append($"<th>{dc.ColumnName}</th>");
            }
            sb.Append("</tr></thead>");
            sb.Append("<tbody>");
            foreach(DataRow row in dt.Rows)
            {
                sb.Append("<tr>");

                object[] values = row.ItemArray;
                for(int i =0; i<values.Length; i++)
                {
                    sb.Append($"<td>{row[i]}</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</tbody>");
            sb.Append("</table>");
            return sb.ToString();
        }
    }
    public enum QuryType
    {
        SELECT = 0,
        ESQ = 1,
        CustomQuery = 2,
        UPDATE = 3,
        INSERT = 4,
        DELETE = 5

    }
}
