#pragma once

#include "DateTime.h"

namespace ServerSvcUnmanaged {

	struct LinearEncoderDataItem
	{
		LinearEncoderDataItem() = default;
		LinearEncoderDataItem(double inVelocityInMMPerMSec, CDateTime inProcessingTime, long inDeviceTimeStamp, unsigned int inTriggerNumber, double inXPosition)
		{
			xPosition = inXPosition;
			triggerNumber = inTriggerNumber;
			deviceTimeStamp = inDeviceTimeStamp;
			velocityInMMPerMSec = inVelocityInMMPerMSec;
			processingMockTime = inProcessingTime;
		};

		/// <summary>
		/// It should be based on device timestamp and processing time. Something average
		/// </summary>
		CDateTime processingMockTime;

			/// <summary>
			/// This is cyclic device time
			/// </summary>
		long deviceTimeStamp = 0;

			/// <summary>
			/// Tag's number on the conveyor, identified by encoder
			/// </summary>
		unsigned int triggerNumber = 0;

			/// <summary>
			/// The position of identified tag
			/// </summary>
		double xPosition = 0;

		double velocityInMMPerMSec = 0;
	};
}
