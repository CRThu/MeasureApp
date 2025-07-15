using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeasureApp.View.Pages;

namespace MeasureApp.ViewModel
{
    public class MainWindowPages : NotificationObjectBase
    {
        //private DevicesConnectionPage devicesConnectionPage;
        //public DevicesConnectionPage DevicesConnectionPage
        //{
        //    get => devicesConnectionPage;
        //    set
        //    {
        //        devicesConnectionPage = value;
        //        RaisePropertyChanged(() => DevicesConnectionPage);
        //    }
        //}

        private DeviceCommDebugPage deviceCommDebugPage;
        public DeviceCommDebugPage DeviceCommDebugPage
        {
            get => deviceCommDebugPage;
            set
            {
                deviceCommDebugPage = value;
                RaisePropertyChanged(() => DeviceCommDebugPage);
            }
        }

        private Multimeter3458AControlPage multimeter3458AControlPage;
        public Multimeter3458AControlPage Multimeter3458AControlPage
        {
            get => multimeter3458AControlPage;
            set
            {
                multimeter3458AControlPage = value;
                RaisePropertyChanged(() => Multimeter3458AControlPage);
            }
        }

        private TempControlPage tempControlPage;
        public TempControlPage TempControlPage
        {
            get => tempControlPage;
            set
            {
                tempControlPage = value;
                RaisePropertyChanged(() => TempControlPage);
            }
        }

        private SerialPortInstructionPage serialPortInstructionPage;
        public SerialPortInstructionPage SerialPortInstructionPage
        {
            get => serialPortInstructionPage;
            set
            {
                serialPortInstructionPage = value;
                RaisePropertyChanged(() => SerialPortInstructionPage);
            }
        }

        private SerialPortPacketPage serialPortPacketPage;
        public SerialPortPacketPage SerialPortPacketPage
        {
            get => serialPortPacketPage;
            set
            {
                serialPortPacketPage = value;
                RaisePropertyChanged(() => SerialPortPacketPage);
            }
        }

        private DataStorageVisualizationPage dataStorageVisualizationPage;
        public DataStorageVisualizationPage DataStorageVisualizationPage
        {
            get => dataStorageVisualizationPage;
            set
            {
                dataStorageVisualizationPage = value;
                RaisePropertyChanged(() => DataStorageVisualizationPage);
            }
        }

        private AutomationPage automationPage;
        public AutomationPage AutomationPage
        {
            get => automationPage;
            set
            {
                automationPage = value;
                RaisePropertyChanged(() => AutomationPage);
            }
        }

        private FftAnalysisPage fftAnalysisPage;
        public FftAnalysisPage FftAnalysisPage
        {
            get => fftAnalysisPage;
            set
            {
                fftAnalysisPage = value;
                RaisePropertyChanged(() => FftAnalysisPage);
            }
        }

        private UtilityPage utilityPage;
        public UtilityPage UtilityPage
        {
            get => utilityPage;
            set
            {
                utilityPage = value;
                RaisePropertyChanged(() => UtilityPage);
            }
        }

        private SettingsPage settingsPage;
        public SettingsPage SettingsPage
        {
            get => settingsPage;
            set
            {
                settingsPage = value;
                RaisePropertyChanged(() => SettingsPage);
            }
        }

        private TestPage testPage;
        public TestPage TestPage
        {
            get => testPage;
            set
            {
                testPage = value;
                RaisePropertyChanged(() => TestPage);
            }
        }

        private RunTasksPage runTasksPage;
        public RunTasksPage RunTasksPage
        {
            get => runTasksPage;
            set
            {
                runTasksPage = value;
                RaisePropertyChanged(() => RunTasksPage);
            }
        }
        

        public MainWindowPages(object dataContext)
        {
            //DevicesConnectionPage = new DevicesConnectionPage(dataContext);
            DeviceCommDebugPage = new DeviceCommDebugPage(dataContext);
            Multimeter3458AControlPage = new Multimeter3458AControlPage(dataContext);
            TempControlPage = new TempControlPage(dataContext);
            SerialPortInstructionPage = new SerialPortInstructionPage(dataContext);
            SerialPortPacketPage = new SerialPortPacketPage(dataContext);
            DataStorageVisualizationPage = new DataStorageVisualizationPage(dataContext);
            AutomationPage = new AutomationPage(dataContext);
            FftAnalysisPage = new FftAnalysisPage(dataContext);
            UtilityPage = new UtilityPage(dataContext);
            SettingsPage = new SettingsPage(dataContext);
            TestPage = new TestPage(dataContext);
            RunTasksPage = new RunTasksPage(dataContext);
        }
    }
}
