using System;
using System.Linq;
using Common.Domain.DeviceResults;
using Common.Enums;

namespace Server.Device.Communication.CommandInterpretators
{
	public abstract class BaseInterpretator
	{
		// Convert[DomainName=CommandName]ToCommandData
		// ConvertResponseDataTo[DomainName=CommandName]
		// Get[CommandName]CommandData

		public virtual GeneralDeviceResult ConvertResponseDataToGeneralDeviceResult(ResponseData responseData)
		{
			var result = new GeneralDeviceResult();
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

        public virtual EntityDeviceResult<TEntity> ConvertResponseDataToEntityDeviceResult<TEntity>(ResponseData responseData)
        {
            var result = new EntityDeviceResult<TEntity>();
            CreateGeneralDeviceResult(result, responseData);
            return result;
        }

        protected void CalculateFcs(CommandData commandData)
		{
			commandData.FCS = (byte)(commandData.LEN + commandData.CMD + (commandData.DATA?.Sum(x => x) ?? 0x0));
		}

		protected GeneralDeviceResult CreateGeneralDeviceResult(GeneralDeviceResult result, ResponseData responseData)
		{
			result.Status = (StatusCode) responseData.STATUS;
			result.Timer = BitConverter.ToUInt16(new[] {responseData.TMR_HI, responseData.TMR_LO}, 0);

			return result;
		}

        protected T[] Normalize<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            Array.Reverse(result);
            return result;
        }
    }
}