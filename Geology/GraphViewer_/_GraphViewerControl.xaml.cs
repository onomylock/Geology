using System;
using System.Linq;
using System.Windows.Controls;
using Geology.Tasks;
using System.Windows;

namespace Geology.GraphViewer
{
    /// <summary>
    /// Логика взаимодействия для GraphViewerControl.xaml
    /// </summary>
    public partial class GraphViewerControl : UserControl
    {
        private TableGraphPoint tableGraphPoint = new TableGraphPoint();

        public GraphViewerControl()
        {
            InitializeComponent();
            System.Collections.Generic.List<TaskTypes> list;

            DataContext = tableGraphPoint;

            tableGraphPoint.GraphControl = GraphOpenGlWindow;
            XYOpenGlWindow.graphControl = GraphOpenGlWindow;
            XYOpenGlWindow.tableGraph = tableGraphPoint;

            //if (LanguageSupport.LanguageLocalization.Culture == "en-US")
            {
                if (LimitedFunc.limited2)
                {
                    list = new System.Collections.Generic.List<TaskTypes>();
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

            tableGraphPoint.SelectionItemChanged += () =>
            {
                dataGridReceivers.ItemsSource = tableGraphPoint.GraphReceivers;
                dataGridTasks.ItemsSource = tableGraphPoint.TypeModelValues;
                dataGridPositions.ItemsSource = tableGraphPoint.ObservingPositions;
                dataGridCurves.ItemsSource = tableGraphPoint.Curves;
            };
        }
        private void dataGridGraphsDeviceComboBox_PreviewMouseDown(object sender, EventArgs e)
        {
            dataGridGraphsDeviceComboBox.ItemsSource = Observing.ObservingData.devices.Values.Select(d => d.Name);
        }

        private void buttonAddGraph_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tableGraphPoint.Add(new TableGraphPointValue() { Index = tableGraphPoint.Values.Count + 1 });
        }
        private void buttonRemoveGraph_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (tableGraphPoint.SelectedItem != null)
                tableGraphPoint.Remove(tableGraphPoint.SelectedItem);
        }

        private void buttonRemovePosition_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tableGraphPoint.Values.Clear();
        }
        private void buttonClearAllPositions_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }
        private void buttonCopyToAllPositions_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void ResetCurves()
        {
            foreach (var val in tableGraphPoint.Values)
            {
                val.ResetCurves();
            }
            GraphOpenGlWindow.CurvesList.UpdateLayout();
        }

        private void buttonResetCurves_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ResetCurves();
        }
    }
}