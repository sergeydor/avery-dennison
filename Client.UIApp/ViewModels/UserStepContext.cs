using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.ViewModels
{
    public enum UserStep
    {
        NDef = -1,
        Initialization = 0,
        SetSettings = 1,
        RealtimeTest = 2,
        Playback = 3
    }

    public class StepTabState
    {
        public bool Enabled { get; set; } = false;
        public bool Visible { get; set; } = true;
    }

    public class UserStepContext : ViewModelBase
    {
        private static Lazy<UserStepContext> _instance = new Lazy<UserStepContext>();

        private UserStep _currentStep = UserStep.NDef;
        public UserStep CurrentStep
        {
            get { return _currentStep; }
            set
            {
                _currentStep = value;
                RaisePropertyChanged(() => CurrentStep);
                RaisePropertyChanged(() => CurrentStepIndex);
            }
        } 
        public int CurrentStepIndex
        {
            get { return (int)CurrentStep; }
        }

        private Dictionary<UserStep, StepTabState> _stepToEnabledAndVisibilityStateDict = new Dictionary<UserStep, StepTabState>();

        public Dictionary<UserStep, StepTabState> StepToEnabledAndVisibilityStateDict { get { return _stepToEnabledAndVisibilityStateDict; } }
        
        public static UserStepContext Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public UserStepContext()
        {
            int[] stepsIndexes = Enum.GetValues(typeof(UserStep)).Cast<int>().ToArray();
            foreach(int stepIdx in stepsIndexes)
            {
                if (stepIdx < 0) continue;
                UserStep step = (UserStep)stepIdx;
                _stepToEnabledAndVisibilityStateDict.Add(step, new StepTabState());
            }
        }

        public void MakeInitStepEnabled()
        {
            StepToEnabledAndVisibilityStateDict[UserStep.Initialization].Enabled = true;
            RaisePropertyChanged(() => StepToEnabledAndVisibilityStateDict);
            CurrentStep = UserStep.Initialization;
            RaisePropertyChanged(() => CurrentStep);
        }

        public void CompleteInitStep()
        {
            StepToEnabledAndVisibilityStateDict[UserStep.Initialization].Enabled = true; // leave enabled for now
            StepToEnabledAndVisibilityStateDict[UserStep.SetSettings].Enabled = true;            
            
            CurrentStep = UserStep.SetSettings;

            StepToEnabledAndVisibilityStateDict[UserStep.RealtimeTest].Enabled = true;

            RaisePropertyChanged(() => StepToEnabledAndVisibilityStateDict);
        }

        public void CompleteSetSettingsStep()
        {
            StepToEnabledAndVisibilityStateDict[UserStep.SetSettings].Enabled = true; // leave enabled for now
            StepToEnabledAndVisibilityStateDict[UserStep.RealtimeTest].Enabled = true;
            RaisePropertyChanged(() => StepToEnabledAndVisibilityStateDict);
            CurrentStep = UserStep.RealtimeTest;
        }


    }
}
