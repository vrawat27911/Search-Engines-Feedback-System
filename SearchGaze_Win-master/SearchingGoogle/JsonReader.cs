using Newtonsoft.Json;
using System;
//using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SearchingGoogle
{
    public class JsonReader
    {
        public List<ResultItem> LoadJson(string filename)
        {
            if (File.Exists(filename))
            {
                List<ResultItem> items;
                using (StreamReader r = new StreamReader(filename))
                {
                    string json = r.ReadToEnd();
                    items = JsonConvert.DeserializeObject<List<ResultItem>>(json);
                    dynamic result = JsonConvert.DeserializeObject(json);
                }
                return items;
            }
            else
            {
                return null;
            }
        }

        public List<GazeNClickResult> LoadJsonNew(string filename)
        {
            if (File.Exists(filename))
            {
                List<GazeNClickResult> items;
                using (StreamReader r = new StreamReader(filename))
                {
                    string json = r.ReadToEnd();
                    items = JsonConvert.DeserializeObject<List<GazeNClickResult>>(json);
                    dynamic result = JsonConvert.DeserializeObject(json);
                }
                return items;
            }
            else
            {
                return null;
            }
        }
    }

    public class ResultItem
    {   public string link;
        public string title;
        public string description;
        public int rank;
    }

    public class GazeNClickResult: IComparable
    {
        public string link;
        public string title;
        public string description;
        public int rank;
        public int gazeRank;
        public int gazeCount = 0;
        public int clickCount = 0;

        public static IComparer<GazeNClickResult> sortByClkNGaze()
        {
            return (IComparer<GazeNClickResult>)new sortByClkNGazeHelper();
        }

        public static IComparer<GazeNClickResult> sortByRank()
        {
            return (IComparer<GazeNClickResult>)new sortByRankHelper();
        }

        int IComparable.CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        private class sortByClkNGazeHelper : IComparer<GazeNClickResult>
        {
            public int Compare(GazeNClickResult a, GazeNClickResult b)
            {
                GazeNClickResult a_gc = (GazeNClickResult)a;
                GazeNClickResult b_gc = (GazeNClickResult)b;

                //count more means rank should be less.
                if ((a_gc.gazeCount/100 + a_gc.clickCount) < (b_gc.gazeCount/100 + b_gc.clickCount))
                    return 1;
                else if ((a_gc.gazeCount/100 + a_gc.clickCount) > (b_gc.gazeCount/100 + b_gc.clickCount))
                    return -1;
                //else if ((a_gc.gazeCount + a_gc.clickCount) == (b_gc.gazeCount + b_gc.clickCount))
                else
                {
                    if (a_gc.rank > b_gc.rank)
                        return 1;
                    else if (a_gc.rank < b_gc.rank)
                        return -1;
                    else
                        return 0;
                }
            }
        }
        
        private class sortByRankHelper : IComparer<GazeNClickResult>
        {
            public int Compare(GazeNClickResult a, GazeNClickResult b)
            {
                GazeNClickResult a_gc = (GazeNClickResult)a;
                GazeNClickResult b_gc = (GazeNClickResult)b;
                
                if (a_gc.rank > b_gc.rank)
                    return 1;
                else if (a_gc.rank < b_gc.rank)
                    return -1;
                else
                    return 0;
            }
        }
    }
}
