using System;
using Common.Domain.DeviceResults.UnsolicitedReplyCommands;
using Common.Enums;
using Common.Enums.UnsolicitedReplyCommands;

namespace Server.Device.Communication.CommandInterpretators
{
	public class UnsolicitedReplyCommandsInterpretator : BaseInterpretator
	{
		/// <summary>
		/// Fault (0xE0) response
		/// </summary>
		public FaultResult ConvertResponseToFaultResult(ResponseData responseData)
		{
			var result = new FaultResult { FaultID = (FaultCode)responseData.DATA[0] };
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Message (0xE1) command
		/// </summary>
		public MessageResult ConvertResponseDataToMessageResult(ResponseData responseData)
		{
			var result = new MessageResult {MessageID = responseData.DATA[0]};
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Send RF Data (0xE2) response
		/// </summary>
		public SendRFDataResult ConvertResponseDataToSendRFDataResult(ResponseData responseData)
		{
			var data = responseData.DATA;
			ushort? ifad = null;
			ushort? irad = null;
			if (data != null)
			{
				ifad = BitConverter.ToUInt16(data, 0);
				irad = BitConverter.ToUInt16(data, 2);
			}

			var result = new SendRFDataResult();
			CreateGeneralDeviceResult(result, responseData);
			result.IFAD = ifad ?? 0x0;
			result.IRAD = irad ?? 0x0;

			return result;
		}

		/// <summary>
		/// Test (0xE3) command
		/// </summary>
		public CommandData GetTestE3CommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0xE3,
				FCS = 0x52
			};

			return commandData;
		}

		/// <summary>
		/// Test (0xE3) response
		/// </summary>
		public TestE3Result ConvertResponseDataToTestE3Result(ResponseData responseData)
		{
			var result = new TestE3Result();
			CreateGeneralDeviceResult(result, responseData);

			var data = responseData.DATA;
			if (data == null)
			{
				return result;
			}

			result.TestType = (TestE3Type)data[0];
			result.TestStatus = (TestE3Status)data[1];
			result.TestCount = BitConverter.ToUInt32(Normalize(data, 2, 4), 0);

			var arrayLength = 0xC;

			var epcTagID = new byte[arrayLength];
			Array.Copy(data, 6, epcTagID, 0, arrayLength);
			result.EPCTagID = epcTagID;

			result.Sensitivity = BitConverter.ToUInt16(Normalize(data, 18, 2), 0);

			var tid = new byte[arrayLength];
			Array.Copy(data, 20, tid, 0, arrayLength);
			result.TID = tid;

			return result;
		}

		/// <summary>
		/// Results of Sensitivity Test (0xE4) response
		/// </summary>
		public SensitivityTestResult ConvertResponseDataToSensitivityTestResult(ResponseData responseData)
		{
			var result = new SensitivityTestResult();
			CreateGeneralDeviceResult(result, responseData);
			result.PassState = (PassState)responseData.DATA[0];
			result.Power = BitConverter.ToInt16(responseData.DATA, 1);

			return result;
		}

		/// <summary>
		/// Event (0xE5) response
		/// </summary>
		public EventResult ConvertResponseDataToEventResult(ResponseData responseData)
		{
			var result = new EventResult
			{
				EventId = (EventID)responseData.DATA[0],
				TBD = new byte[0x4]
			};
			Array.Copy(responseData.DATA, 1, result.TBD, 0, 0x4);
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}
	}
}