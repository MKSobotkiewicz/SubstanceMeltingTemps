using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
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

namespace SubstanceMeltingTemps.Substance
{
    /// <summary>
    /// Logika interakcji dla klasy SubstanceData.xaml
    /// </summary>
    public partial class SubstanceData : UserControl
    {
        private readonly MeasuredSubstance MeasuredSubstance;
        private readonly Instrument Instrument;
        private bool editState = false;
        private Action<MeasuredSubstance> DeleteSubstance;
        private Action UpdatePlot;
        private bool isC;

        public SubstanceData(MeasuredSubstance measuredSubstance, Action<MeasuredSubstance> deleteSubstance, Action updatePlot, bool isC)
        {
            this.isC = isC;
            MeasuredSubstance = measuredSubstance;
            DeleteSubstance = deleteSubstance;
            UpdatePlot = updatePlot;
            InitializeComponent();
            DataContext = this;
            NameTextBox.Text = MeasuredSubstance.Name;
            if (this.isC)
            {
                NominalTemperatureTextBox.Text = MeasuredSubstance.NominalMeltingTemperatureC.ToString();
                MeasuredTemperatureTextBox.Text = MeasuredSubstance.MeasuredMeltingTemperatureC.ToString();
            }
            else
            {
                NominalTemperatureTextBox.Text = Utilities.CtoF(MeasuredSubstance.NominalMeltingTemperatureC).ToString();
                MeasuredTemperatureTextBox.Text = Utilities.CtoF(MeasuredSubstance.MeasuredMeltingTemperatureC).ToString();
            }
        }

        private void EditSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (editState) 
            {
                editState = false;
                NameTextBox.IsEnabled = false;
                NominalTemperatureTextBox.IsEnabled = false;
                MeasuredTemperatureTextBox.IsEnabled = false;
                EditSaveButton.SetResourceReference(ContentProperty, "Edit");
                if (this.isC)
                {
                    MeasuredSubstance.Update(NameTextBox.Text, float.Parse(NominalTemperatureTextBox.Text), float.Parse(MeasuredTemperatureTextBox.Text));
                }
                else
                {
                    MeasuredSubstance.Update(NameTextBox.Text, Utilities.FtoC(float.Parse(NominalTemperatureTextBox.Text)), Utilities.FtoC(float.Parse(MeasuredTemperatureTextBox.Text)));
                }
                UpdatePlot();
                return;
            }
            editState = true;
            NameTextBox.IsEnabled = true;
            NominalTemperatureTextBox.IsEnabled = true;
            MeasuredTemperatureTextBox.IsEnabled = true;
            EditSaveButton.SetResourceReference(ContentProperty, "Save");
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteSubstance(MeasuredSubstance);
        }
    }
}
