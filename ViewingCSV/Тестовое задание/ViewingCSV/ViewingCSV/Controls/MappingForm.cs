using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ViewingCSV.Controls;
using static ViewingCSV.Controls.CSV;

namespace ViewingCSV
{
    partial class MainWindow
    {
        private Dictionary<Parametres, string> copyHeader = new Dictionary<Parametres, string>();

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if(table.Where(p => p.Parametre != Parametres.NoMapped && p.Parametre != Parametres.Ignore).Count()==0)
            {
                MessageBox.Show("Вы не все переключили параметры");
                return;
            }
            if (table.Where(p => p.Error != "").Count() != 0)
            {
                MessageBox.Show("Есть ошибки!!!");
                return;
            }
            for (int i = 0; i < table.Count(); i++)
            {
                for (int j = 0; j < table.Count(); j++)
                {
                    if(i!=j && table[i].Header==table[j].Header)
                    {
                        MessageBox.Show("Есть колонки повтор!!!");
                        return;
                    }
                }
            }

            Mapping.Visibility = Visibility.Collapsed;
            Uploading.Visibility = Visibility.Collapsed;
            Table.Visibility = Visibility.Visible;

            AmountPage = table[0].Rows.Count() / 50;
            Page = 0;
            NumberOfButtonP = 0;
            NumberOfButtonN = 10;
            if (NumberOfButtonN >= AmountPage)
                NumberOfButtonN = AmountPage;

            UpdatePage();
        }       

        private void SomeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combox = sender as ComboBox;

            var enumParametre = (Parametres)Enum.ToObject(typeof(Parametres), (sender as ComboBox).SelectedItem);
            switch (enumParametre)
            {
                case Parametres.SKU:
                case Parametres.Brand:
                case Parametres.Price:
                case Parametres.Weight:
                    if (table.Count(p => p.Parametre == enumParametre) > 1)
                    {
                        var k = copyHeader.FirstOrDefault(p1 => p1.Key == enumParametre);
                        table.FirstOrDefault(p => p.Header == k.Value).Parametre = Parametres.Ignore;
                        table.FirstOrDefault(p => p.Header == k.Value).Error = "";
                        copyHeader.Remove(k.Key);
                    }
                    break;
                default:
                    break;
            }

            if (copyHeader.Count(p => p.Key == enumParametre) >= 1)
                copyHeader.Remove(copyHeader.FirstOrDefault(p => p.Key == enumParametre).Key);

            var IsNumber = new Regex(@"^\d+(?:[\.]\d+)?$");
            switch (enumParametre)
            {
                case Parametres.NoMapped:
                case Parametres.Feature:
                case Parametres.Product:
                case Parametres.Ignore:
                    {
                        foreach (var item in table.Where(p => p.Parametre == enumParametre))
                        {
                            item.Error = "";
                            item.Parametre &= ~Parametres.NoMapped;
                        }
                        break;
                    }

                case Parametres.SKU:
                case Parametres.Brand:
                    {
                        foreach (var item in table.Where(p => p.Parametre == enumParametre))
                        {
                            bool isEmpty = false;
                            foreach (var row in item.Rows)
                                if (string.IsNullOrEmpty(row.ToString())) isEmpty = true;

                            item.Error = isEmpty ? "Есть пустые значения. " : "";

                            copyHeader.Add(enumParametre, item.Header);

                            item.Parametre &= ~Parametres.NoMapped;
                        }
                        break;
                    }

                case Parametres.Price:
                    {
                        foreach (var item in table.Where(p => p.Parametre == enumParametre))
                        {
                            bool isEmpty = false;
                            bool isNumber = true;
                            foreach (var row in item.Rows)
                            {
                                if (string.IsNullOrEmpty(row.ToString())) isEmpty = true;
                                if (!IsNumber.IsMatch(row.ToString())) isNumber = false;
                            }

                            item.Error = "";
                            item.Error += isEmpty ? "Есть пустые значения. " : "";
                            item.Error += !isNumber ? "Есть не числовые значения. " : "";

                            copyHeader.Add(enumParametre, item.Header);

                            item.Parametre &= ~Parametres.NoMapped;
                        }
                        break;
                    }

                case Parametres.Weight:
                    {
                        foreach (var item in table.Where(p => p.Parametre == enumParametre))
                        {
                            bool isNumber = true;
                            foreach (var row in item.Rows)
                                if (!IsNumber.IsMatch(row.ToString())) isNumber = false;

                            item.Error = !isNumber ? "Есть не числовые значения. " : "";

                            copyHeader.Add(enumParametre, item.Header);

                            item.Parametre &= ~Parametres.NoMapped;
                        }
                        break;
                    }

                default:
                    break;
            }

            GridMapping.ItemsSource = "";
            GridMapping.ItemsSource = table;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Mapping.Visibility = Visibility.Collapsed;
            Uploading.Visibility = Visibility.Visible;
            Table.Visibility = Visibility.Collapsed;

            table = null;

            Pages.Children.Clear();
            GridMapping.ItemsSource = "";
            NameFile.Content = "";
            copyHeader.Clear();
            Choose_Button.IsEnabled = false;
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var combox = sender as ComboBox;

            combox.ItemsSource = Enum.GetValues(typeof(Parametres));
            combox.Loaded -= ComboBox_Loaded;
            combox.SelectionChanged += SomeSelectionChanged;
        }
    }
}
