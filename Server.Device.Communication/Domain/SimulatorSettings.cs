﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Device.Communication.Domain
{
    public class SimulatorSettings
    {
        public int EncoderStepsPerTag { get; set; } = 0;

        //public double ScannerStepInMm { get; set; } = 1.000000f;
        //public int EncoderStepInMsec { get; set; } = 10;

        /// <summary>
        /// mm per sec
        /// </summary>
        //public double VelocityInMMPerMSec { get; set; } = 0; //calculated, not in config

        public int EncoderReaderTagsDistance { get; set; } = 0; // this one is set from UI

        public double VelocityTagsPerMSec { get; set; } = 0; // this one is set from UI, to remove from conveyor section in simulator cfg

        public int TagLengthInMm { get; set; } = 0; // this one is passed through api, set from UI, to remove from conveyor section in simulator cfg

        public int DistanceBetweenTagsInMm { get; set; } = 0; // this one is passed through api, set from UI, to remove from conveyor section in simulator cfg

        /// <summary>
        /// Count of tags for single test
        /// </summary>
        public int TestTagsNumber { get; set; } = 0; // taken from UI

        /// <summary>
        /// We're preparing commands ahead of time on c# layer. This is time period for which cammands being prepared
        /// this one is taken from simulator cfg
        /// </summary>
        private int? _predicatedConveyorMoveTimeMs = null; // 5000 by default
        public int PredicatedConveyorMoveTimeMs
        {
            get
            {
                if (!_predicatedConveyorMoveTimeMs.HasValue)
                {
                    //var simulatorSettings =  (SimulatorSettingsSection)ConfigurationManager.GetSection("simulatorSettings");
                    //_predicatedConveyorMoveTimeMs = simulatorSettings.ThreadSleepMs;
                    _predicatedConveyorMoveTimeMs = int.Parse(ConfigurationManager.AppSettings["ThreadSleepMs"]);
                }
                return _predicatedConveyorMoveTimeMs.Value;
            }
        }
    }
}
