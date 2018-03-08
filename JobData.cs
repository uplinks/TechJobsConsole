using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

namespace TechJobsConsole
{
    class JobData
    {
        static ImmutableList<Dictionary<string, string>> AllJobs;
        static bool IsDataLoaded = false;

        public static ImmutableList<Dictionary<string, string>> FindAll()
        {
            LoadData();
            return AllJobs;
        }

        /*
         * Scan all columns in all jobs for a match with the search argument. When a match occurs add that job to the set.
         * Once all jobs have been found, make a list of jobs from the set and return the list.
         */
        public static List<Dictionary<string, string>> FindByValue(string searchArgument)
        {
            LoadData();
            HashSet<Dictionary<string, string>> foundJobsSet = new HashSet<Dictionary<string, string>>();
            List<Dictionary<string, string>> foundJobs = new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> job in AllJobs)
            {
                foreach (string key in job.Keys)
                {
                    if (job[key].IndexOf(searchArgument, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        foundJobsSet.Add(job);
                    }
                }
            }

            foreach (Dictionary<string, string> job in foundJobsSet)
            {
                foundJobs.Add(job);
            }

            return sortJobs(foundJobs, "employer");
        }

        /*
         * Returns a list of all values contained in a given column,
         * without duplicates. 
         */
        public static List<string> FindAll(string column)
        {
            LoadData();

            //List<string> values = new List<string>();
            HashSet<string> values = new HashSet<string>();

            foreach (Dictionary<string, string> job in AllJobs)
            {
                values.Add(job[column]);
            }
            return values.OrderBy(q => q).ToList();

        }

        /*
         * Returns a list of dictionaries that match a value contained in a given column.
         */
        public static List<Dictionary<string, string>> FindByColumnAndValue(string column, string value)
        {
            // load data, if not already loaded
            LoadData();

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> row in AllJobs)
            {
                string aValue = row[column];

                if (aValue.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    jobs.Add(row);
                }
            }

            return sortJobs(jobs, column);
        }

        /*
         * Load and parse data from job_data.csv
         */
        private static void LoadData()
        {

            if (IsDataLoaded)
            {
                return;
            }

            List<string[]> rows = new List<string[]>();

            using (StreamReader reader = File.OpenText("job_data.csv"))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    string[] rowArrray = CSVRowToStringArray(line);
                    if (rowArrray.Length > 0)
                    {
                        rows.Add(rowArrray);
                    }
                }
            }

            string[] headers = rows[0];
            rows.Remove(headers);

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();

            // Parse each row array into a more friendly Dictionary
            foreach (string[] row in rows)
            {
                Dictionary<string, string> rowDict = new Dictionary<string, string>();

                for (int i = 0; i < headers.Length; i++)
                {
                    rowDict.Add(headers[i], row[i]);
                }
                jobs.Add(rowDict);
            }

            AllJobs = jobs.ToImmutableList();

            IsDataLoaded = true;
        }

        /*
         * Parse a single line of a CSV file into a string array
         */
        private static string[] CSVRowToStringArray(string row, char fieldSeparator = ',', char stringSeparator = '\"')
        {
            bool isBetweenQuotes = false;
            StringBuilder valueBuilder = new StringBuilder();
            List<string> rowValues = new List<string>();

            // Loop through the row string one char at a time
            foreach (char c in row.ToCharArray())
            {
                if ((c == fieldSeparator && !isBetweenQuotes))
                {
                    rowValues.Add(valueBuilder.ToString());
                    valueBuilder.Clear();
                }
                else
                {
                    if (c == stringSeparator)
                    {
                        isBetweenQuotes = !isBetweenQuotes;
                    }
                    else
                    {
                        valueBuilder.Append(c);
                    }
                }
            }

            // Add the final value
            rowValues.Add(valueBuilder.ToString());
            valueBuilder.Clear();

            return rowValues.ToArray();
        }

        /*
        * Returns a sorted list of jobs. 
        */
        private static List<Dictionary<string, string>> sortJobs(List<Dictionary<string, string>> jobs, string column)
        {
            var orderedJobs = jobs.OrderBy(x => x[column]);
            return orderedJobs.ToList();
        }
    }
}
