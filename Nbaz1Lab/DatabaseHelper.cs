using Npgsql;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nbaz1Lab
{
    public static class DatabaseHelper
    {
        private static string connString = string.Format("Server=192.168.56.12;Port=5432;" +
                       "User Id=postgres;Password=reverse;Database=postgres;");
        private static NpgsqlConnection conn;

        public static bool Insert(string tableName, params string[] arguments)
        {
            conn = new NpgsqlConnection(connString);
            conn.Open();

            StringBuilder sb = new StringBuilder("INSERT INTO \"" + tableName + "\" (body, summary, keywords, title) VALUES (");
            for (int i = 0; i < arguments.Length; i++)
            {
                sb.Append("'").Append(arguments[i]).Append("',");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");

            NpgsqlCommand cmd = new NpgsqlCommand(sb.ToString(), conn);
            int affectedRows = cmd.ExecuteNonQuery();

            conn.Close();

            return affectedRows == 0 ? false : true;
        }

        public static Tuple<List<string>, List<float>> QueryMorphologic(string query)
        {
            List<string> listOfDocTitlesBolded = new List<string>();
            List<float> listOfDocRanks = new List<float>();
            conn = new NpgsqlConnection(connString);

            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = query;

                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        listOfDocTitlesBolded.Add(reader.GetString(1));
                        listOfDocRanks.Add(reader.GetFloat(2));
                    }
                conn.Close();
            }
            return new Tuple<List<string>, List<float>>(listOfDocTitlesBolded, listOfDocRanks);
        }


        public static string MorphologySearchQueryBuilder(string boolOperator, string queryText)
        {
            StringBuilder sb = new StringBuilder("SELECT \"title\", ts_headline(\"title\", to_tsquery('english', ' ");
            string searchTextClean = SplitText(queryText, boolOperator);
            List<string> listofPhrases = ProccessSearchTextClean(searchTextClean);

            sb.Append(searchTextClean + "')), ts_rank(\"tsv\", to_tsquery('english', '" + searchTextClean + " ')) rank  FROM \"Document\" WHERE ");

            for (int i = 0; i < listofPhrases.Count; i++)
            {
                sb.Append("tsv @@ to_tsquery('english', '" + listofPhrases.ElementAt(i) + "\')");
                if (i != listofPhrases.Count - 1) sb.Append((boolOperator == "|") ? " OR " : " AND ");
            }
            sb.Append(" ORDER BY rank DESC");

            return sb.ToString();
        }

        private static List<string> ProccessSearchTextClean(string searchTextClean)
        {
            List<string> returnList = new List<string>();

            label:
            for (int i = 0; i < searchTextClean.Count(); i++)
            {
                if (searchTextClean[i] == '|' || searchTextClean[i] == '&') continue;
                if (searchTextClean[i] == '(')
                {
                    int j = searchTextClean.IndexOf(')');
                    returnList.Add(searchTextClean.Substring(i + 1, j - i - 1));
                    searchTextClean = searchTextClean.Remove(i, j - i + 1);
                    goto label;
                }
                else if (searchTextClean[i] == ' ' || (searchTextClean[i] > 64 && searchTextClean[i] < 123))
                {
                    if (searchTextClean[i + 1] == '|' || searchTextClean[i + 1] == '&' || (searchTextClean[i] > 64 && searchTextClean[i] < 123))
                    {
                        if (searchTextClean[i] > 64 && searchTextClean[i] < 123)
                        {
                            if (searchTextClean.IndexOf(' ') != -1)
                            {
                                returnList.Add(searchTextClean.Substring(0, searchTextClean.IndexOf(' ')));
                                searchTextClean = searchTextClean.Remove(0, searchTextClean.IndexOf(' '));
                                goto label;
                            }
                            else
                            {
                                returnList.Add(searchTextClean);
                                break;
                            }
                        }

                        searchTextClean = searchTextClean.Remove(0, 3);

                        if (searchTextClean[i] == '(')
                        {
                            int j = searchTextClean.IndexOf(')');
                            returnList.Add(searchTextClean.Substring(i + 1, j - i - 1));
                            searchTextClean = searchTextClean.Remove(i, j - i + 1);
                            goto label;
                        }
                        else
                        {
                            if (searchTextClean.IndexOf(' ') != -1)
                            {
                                returnList.Add(searchTextClean.Substring(0, searchTextClean.IndexOf(' ')));
                                searchTextClean = searchTextClean.Remove(0, searchTextClean.IndexOf(' '));
                                goto label;
                            }
                            else
                            {
                                returnList.Add(searchTextClean);
                                break;
                            }
                        }
                    }
                }
            }
            return returnList;
        }

        internal static object QueryAnalysis(string query)
        {

            using (var cmd = new NpgsqlCommand(query, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                    }
                }

            }
            return "test";
        }

        internal static void CreateTempTableHours()
        {
            conn = new NpgsqlConnection(connString);
            conn.Open();

            StringBuilder sb = new StringBuilder("CREATE TEMP TABLE hours(hour text);"); //FIXME : tip podataka ovdje
            NpgsqlCommand cmd = new NpgsqlCommand(sb.ToString(), conn);
            cmd.ExecuteNonQuery();

            sb = new StringBuilder();
            for (int i = 0; i < 24; i++)
            {
                sb.AppendLine("INSERT INTO hours VALUES ('" +i+ "-" +(i+1)+  "');");
            }

                cmd = new NpgsqlCommand(sb.ToString(), conn);
                cmd.ExecuteNonQuery();
        }

        internal static void CreateTempTableDays(double differenceInDays, DateTime dateFrom)
        {
            conn = new NpgsqlConnection(connString);
            conn.Open();

            StringBuilder sb = new StringBuilder("CREATE TEMP TABLE days(day date);");
            NpgsqlCommand cmd = new NpgsqlCommand(sb.ToString(), conn);
            cmd.ExecuteNonQuery();


            sb = new StringBuilder();
            for (int i = 0; i < differenceInDays; i++)
            {
                sb.AppendLine("INSERT INTO days VALUES ('"+ dateFrom.ToString("dd-MM-yyyy") + "');");
                dateFrom.AddDays(1);
            }
            cmd = new NpgsqlCommand(sb.ToString(), conn);
            cmd.ExecuteNonQuery();

        }

        internal static void DeleteTempTable(string tempTablename)
        {

            StringBuilder sb = new StringBuilder("DROP TABLE "+tempTablename);
            NpgsqlCommand cmd = new NpgsqlCommand(sb.ToString(), conn);
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        internal static void SaveQueryToDb(string text)
        {
            conn = new NpgsqlConnection(connString);
            conn.Open();

            StringBuilder sb = new StringBuilder("INSERT INTO \"Log\" (query, \"dateTime\") VALUES ('"+text+"', '"+DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")+"'); ");
            NpgsqlCommand cmd = new NpgsqlCommand(sb.ToString(), conn);
            int affectedRows = cmd.ExecuteNonQuery();

            conn.Close();

        }

        internal static string TimeAnalysisQueryBuilder(string v, DateTime dateFrom, DateTime dateTo)
        {
            StringBuilder sb = new StringBuilder();
            double differenceInDays = ((dateTo - dateFrom).TotalDays);

            sb.Append("SELECT * from crosstab('SELECT query, \"dateTime\", CAST(count(*) AS int) as number from \"Log\"");
            sb.Append(" WHERE \"dateTime\" BETWEEN to_timestamp(" + dateFrom.ToString("MM-dd-yyyy") + ") AND to_timestamp(" + dateTo.ToString("MM-dd-yyyy") + ") GROUP BY query, \"dateTime\" ORDER BY query, \"dateTime\"', ");

            if (v == "hour")
            {
                sb.Append("'SELECT hour from hours order by hour') AS pivotTable(query text ");
                for (int i = 0; i < differenceInDays; i++)
                {
                    sb.Append(", " + i+"-"+(i+1) + " INT");
                }
                sb.Append(");");
            }
            else
            {
                sb.Append("'SELECT day from days order by day') AS pivotTable(query text ");
                    for(int i = 0; i < differenceInDays; i++)
                {
                    sb.Append(", d"+i+" INT");
                }
                sb.Append(");");
            }
            return sb.ToString();
        }

        internal static Tuple<List<string>, List<float>, List<float>, List<float>, List<float>> QueryFuzzy(string query)
        {
            List<string> listOfDocTitlesBolded = new List<string>();
            List<float> listOfSimilarityTitle = new List<float>();
            List<float> listOfSimilarityKeywords = new List<float>();
            List<float> listOfSimilarityBody = new List<float>();
            List<float> listOfSimilaritySummary = new List<float>();

            conn = new NpgsqlConnection(connString);

            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = query;

                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        listOfDocTitlesBolded.Add(reader.GetString(0));
                        listOfSimilarityTitle.Add(reader.GetFloat(1));
                        listOfSimilarityKeywords.Add(reader.GetFloat(3));
                        listOfSimilarityBody.Add(reader.GetFloat(2));
                        listOfSimilaritySummary.Add(reader.GetFloat(4));
                    }
                conn.Close();
            }
            return new Tuple<List<string>, List<float>, List<float>, List<float>, List<float>>(listOfDocTitlesBolded, listOfSimilarityTitle, listOfSimilarityKeywords, listOfSimilarityBody, listOfSimilaritySummary);
        }

        private static string SplitText(string queryText, string logicalOperator)
        {
            StringBuilder sb = new StringBuilder();
            bool isInPhrase = false;

            for (int i = 0; i < queryText.Count(); i++)
            {
                if (queryText[i] == '\"')
                {
                    if (!isInPhrase) sb.Append("(");
                    else sb.Append(")");

                    isInPhrase = (isInPhrase == true) ? false : true;

                    if (i != queryText.Count() - 1) i++;
                }
                if (isInPhrase)
                {
                    if (queryText[i] == ' ') sb.Append(" & ");
                    else sb.Append(queryText[i]);
                }
                else if (queryText[i] == ' ') sb.Append(" " + logicalOperator + " ");
                else sb.Append(queryText[i]);
            }

            if (queryText[queryText.Count() - 1] == '\"') sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        internal static string FuzzySearchQueryBuilder(string boolOperator, string queryText)
        {
            string searchTextClean = SplitText(queryText, boolOperator);
            List<string> listofPhrases = ProccessSearchTextClean(searchTextClean);

            StringBuilder sb = new StringBuilder("SELECT \"title\", similarity (lower ('"+searchTextClean+ "'), lower (\"title\")) rank, similarity (lower ('"+searchTextClean+"'), lower (\"body\")) ");
            sb.Append(", similarity(lower('"+searchTextClean+ "'), lower (\"keywords\"))");
            sb.Append(", similarity(lower('"+searchTextClean+ "'), lower (\"summary\"))");
            sb.Append(" FROM \"Document\" WHERE ");

            for (int i = 0; i < listofPhrases.Count; i++)
            {
                sb.Append("(similarity(lower('" + listofPhrases.ElementAt(i) + "'), lower (\"title\")) > 0.1 AND  ");
                sb.Append("similarity(lower('" + listofPhrases.ElementAt(i) + "'), lower (\"keywords\")) > 0.1 AND  ");
                sb.Append("similarity(lower('" + listofPhrases.ElementAt(i) + "'), lower (\"summary\")) > 0.1 AND  ");
                sb.Append("similarity(lower('" + listofPhrases.ElementAt(i) + "'), lower (\"body\")) > 0.1 ) ");
                if (i != listofPhrases.Count - 1)
                {
                    sb.Append((boolOperator == "|") ? " OR " : " AND ");
                }

            }
            sb.Append(" ORDER BY rank DESC");

            return sb.ToString();
        }
    }
}
/*UPDATE "Document" SET 
"tsv" = setweight(to_tsvector('english', title), 'A') 
|| setweight(to_tsvector('english', keywords), 'B') 
|| setweight(to_tsvector('english', summary), 'C') 
|| setweight(to_tsvector('english', body), 'D');
*/
