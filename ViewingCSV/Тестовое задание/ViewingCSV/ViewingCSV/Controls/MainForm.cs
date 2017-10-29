using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewingCSV.Controls;
using static ViewingCSV.Controls.CSV;

namespace ViewingCSV
{
    partial class MainWindow
    {
        private List<CSV> table = new List<CSV>(); //{ get; set; }        

        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "CSV|*.csv|" +
                "All files|*.*";
            dialog.Title = "Выберите .CSV файл";

            if (dialog.ShowDialog() == true)
            {
                FileInfo info = new FileInfo(dialog.FileName);
                if (info.Extension != ".csv")
                {
                    MessageBox.Show("Указанный формат не поддерживается", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Choose_Button.IsEnabled = false;
                    return;
                }

                table = GetTable(dialog.FileName);
                if (table != null) Choose_Button.IsEnabled = true;
                else MessageBox.Show("Пустая база данных", "", MessageBoxButton.OK, MessageBoxImage.Error);
                NameFile.Content = info.Name;            
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            Uploading.Visibility = Visibility.Collapsed;
            Mapping.Visibility = Visibility.Visible;

            GridMapping.ItemsSource = table;
        }
        
    }
}
