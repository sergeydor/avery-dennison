using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Infrastructure.ErrorHandling.Enums
{
    [Serializable]
    public enum ErrorCode
    {
        UNDEFINED_ERROR = 0,

        [Description("Entity not found")]
        NOT_FOUND = 1,

        [Description("Input parameter's value is not valid.")]
        WRONG_INPUT_PARAMETER = 2,

        [Description("Device not found.")]
        DEVICE_NOT_FOUND = 3,

        [Description("Timeout error. Cannot get response from device.")]
        DEVICE_WAITCMDRESULT_TIMEOUT_ERROR = 4,

        [Description("Lane number must be 0-based.")]
        LANE_NUM_START_INDEX_WRONG = 5,

        [Description("Lane numbers sequence is wrong.")]
        LANE_SEQUENCE_WRONG = 6,

        [Description("Reader's Lane number must be unique.")]
        LANE_MUSTBE_UNIQUE = 7,

        [Description("Data received from device have errors. Default values will be displayed.")]
        PROCESSING_RECEIVED_DATA_ERROR = 8,

        [Description("Avery.Svc TimeoutException occured. The application may occur in inconsistent state. It's highly recomended to re-start both Avery services and UI application.")]
        SVC_TIMEOUT_EXCEPTION = 9,

        [Description("Cannot get device config from Emulator. Try to re-install DSF.")]
        SIM_OBTAINING_DEVICECFG_ERROR = 10,

        [Description("Processing Reset command (0x02) error.")]
        SVC_RESET_COMMAND_ERROR = 11,

        [Description("Service call result is null. Check connection to Avery.Svc. Try to restart application.")]
        SVC_CALLSVC_RESULT_ISNULL = 12,

        [Description("An error occured while stopping test in emulator")]
        SIM_STOP_TEST_ERROR = 13,

        [Description("An error occured suring the check of mongodb service status")]
        SVC_CHECK_MONGOSVC_ERROR = 14


        //DEVICE_SVC_SENDDATA_DECLINED = 3,

        // TODO - define range of errors for different layers: server, simulator, ui...
    }
}
