using SubstanceMeltingTemps.Substance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.IO;
using System.Threading;
using System.Globalization;

namespace SubstanceMeltingTemps
{
    public partial class MainWindow : Window
    {
        public InstrumentsFile InstrumentsFile { get; set; }
        public Instrument SelectedInstrument { get; set; }

        private LineSeries dataSeries = new LineSeries
        {
            MarkerFill = OxyColors.Red,
            MarkerStroke = OxyColors.Red,
            MarkerSize = 1,
            MarkerType = MarkerType.Circle,
            StrokeThickness = 0
        };
        private LineSeries regressionLine = new LineSeries
        {
            StrokeThickness = 1,
            Color = OxyColors.Blue
        };
        private bool isC = true;

        private LinearAxis nominalAxis;
        private LinearAxis measuredAxis;

        private static readonly Regex numericOnly = new Regex("[^0-9.-]+");
        private static readonly Regex naturalNumbersOnly = new Regex("^[1-9][0-9]*$");

        public MainWindow()
        {;
            InstrumentsFile = new InstrumentsFile();
            InitializeComponent();
            DataContext = this;
            NamesComboBox.ItemsSource = BaseSubstance.PredefinedSubstances;

            DataPlotModel.Series.Add(dataSeries);
            DataPlotModel.Series.Add(regressionLine);
            SetLanguageDictionary();
        }

        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "pl-PL":
                    dict.Source = new Uri("..\\Resources\\Resources.pl-PL.xaml", UriKind.Relative);
                    nominalAxis = new LinearAxis()
                    {
                        Position = AxisPosition.Left,
                        Minimum = 0,
                        AbsoluteMinimum = 0,
                        MajorGridlineStyle = LineStyle.Dash,
                        Title = "Temperatura nominalna"
                    };
                    measuredAxis = new LinearAxis()
                    {
                        Position = AxisPosition.Bottom,
                        Minimum = 0,
                        AbsoluteMinimum = 0,
                        MajorGridlineStyle = LineStyle.Dash,
                        Title = "Temperatura zmierzona"
                    };
                    DataPlotModel.Axes.Clear();
                    DataPlotModel.Axes.Add(nominalAxis);
                    DataPlotModel.Axes.Add(measuredAxis);
                    DataPlotModel.InvalidatePlot(true);
                    break;
                case "en-US":
                    default:
                    dict.Source = new Uri("..\\Resources\\Resources.en-EN.xaml", UriKind.Relative);
                    nominalAxis = new LinearAxis()
                    {
                        Position = AxisPosition.Left,
                        Minimum = 0,
                        AbsoluteMinimum = 0,
                        MajorGridlineStyle = LineStyle.Dash,
                        Title = "Nominal temperature"
                    };
                    measuredAxis = new LinearAxis()
                    {
                        Position = AxisPosition.Bottom,
                        Minimum = 0,
                        AbsoluteMinimum = 0,
                        MajorGridlineStyle = LineStyle.Dash,
                        Title = "Measured temperature"
                    };
                    DataPlotModel.Axes.Clear();
                    DataPlotModel.Axes.Add(nominalAxis);
                    DataPlotModel.Axes.Add(measuredAxis);
                    DataPlotModel.InvalidatePlot(true);
                    break;
            }
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(dict);
        }

        private void ChangeScale() 
        {
            if (isC)
            {
                MeasTemp.SetResourceReference(ContentProperty, "Measured temperature [°C]");
                NomTemp.SetResourceReference(ContentProperty, "Nominal temperature [°C]");
                NamesComboBox.ItemsSource = BaseSubstance.PredefinedSubstances;
            }
            else
            {
                MeasTemp.SetResourceReference(ContentProperty, "Measured temperature [K]");
                NomTemp.SetResourceReference(ContentProperty, "Nominal temperature [K]");
                var predefinedInF = new List<BaseSubstance>();
                foreach (var substance in BaseSubstance.PredefinedSubstances) 
                {
                    predefinedInF.Add(new BaseSubstance(substance.Name,Utilities.CtoF(substance.NominalMeltingTemperatureC)));
                }
                NamesComboBox.ItemsSource = predefinedInF;
            }
            RefreshSubstanceList();
            DataPlotModel.InvalidatePlot(true);
        }

        private void OnPredefinedSelected(object sender, RoutedEventArgs e)
        {
            var selecedItem = NamesComboBox.SelectedItem as BaseSubstance;
            if (selecedItem!=null)
            {
                NominalTemperatureTextBox.Text = selecedItem.NominalMeltingTemperatureC.ToString();
            }
        }

        private void AddSubstanceButton_Click(object sender, RoutedEventArgs e)
        {
            var name = NamesComboBox.Text;
            float nominalMeltingTemperature;
            float measuredMeltingTemperature;
            var canParseNominalMeltingTemperature = float.TryParse(NominalTemperatureTextBox.Text, out nominalMeltingTemperature);
            var canParseMeasuredMeltingTemperature = float.TryParse(MeasuredTemperatureTextBox.Text, out measuredMeltingTemperature);
            if (canParseNominalMeltingTemperature && canParseMeasuredMeltingTemperature) 
            {
                if (isC)
                {
                    SelectedInstrument.Add(new MeasuredSubstance(name, nominalMeltingTemperature, measuredMeltingTemperature));
                }
                else
                {
                    SelectedInstrument.Add(new MeasuredSubstance(name, Utilities.FtoC(nominalMeltingTemperature), Utilities.FtoC(measuredMeltingTemperature)));
                }
                NamesComboBox.Text = "";
                NominalTemperatureTextBox.Text = "";
                MeasuredTemperatureTextBox.Text = "";
                SubstanceDataListBox.Items.Add(new SubstanceData(SelectedInstrument.Last(), RemoveSubstance, UpdatePlot,isC));
                UpdatePlot();
            }
        }

        private void TextBoxNumberValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = numericOnly.IsMatch(e.Text);
        }
        private void TextBoxNaturalNumberValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !naturalNumbersOnly.IsMatch(e.Text);
        }

        private void InstrumentsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedInstrument = (Instrument)InstrumentsListBox.SelectedItem;
            RefreshSubstanceList();
        }

        private void RefreshSubstanceList()
        {
            SubstanceDataListBox.Items.Clear();
            if (SelectedInstrument == null) 
            {
                return;
            }
            foreach (var data in SelectedInstrument)
            {
                SubstanceDataListBox.Items.Add(new SubstanceData(data, RemoveSubstance,UpdatePlot, isC));
            }
            UpdatePlot();
        }

        private void UpdatePlot()
        {
            dataSeries.Points.Clear();
            regressionLine.Points.Clear();
            if (SelectedInstrument.Count == 0) 
            {
                return;
            }
            foreach (var data in SelectedInstrument)
            {
                if (isC)
                {
                    dataSeries.Points.Add(new DataPoint(data.NominalMeltingTemperatureC, data.MeasuredMeltingTemperatureC));
                }
                else
                {
                    dataSeries.Points.Add(new DataPoint(Utilities.CtoF(data.NominalMeltingTemperatureC), Utilities.CtoF(data.MeasuredMeltingTemperatureC)));
                }
            }
            if (isC)
            {
                var minValue = SelectedInstrument.Select(v => v.NominalMeltingTemperatureC).Min();
                var maxValue = SelectedInstrument.Select(v => v.NominalMeltingTemperatureC).Max();
                regressionLine.Points.Add(new DataPoint(minValue, SelectedInstrument.LinearRegression(minValue)));
                regressionLine.Points.Add(new DataPoint(maxValue, SelectedInstrument.LinearRegression(maxValue)));
            }
            else
            {
                var minValue = SelectedInstrument.Select(v => Utilities.CtoF(v.NominalMeltingTemperatureC)).Min();
                var maxValue = SelectedInstrument.Select(v => Utilities.CtoF(v.NominalMeltingTemperatureC)).Max();
                regressionLine.Points.Add(new DataPoint(minValue, SelectedInstrument.LinearRegression(minValue)));
                regressionLine.Points.Add(new DataPoint(maxValue, SelectedInstrument.LinearRegression(maxValue)));
            }
            DataPlotModel.InvalidatePlot(true);
            CorrelationCoefficient.Text = SelectedInstrument.CorrelationCoefficient.ToString();
            Alpha.Text = SelectedInstrument.Alpha.ToString();
            Beta.Text = SelectedInstrument.Beta.ToString();
        }

        private void AddInstrumentButton_Click(object sender, RoutedEventArgs e)
        {
            int id;
            if (!int.TryParse(NewInstrumentId.Text, out id)) 
            {
                return;
            }
            if (id < 1) 
            {
                return;
            }
            InstrumentsFile.Add(new Instrument(id));
            InstrumentsListBox.DataContext = null;
            InstrumentsListBox.DataContext = this;
        }

        private void RemoveSubstance(MeasuredSubstance substance) 
        {
            SelectedInstrument.Remove(substance);
            RefreshSubstanceList();
            UpdatePlot();
        }

        private void FitButton_Click(object sender, RoutedEventArgs e)
        {
            DataPlotModel.ResetAllAxes();
            DataPlotModel.InvalidatePlot(true);
        }

        private void ChangeToC_Click(object sender, RoutedEventArgs e)
        {
            if (isC) 
            {
                return;
            }
            isC = true;
            ChangeScale();
        }

        private void ChangeToF_Click(object sender, RoutedEventArgs e)
        {

            if (!isC)
            {
                return;
            }
            isC = false;
            ChangeScale();
        }

        private void English_Click(object sender, RoutedEventArgs e)
        {
            CultureInfo ci = new CultureInfo("en-EN");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            SetLanguageDictionary();
        }
        private void Polish_Click(object sender, RoutedEventArgs e)
        {
            CultureInfo ci = new CultureInfo("pl-PL");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            SetLanguageDictionary();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".xml";
            dialog.Filter = "XML documents (.xml)|*.xml";
            var result = dialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    InstrumentsFile = InstrumentsFile.FromXml(dialog.FileName);
                    InstrumentsListBox.DataContext = null;
                    InstrumentsListBox.DataContext = this;
                }
                catch 
                {
                    MessageBox.Show("Unable to open file "+ dialog.FileName, "File error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "InstrumentsFile";
            dialog.DefaultExt = ".xml";
            dialog.Filter = "XML documents (.xml)|*.xml";
            var result = dialog.ShowDialog();

            if (result == true)
            {
                InstrumentsFile.ToXml(dialog.FileName);
            }
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
