using Common.Enums;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Client.UIApp.UIElements.TestModeFlagsView
{
    public class TestModeFlagsViewModel : ViewModelBase
    {
        public Test10Type TargetFlagsEnum { get; set; } = Test10Type.ReadTest;

        public bool WriteTest
        {
            get { return (TargetFlagsEnum & Test10Type.WriteTest) != 0; }
            set
            {
                if(!value)
                    TargetFlagsEnum &= ~Test10Type.WriteTest;
                else
                    TargetFlagsEnum |= Test10Type.WriteTest;
                RaisePropertyChanged(() => WriteTest);
                if (value) SensTest = false;
            }
        }

        public bool ReadTest
        {
            get { return (TargetFlagsEnum & Test10Type.ReadTest) != 0; }
            set
            {
                if(!value)
                    TargetFlagsEnum &= ~Test10Type.ReadTest;
                else
                    TargetFlagsEnum |= Test10Type.ReadTest;

                RaisePropertyChanged(() => ReadTest);
                if (value) SensTest = false;
            }
        }

        public bool SensTest
        {
            get { return (TargetFlagsEnum & Test10Type.SensTest) != 0; }
            set
            {
                if(!value)
                    TargetFlagsEnum &= ~Test10Type.SensTest;
                else
                    TargetFlagsEnum |= Test10Type.SensTest;

                RaisePropertyChanged(() => SensTest);
                if(value)
                {
                    WriteTest = false;
                    ReadTest = false;
                    TIDTest = false;
                    IDFilter = false;
                }
            }
        }

        public bool TIDTest
        {
            get { return (TargetFlagsEnum & Test10Type.TIDTest) != 0; }
            set
            {
                if(!value)
                    TargetFlagsEnum &= ~Test10Type.TIDTest;
                else
                    TargetFlagsEnum |= Test10Type.TIDTest;

                RaisePropertyChanged(() => TIDTest);
                if (value) SensTest = false;
            }
        }

        public bool IDFilter
        {
            get { return (TargetFlagsEnum & Test10Type.IDFilter) != 0; }
            set
            {
                if(!value)
                    TargetFlagsEnum &= ~Test10Type.IDFilter;
                else
                    TargetFlagsEnum |= Test10Type.IDFilter;

                RaisePropertyChanged(() => IDFilter);
                if (value) SensTest = false;
            }
        }
        
        public void UpdateViewModel(Test10Type tt)
        {
            this.TargetFlagsEnum = tt;
            RaisePropertyChanged(() => WriteTest);
            RaisePropertyChanged(() => ReadTest);
            RaisePropertyChanged(() => SensTest);
            RaisePropertyChanged(() => TIDTest);
            RaisePropertyChanged(() => IDFilter);
        }
    }
}
