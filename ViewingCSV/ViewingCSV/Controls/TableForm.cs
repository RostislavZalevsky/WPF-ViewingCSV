using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using ViewingCSV.Controls;
using static ViewingCSV.Controls.CSV;

namespace ViewingCSV
{
    partial class MainWindow
    {
        public int Page { get; set; }
        public double AmountPage { get; set; }
        public double NumberOfButtonP { get; set; }
        public double NumberOfButtonN { get; set; }

        private List<object> GetPage(List<object> list, int page, int pageSize)
        {
            return list.Skip(page * pageSize).Take(pageSize).ToList();
        }

        private ICollection CreateDataSource()
        {
            DataTable dt = new DataTable();
            DataRow dr;

            List<List<object>> copyRow = new List<List<object>>();
            var t = table.Where(p => p.Parametre != Parametres.NoMapped && p.Parametre != Parametres.Ignore).ToList();

            foreach (var item in t)
            {
                dt.Columns.Add(new DataColumn(item.Header, typeof(string)));
                copyRow.Add(GetPage(item.Rows, Page, 50));
            }

            for (int i = 0; i < copyRow[0].Count(); i++)
            {
                dr = dt.NewRow();
                for (int j = 0; j < copyRow.Count(); j++)
                {
                    dr[j] = copyRow[j].ElementAt(i);
                }
                dt.Rows.Add(dr);
            }

            DataView dv = new DataView(dt);
            return dv;
        }

        private void Tb_Checked(object sender, RoutedEventArgs e)
        {
            var b = sender as ToggleButton;
            Page = Convert.ToInt32(b.Content) - 1;
            Pages.Children.Clear();
            UpdatePage();
        }

        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (NumberOfButtonP > 0 && 10 > NumberOfButtonP)
            {
                NumberOfButtonP = 0;
                NumberOfButtonN = 10;
            }
            else if ((NumberOfButtonP - 10) < 0) return;

            NumberOfButtonP -= 10;
            NumberOfButtonN -= 10;

            if (NumberOfButtonN >= AmountPage)
                NumberOfButtonN = AmountPage;
            Pages.Children.Clear();
            UpdatePage();
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (AmountPage+1 <= NumberOfButtonN) return;

            NumberOfButtonP += 10;
            NumberOfButtonN += 10;

            if (NumberOfButtonN >= AmountPage+1)
                NumberOfButtonN = AmountPage+1;

            if (NumberOfButtonP >= AmountPage)
                NumberOfButtonP = AmountPage-10;

            Pages.Children.Clear();
            UpdatePage();
        }


        private void PButton_Click(object sender, RoutedEventArgs e)
        {
            if (Page == 0) return;
            Page--;
            if (NumberOfButtonN >= AmountPage)
                NumberOfButtonN = AmountPage;

            Pages.Children.Clear();
            UpdatePage();
        }

        private void NButton_Click(object sender, RoutedEventArgs e)
        {
            if (Page >= AmountPage) return;
            Page++;
            if (NumberOfButtonN >= AmountPage)
                NumberOfButtonN = AmountPage;

            Pages.Children.Clear();
            UpdatePage();
        }

        private void UpdatePage()
        {
            for (double i = NumberOfButtonP; i < NumberOfButtonN; i++)
            {
                ToggleButton tb = new ToggleButton();
                if (i == Page) tb.IsChecked = true; ;
                tb.Content = i + 1;
                tb.Checked += Tb_Checked;
                tb.FontSize = 20;

                Pages.Children.Add(tb);
            }

            GridTable.ItemsSource = CreateDataSource();
        }
    }
}
