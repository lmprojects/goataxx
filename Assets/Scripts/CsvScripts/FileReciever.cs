using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarGames.Scripts
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    internal class ReceiveFrom : Attribute
    {
        public string Source { get; protected set; }

        public ReceiveFrom(string source)
        {
            this.Source = source;
        }
    }

    public abstract class DataSource : Dictionary<string, string>
    {
        public abstract IEnumerator GetData(Action callback);
    }

    abstract public class FileReciever
    {
        public abstract void Recieve(DataSource dataSource);

        public  static List<T> LoadData<T>(string text) where T : BaseItem
        {
            var grid = CsvReader.SplitCsvGrid(text);

            List<T> valueList = new List<T>();
            var list = ReadToDict(grid);
            foreach (var dict in list)
            {
                //Debug.Log(dict.ToStringCustomDict());
                T info = Activator.CreateInstance<T>();
                info.ReadDictionary(dict);
                valueList.Add(info);
            }
            return valueList;
        }

        protected static List<Dictionary<string, string>> ReadToDict(string[,] grid)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            ReadToList(grid, (keys, row) =>
            {
                if (row.All(o => string.IsNullOrEmpty(o)))
                    return;
                Dictionary<string, string> dict = new Dictionary<string, string>();
                for (int i = 0; i < keys.Count; i++)
                {
                    string key = keys[i].ToLower();
                    string value = row[i];
                    dict[key] = value;
                }
                list.Add(dict);
            });
            return list;
        }

        protected static void ReadToList(string[,] grid, Action<List<string>, List<string>> rowAction)
        {
            List<string> keys = new List<string>();
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                string value = grid[x, 0];
                if (value != null)
                    keys.Add(value);
                else
                    keys.Add("");
            }

            for (int y = 1; y < grid.GetLength(1); y++)
            {
                List<string> row = Enumerable.Range(0, grid.GetLength(0)).Select(o => string.IsNullOrEmpty(grid[o, y]) ? "" : grid[o, y]).ToList();
                rowAction(keys, row);
            }
        }
    }
}
