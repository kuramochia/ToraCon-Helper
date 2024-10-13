using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using ToraConHelper.ViewModels;

namespace ToraConHelper.Views
{
    /// <summary>
    /// PowerToysPage.xaml の相互作用ロジック
    /// </summary>
    public partial class PowerToysPage : Page
    {
        public PowerToysPage(ViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
