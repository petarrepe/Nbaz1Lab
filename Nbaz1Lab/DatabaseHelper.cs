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

        internal static NpgsqlDataReader QueryFuzzy(string query)
        {
            throw new NotImplementedException();
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
            StringBuilder sb = new StringBuilder();

            throw new NotImplementedException();
        }
    }
}
/*UPDATE "Document" SET 
"tsv" = setweight(to_tsvector('english', title), 'A') 
|| setweight(to_tsvector('english', keywords), 'B') 
|| setweight(to_tsvector('english', summary), 'C') 
|| setweight(to_tsvector('english', body), 'D');
*/
