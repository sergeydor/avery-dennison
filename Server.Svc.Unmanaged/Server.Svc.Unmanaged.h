// Server.Svc.Unmanaged.h

#pragma once

#include <iostream>
#include <string>  
#include <vector>
#include <queue>
#include <list>
#include <map>
#include "UMConveyorShot.h"
#include <set>



//forward declaration
namespace std
{
	class mutex;
	class thread;	
}

template <class T, class Container = std::list<T>>
class SafeQueue;


namespace ServerSvcUnmanaged {
	
	class DeviceManager
	{
	public:		
		DeviceManager();
		virtual ~DeviceManager();
				
		bool Initialize(const DeviceConfig & inConfig);
		void InitializeStep2(const std::vector<DeviceInfo>& inDevInfo);
		bool CheckDevicesInitStatus();
		void SetConveyorSettings(const ConveyorSettings& inSettings);
		std::vector<DeviceInfo>& GetDeviceListInfo() { return mDevices; }
		bool SendCommandToDevice(const std::string& inDeviceMACAddress, const DeviceCommand& inCommand);
		std::vector<std::string> GetErrorList() { return std::move(mErrorList); };
		
		std::vector<DeviceCommand> GetOutgoingDataByDevice(const std::string& inDeviceMACAddress);
		std::vector<DeviceCommand> GetIncommingDataByDevice(const std::string& inDeviceMACAddress);
		std::vector<ConveyorShot> GetConveyorShots();

		void Start(); // Start device listening
		void Stop(); // Stop device listening
		void StopTest(); // Stop test, clean all queues
		void StartTest(); // Start test, reset mTestStartTime and timersCount
		void ResetState(); // remove all devices, clean all queues

		std::vector<ConveyorShot> GenerateSnapshotsForMashalingTest();

	private:		
		void Write(hid_device* inHandle, const unsigned char* inWriteBuf);
		void Mark(int lane);
		void Punch(int lane);
		bool Read(hid_device* inHandle, unsigned char* ourBuf, unsigned int inTimeOutMilliseconds);
		void Clean();
		bool GetDeviceMACAddress(hid_device* inHandle, std::string& outMACAddress);
		hid_device* FindDevice(const std::string inMACAddress);
		bool Process90Responce(const DeviceInfo& devInfo, const DeviceCommand& cmd);
		bool ProcessE3Responce(const DeviceInfo& devInfo, const DeviceCommand& cmd);
		bool ProcessResponce(const DeviceInfo& devInfo, const DeviceCommand& cmd);
		void HandleErrorCallstack(const std::string& funcName);
		void LogError(const std::string& errorText);
		void ListenDevicesCommands();
		CDateTime GetProcessingTime(long long deviceTs, long long& prevProcessingTime);
		void ScheduleMarkAndPunch(const DeviceInfo& devInfo, const TagColumn* clmn, long long inProcTime);
		void CheckMarkPunchQueue();

	private:
		std::vector<DeviceInfo> mDevices;
		std::vector<std::string> mErrorList;
		ConveyorSettings mConveyorSettings;
		DeviceConfig mDeviceConfig;
		std::unique_ptr<SafeQueue<ConveyorShot>> mConveyorQueue;
		std::multiset<FailedTagCommand> mMarkPunchDataQueue;
		std::unique_ptr<std::mutex> mMarkPunchMutex = nullptr;

		std::map<std::string, std::unique_ptr<SafeQueue<DeviceCommand>>> mOutgoingMsgs;
		std::map<std::string, std::unique_ptr<SafeQueue<DeviceCommand>>> mIncomingMsgs;

		std::unique_ptr<std::thread> mDeviceListenerThread = nullptr;

		volatile bool mDeviceListenerLoop = true;

		long mTimerCircleCount = 0;
		bool mTestIsRunning = false;
		//std::vector<int> cmds;
		std::vector<long long> times;
		std::vector<std::pair<int, long long>> times_p;
		CDateTime mTestStartTime;
		ConveyorShot mCurrentShot;
	};

}
