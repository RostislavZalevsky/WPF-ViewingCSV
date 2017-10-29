using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewingCSV.Controls
{
    public class CSV
    {
        public string Header { get; set; }
        public List<object> Rows = new List<object>();
        public Parametres Parametre { get; set; }
        public string Example { get; set; }
        public string Error { get; set; }

        public enum Parametres
        {
            NoMapped,
            SKU, Brand, Price, Weight, Feature, Product,
            Ignore​
        }

        public static List<CSV> GetTable(string fileName)
        {
            var list = new List<CSV>();
            var lines = File.ReadAllLines(fileName);

            bool isHeader = true;

            foreach (var line in lines)
            {
                var cells = line.Split(',');

                if (isHeader == true)
                {
                    isHeader = false;
                    foreach (var cell in cells)
                    {
                        list.Add(new CSV { Header = cell });
                    }
                    continue;
                }

                for (int i = 0; i < cells.Length; i++)
                {
                    list[i].Rows.Add(cells[i]);
                }
            }

            for (int i = 0; i < list.Select(p => p.Header).Count(); i++)
            {
                string example = "";
                int begin = 0;
                int end = 4;

                for (int j = 0; j < list[i].Rows.Count(); j++)
                {
                    if (string.IsNullOrEmpty(list[i].Rows[j].ToString()))
                    {
                        begin++;
                        end++;
                        continue;
                    }

                    if (j == begin)
                    {
                        example += list[i].Rows[j];
                        continue;
                    }

                    example += " / " + list[i].Rows[j];

                    if (j == end && list[i].Rows.Count() >= end + 1)
                    {
                        example += " / ...";
                        break;
                    }
                }

                list[i].Parametre = Parametres.NoMapped;
                list[i].Example = example;
                list[i].Error = "";
            }

            return list;
        }
    }
}
