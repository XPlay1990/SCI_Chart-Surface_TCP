using SciChart_RealTime3DSurfaceMesh;
using System.Windows;

namespace SciChartExport {
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            SynchronousSocketClient tcpClient = new SynchronousSocketClient(SurfacePlotter);
            tcpClient.StartClient();
        }
    }
}
