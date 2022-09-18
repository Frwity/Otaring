using Com.RandomDudes.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Com.RandomDudes.Utility {
    public static class JSONUtility
    {
        public static T[] CreateArrayFromJson<T>(string json) => CreateListFromJson<T>(json).ToArray();

        public static List<T> CreateListFromJson<T>(string json)
        {
            // Clean JSON from unuseful characters
            char[] removeChar = { '[', ']' };
            json = json.Trim(removeChar);

            string[] removeString = { "}," };
            string[] allResults = json.Split(removeString, StringSplitOptions.None);
            string splitResult;

            List<T> list = new List<T>();

            for (int i = 0; i < allResults.Length; i++)
            {
                splitResult = allResults[i];
                if (splitResult == string.Empty)
                    continue;
                splitResult = splitResult.Trim('}');
                splitResult += '}';
                list.Add(JsonUtility.FromJson<T>(splitResult));
            }

            return list;
        }

        public static string CreateJSONFromArray<T>(T[] array) => CreateJSONFromList(array.ToList());

        public static string CreateJSONFromList<T>(List<T> list)
        {
            string json = "[";
            int lLength = list.Count;

            for (int i = 0; i < lLength; i++)
            {
                json += JsonUtility.ToJson(list[i]);

                if (i != lLength - 1)
                    json += ",";
            }

            json += "]";
            DevLog.Error(json);

            return json;
        }
    }
}