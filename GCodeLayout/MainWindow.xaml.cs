using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GCodeLayout
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<ProgramLine> CompleteProgram;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "NC Programs (*.NC)|*.NC|All Files (*.*)|*.*";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == true)
            {
                ParseProgram(ofd.FileName);
                
                if (CompleteProgram == null) return;

                lvProgData.ItemsSource = CompleteProgram.Where(i => i.Comments.Length > 0);
            }
        }

        private void ParseProgram(String ProgramPath)
        {
            if (!System.IO.File.Exists(ProgramPath)) return;

            CompleteProgram = new ObservableCollection<ProgramLine>();

            try
            {
                IEnumerable<String> lines = File.ReadLines(ProgramPath);
                int lineNum = 0;
                foreach (var s in lines)
                {
                    lineNum++;
                    CompleteProgram.Add(new ProgramLine(s, lineNum));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Reading File\n" + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Decimal shiftAmount;
            if(!Decimal.TryParse(tbZShift.Text, out shiftAmount)) return;

            foreach (var i in lvProgData.SelectedItems)
            {
                var line = i as ProgramLine;
                if (line != null)
                {
                    line.ShiftZ(shiftAmount, cbAboveZero.IsChecked ?? false);
                }
            }

            lvProgData.Items.Refresh();
            //lvProgData.ItemsSource = CompleteProgram.Where(i => i.Comments.Length > 0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(String.Join("\n", CompleteProgram.Select(p => p.LineContent)));
        }
    }
}
