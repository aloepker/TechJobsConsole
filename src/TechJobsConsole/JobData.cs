﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TechJobsConsole
{
    class JobData
    {
        static List<Dictionary<string, string>> AllJobs = new List<Dictionary<string, string>>();
        static bool IsDataLoaded = false;

        public static List<Dictionary<string, string>> FindAll()
        {
            LoadData();
            //???
            return AllJobs;
        }

        /*
         * Returns a list of all values contained in a given column,
         * without duplicates. 
         */
        public static List<string> FindAll(string column)
        {
            LoadData();

            List<string> values = new List<string>();

            foreach (Dictionary<string, string> job in AllJobs)
            {
                string aValue = job[column];

                if (!values.Contains(aValue))
                {
                    values.Add(aValue);
                }
            }
            return values;
        }

        public static List<Dictionary<string, string>> FindByColumnAndValue(string column, string value)     ///  <<< corrected case sensitivity!!
        {
            LoadData();

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> row in AllJobs)
            {
                string aValue = row[column];

                if (aValue.ToUpper().Contains(value.ToUpper()))
                {
                    jobs.Add(row);
                }
            }

            return jobs;
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

            // Parse each row array into a more friendly Dictionary
            foreach (string[] row in rows)
            {
                Dictionary<string, string> rowDict = new Dictionary<string, string>();

                for (int i = 0; i < headers.Length; i++)
                {
                    rowDict.Add(headers[i], row[i]);
                }
                AllJobs.Add(rowDict);
            }

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
        public static List<Dictionary<string, string>> FindByValue(string searchValue)  //a search that looks for a string in all the collums
        {
            LoadData();
            List<Dictionary<string, string>> searchedJobs = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> row in AllJobs)   //foreach dictionary in a list : (iterates through a list of dictionaries)
            {
                foreach (KeyValuePair<string, string> attribute in row)   //for each thing in the dictionary, do the following
                {
                    //if found, compair it and add to list searchedJobs!                    
                    if (attribute.Value.ToUpper().Contains(searchValue.ToUpper()))
                    {
                        //  my comparison to other values in the list 
                        bool notAlreadyFound = true;
                        //cycle through
                        for (int each = 0; each < searchedJobs.Count; each++)
                        {

                            if (searchedJobs[each].Count == row.Count)
                            {
                                notAlreadyFound = false;
                                foreach (KeyValuePair<string, string> pair in searchedJobs[each])
                                {
                                    string value;
                                    if (row.TryGetValue(pair.Key, out value))
                                    {
                                        if (value != pair.Value)
                                        {
                                            notAlreadyFound = true;
                                        }
                                    }                                  
                                }
                            }
                        }     
                        if (notAlreadyFound) { searchedJobs.Add(row); }
                    }  
                }
            }
            
            return searchedJobs;
        }
    }
}