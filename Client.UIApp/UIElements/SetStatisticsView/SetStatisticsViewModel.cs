using Common.Domain.TestSetupCommands;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.SetStatisticsView
{
    public class SetStatisticsViewModel : ViewModelBase
    {
        public TestStatistics TestStatistics { get; set; }

        public void UpdateViewModel(TestStatistics testStatistics)
        {
            this.TestStatistics = testStatistics;
            RaisePropertyChanged(() => TestStatistics);
        }
    }
}
