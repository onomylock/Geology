using System;
using System.Linq;
using System.Windows.Controls;
using Geology.Tasks;

namespace Geology.GraphViewer
{
    /// <summary>
    /// Логика взаимодействия для GraphViewerControl.xaml
    /// </summary>
    public partial class GraphViewerControlProfile : UserControl
    {
        public TimeInfo timeInfo;
        private TableGraphProfile tableGraphProfile = new TableGraphProfile();

        public GraphViewerControlProfile()
        {
            InitializeComponent();

            DataContext = tableGraphProfile;

            tableGraphProfile.GraphControl = GraphOpenGlWindow;
            XYOpenGlWindow.graphControl = GraphOpenGlWindow;
            XYOpenGlWindow.tableGraph = tableGraphProfile;

            //if (LanguageSupport.LanguageLocalization.Culture == "en-US")
            {
                if (LimitedFunc.limited2)
                {
                    var list = new System.Collections.Generic.List<TaskTypes>();
                    list.Add(TaskTypes.None);
                    list.Add(TaskTypes.EM);
                    list.Add(TaskTypes.Magnite);
                    dataGridGraphsGraphTypeComboBox.ItemsSource = list;
                }
                else
                {
                    dataGridGraphsGraphTypeComboBox.ItemsSource = (TaskTypes[])Enum.GetValues(typeof(TaskTypes));
                }
            }
            //else
            //{
            //    if (LimitedFunc.limited2)
            //    {
            //        var listS = new System.Collections.Generic.List<String>();
            //        listS.Add(LanguageSupport.LanguageLocalization.None);
            //        listS.Add(LanguageSupport.LanguageLocalization.EM);
            //        listS.Add(LanguageSupport.LanguageLocalization.Magnite);
            //        dataGridGraphsGraphTypeComboBox.ItemsSource = listS;
            //    }
            //    else
            //    {
            //        var listS = new System.Collections.Generic.List<String>();
            //        listS.Add(LanguageSupport.LanguageLocalization.None);
            //        listS.Add(LanguageSupport.LanguageLocalization.IP);
            //        listS.Add(LanguageSupport.LanguageLocalization.EM);
            //        listS.Add(LanguageSupport.LanguageLocalization.EMIP);
            //        listS.Add(LanguageSupport.LanguageLocalization.Harmonic);
            //        listS.Add(LanguageSupport.LanguageLocalization.ColeCole);
            //        listS.Add(LanguageSupport.LanguageLocalization.Stationar);
            //        listS.Add(LanguageSupport.LanguageLocalization.MTZ);
            //        listS.Add(LanguageSupport.LanguageLocalization.Magnite);
            //        dataGridGraphsGraphTypeComboBox.ItemsSource = listS;
            //    }
            //}
            dataGridGraphsDirectionComboBox.ItemsSource = (ProfileDirection[])Enum.GetValues(typeof(ProfileDirection));

            tableGraphProfile.SelectionItemChanged += () =>
            {
                dataGridTasks.ItemsSource = tableGraphProfile.TypeModelValues;
                dataGridPositions.ItemsSource = tableGraphProfile.ObservingPositions;
                dataGridCurves.ItemsSource = tableGraphProfile.Curves;
            };
        }

        private void dataGridGraphsDeviceComboBox_PreviewMouseDown(object sender, EventArgs e)
        {
            dataGridGraphsDeviceComboBox.ItemsSource = Observing.ObservingData.devices.Values.Select(d => d.Name);
            //dataGridReceivers.ItemsSource = tableGraphPoint.GraphReceivers;
        }
        private void dataGridGraphsTimeComboBox_PreviewMouseDown(object sender, EventArgs e)
        {
            if (timeInfo != null && timeInfo.meshTimeUser != null)
                dataGridGraphsTimeComboBox.ItemsSource = timeInfo.meshTimeUser.times;
        }
        private void dataGridGraphs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        } // DELETE

        private void buttonAddGraph_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tableGraphProfile.Add(new TableGraphProfileValue() { Index = tableGraphProfile.Values.Count + 1 });
        }
        private void buttonRemoveGraph_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (tableGraphProfile.SelectedItem != null)
                tableGraphProfile.Remove(tableGraphProfile.SelectedItem);
        }

        private void buttonRemovePosition_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tableGraphProfile.Values.Clear();
        }
        private void buttonClearAllPositions_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }
        private void buttonCopyToAllPositions_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void ResetCurves()
        {
            foreach (var val in tableGraphProfile.Values)
            {
                val.ResetCurves();
            }
        }

        private void buttonResetCurves_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ResetCurves();
        }
    }
}