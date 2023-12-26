#pragma once
#include "DateTime.h"
#include "hidapi.h"

namespace ServerSvcUnmanaged {

	enum class DeviceType
	{
		GPIO = 1,
		Reader = 2
	};

	enum class ExecutionTypeBy
	{
		ByHiLoTime = 1,
		ByReceivedTime = 2
	};

	enum class ExecutionTypeFrom
	{
		FromEncoderTimeStamp = 1,
		FromReaderTimeStamp = 2
	};

	struct DeviceInfo
	{
		DeviceType mType;
		std::string mMACAddress;
		hid_device* mHandle;
		unsigned char mLane = -1;
	};

	struct DeviceConfig
	{
		unsigned int vendorId = 0x04d8;
		unsigned int readerProductId = 0x003F;
		unsigned int GPIOProductId = 0x0040;
		unsigned char readersCount = 1;
	};

	struct ConveyorSettings
	{
		unsigned int tagLengthInMm = 24;
		unsigned int distanceBetweenTagsInMm = 12;
		unsigned int tagsCountPerOneLane = 100;
		unsigned int snapshotIntervalMs = 500;
		unsigned int tagsCountForVelocityMeasurement = 20;
		std::map<std::string, unsigned int> encoderReaderTagsDistance;
		std::map<std::string, unsigned int> readerMarkerTagsDistance;
		std::map<std::string, unsigned int> markerPuncherTagsDistance;
		std::map<unsigned int, unsigned int> markerDelay;
		std::map<unsigned int, unsigned int> puncherDelay;
		long long    timerCicleLengthMs = 1 << 16;
		ExecutionTypeBy exType = ExecutionTypeBy::ByHiLoTime;
		ExecutionTypeFrom exTypeFrom = ExecutionTypeFrom::FromReaderTimeStamp;
	};

	struct DeviceCommand
	{
		unsigned char data[64];
		CDateTime receivedDate;
	};

	struct FailedTagCommand
	{
		unsigned int lane = -1;
		CDateTime    timeToExecute;
		unsigned int commandsToExecute = -1;
		enum class ExecutionType
		{
			mark,
			punch
		} type = ExecutionType::mark;

		bool operator < (const FailedTagCommand& rval) const { return timeToExecute.GetMilliseconds() < rval.timeToExecute.GetMilliseconds(); }
	};

	struct EncoderHexData
	{
		/// <summary>
		/// Contains the count of tags that have passed the sensor since entering high speed test mode. 
		/// AN: tags counter, we should to investigate what datatype to use - for now uint
		/// </summary>
		unsigned int sensor0Count;

		/// <summary>
		/// Contains the encoder A count which increments based on encoder/line speed. 
		/// AN: offset in mm, we should to investigate what datatype to use - for now double
		/// </summary>
		unsigned int encoderACount;

		/// <summary>
		/// Device timestamp, let's use TimeSpan for now
		/// </summary>
		long long tmrHiLo;
	};
}