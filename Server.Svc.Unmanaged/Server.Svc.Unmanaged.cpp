// This is the main DLL file.

#include "stdafx.h"
#include "Server.Svc.Unmanaged.h"
#include "SafeQueue.hpp"
#include "Callstack.h"
#include <iostream>
#include <fstream>
#include <thread>
#include <Windows.h>
#include <assert.h>
#include <future>
#include <mutex>
#include <ctime>

using namespace ServerSvcUnmanaged;
using namespace std;

//#define ENABLE_LOGGING

namespace
{
	const unsigned char kMessageSize = 65;
	const int kTagLengthInMm = 40; //4cm
	const int kDistanceBetweenTagsInMm = 20;
	const int kTestTagsNumber = 500;
}

DeviceManager::DeviceManager() : mDeviceListenerThread(new std::thread())
, mConveyorQueue(new SafeQueue<ConveyorShot>())
, mMarkPunchMutex(new std::mutex())
{
	mErrorList.reserve(1000);
}

DeviceManager::~DeviceManager()
{
	Clean();
	hid_exit();
};

void DeviceManager::Clean()
{
	try
	{
		for (auto &device : mDevices)
		{
			if (device.mHandle)
				hid_close(device.mHandle);
		}
	}
	catch (...)
	{
		HandleErrorCallstack("DeviceManager::Clean");
	}
}

bool DeviceManager::Initialize(const DeviceConfig& inConfig)
{
	try
	{
		int result = hid_init();
		if (result == -1)
		{
#ifdef ENABLE_LOGGING
			printf("Failed to init hid lib\n");
#endif
			HandleErrorCallstack("DeviceManager::Initialize: Failed to init hid lib");
			return false;
		}
		mDeviceConfig = inConfig;

		/*hid_device* handle = nullptr;
		DeviceInfo devInfo;
		struct hid_device_info *devs, *cur_dev;

		devs = hid_enumerate(inConfig.vendorId, inConfig.readerProductId);
		cur_dev = devs;
		unsigned char readers_count = 0;
		while (cur_dev) {

#ifdef ENABLE_LOGGING
			printf("Device Found\n  type: %04hx %04hx\n  path: %s\n  serial_number: %ls", cur_dev->vendor_id, cur_dev->product_id, cur_dev->path, cur_dev->serial_number);
			printf("\n");
			printf("  Manufacturer: %ls\n", cur_dev->manufacturer_string);
			printf("  Product:      %ls\n", cur_dev->product_string);
			printf("  Release:      %hx\n", cur_dev->release_number);
			printf("  Interface:    %d\n", cur_dev->interface_number);
			printf("\n");
#endif

			devInfo.mType = DeviceType::Reader;
			if (handle = hid_open_path(cur_dev->path))
			{
				devInfo.mHandle = handle;
				if (GetDeviceMACAddress(handle, devInfo.mMACAddress))
				{
					mOutgoingMsgs.emplace(devInfo.mMACAddress, std::unique_ptr<SafeQueue<DeviceCommand>>(new SafeQueue<DeviceCommand>()));
					mIncomingMsgs.emplace(devInfo.mMACAddress, std::unique_ptr<SafeQueue<DeviceCommand>>(new SafeQueue<DeviceCommand>()));
					mDevices.push_back(devInfo);
					readers_count++;
				}
#ifdef ENABLE_LOGGING
				else
					printf("Can't get MAC address\n");
#endif
			}
#ifdef ENABLE_LOGGING
			else
			{
				printf("Can't open the device\n");
				printf("Error: %ls\n", hid_error(handle));
			}
#endif
			cur_dev = cur_dev->next;
		}
		hid_free_enumeration(devs);

		devs = hid_enumerate(inConfig.vendorId, inConfig.GPIOProductId);
		cur_dev = devs;

		while (cur_dev) {

#ifdef ENABLE_LOGGING
			printf("Device Found\n  type: %04hx %04hx\n  path: %s\n  serial_number: %ls", cur_dev->vendor_id, cur_dev->product_id, cur_dev->path, cur_dev->serial_number);
			printf("\n");
			printf("  Manufacturer: %ls\n", cur_dev->manufacturer_string);
			printf("  Product:      %ls\n", cur_dev->product_string);
			printf("  Release:      %hx\n", cur_dev->release_number);
			printf("  Interface:    %d\n", cur_dev->interface_number);
			printf("\n");
#endif

			devInfo.mType = DeviceType::GPIO;
			if (handle = hid_open_path(cur_dev->path))
			{
				devInfo.mHandle = handle;
				if (GetDeviceMACAddress(handle, devInfo.mMACAddress))
				{
					mOutgoingMsgs.emplace(devInfo.mMACAddress, std::unique_ptr<SafeQueue<DeviceCommand>>(new SafeQueue<DeviceCommand>()));
					mIncomingMsgs.emplace(devInfo.mMACAddress, std::unique_ptr<SafeQueue<DeviceCommand>>(new SafeQueue<DeviceCommand>()));
					mDevices.push_back(devInfo);
					readers_count++;
				}
#ifdef ENABLE_LOGGING
				else
					printf("Can't get MAC address\n");
#endif
			}
#ifdef ENABLE_LOGGING
			else
			{
				printf("Can't open the device\n");
				printf("Error: %ls\n", hid_error(handle));
			}
#endif
			cur_dev = cur_dev->next;
		}
		hid_free_enumeration(devs);

#ifdef ENABLE_LOGGING
		if (readers_count != inConfig.readersCount)
			printf("Readers count not equal to config count\n");
#endif
*/
	}
	catch (...)
	{		
		printf("An Unexpected Error Occurred in DeviceManager::Initialize\n");
		HandleErrorCallstack("DeviceManager::Initialize");
		return false;
	}
	return true;
	//hid_set_nonblocking(handle, 1);
}

bool DeviceManager::CheckDevicesInitStatus()
{
	try
	{
		hid_device* handle = nullptr;
		DeviceInfo devInfo;
		struct hid_device_info *devs, *cur_dev;

		devs = hid_enumerate(mDeviceConfig.vendorId, mDeviceConfig.readerProductId);
		cur_dev = devs;
		while (cur_dev) {

#ifdef ENABLE_LOGGING
			printf("Device Found\n  type: %04hx %04hx\n  path: %s\n  serial_number: %ls", cur_dev->vendor_id, cur_dev->product_id, cur_dev->path, cur_dev->serial_number);
			printf("\n");
			printf("  Manufacturer: %ls\n", cur_dev->manufacturer_string);
			printf("  Product:      %ls\n", cur_dev->product_string);
			printf("  Release:      %hx\n", cur_dev->release_number);
			printf("  Interface:    %d\n", cur_dev->interface_number);
			printf("\n");
#endif

			devInfo.mType = DeviceType::Reader;
			if (handle = hid_open_path(cur_dev->path))
			{
				devInfo.mHandle = handle;
				if (GetDeviceMACAddress(handle, devInfo.mMACAddress))
				{
					if (mOutgoingMsgs.find(devInfo.mMACAddress) == mOutgoingMsgs.end())
					{
						mOutgoingMsgs.emplace(devInfo.mMACAddress, std::unique_ptr<SafeQueue<DeviceCommand>>(new SafeQueue<DeviceCommand>()));
						mIncomingMsgs.emplace(devInfo.mMACAddress, std::unique_ptr<SafeQueue<DeviceCommand>>(new SafeQueue<DeviceCommand>()));
						mDevices.push_back(devInfo);
					}
				}
#ifdef ENABLE_LOGGING
				else
				{
					printf("Can't get MAC address\n");
					printf("Error: %ls\n", hid_error(handle));
				}
#endif
			}
#ifdef ENABLE_LOGGING
			else
			{
				printf("Can't open the device\n");
			}
#endif
			cur_dev = cur_dev->next;
		}
		hid_free_enumeration(devs);

		devs = hid_enumerate(mDeviceConfig.vendorId, mDeviceConfig.GPIOProductId);
		cur_dev = devs;

		while (cur_dev) {

#ifdef ENABLE_LOGGING
			printf("Device Found\n  type: %04hx %04hx\n  path: %s\n  serial_number: %ls", cur_dev->vendor_id, cur_dev->product_id, cur_dev->path, cur_dev->serial_number);
			printf("\n");
			printf("  Manufacturer: %ls\n", cur_dev->manufacturer_string);
			printf("  Product:      %ls\n", cur_dev->product_string);
			printf("  Release:      %hx\n", cur_dev->release_number);
			printf("  Interface:    %d\n", cur_dev->interface_number);
			printf("\n");
#endif

			devInfo.mType = DeviceType::GPIO;
			if (handle = hid_open_path(cur_dev->path))
			{
				devInfo.mHandle = handle;
				if (GetDeviceMACAddress(handle, devInfo.mMACAddress))
				{
					if (mOutgoingMsgs.find(devInfo.mMACAddress) == mOutgoingMsgs.end())
					{
						mOutgoingMsgs.emplace(devInfo.mMACAddress, std::unique_ptr<SafeQueue<DeviceCommand>>(new SafeQueue<DeviceCommand>()));
						mIncomingMsgs.emplace(devInfo.mMACAddress, std::unique_ptr<SafeQueue<DeviceCommand>>(new SafeQueue<DeviceCommand>()));
						mDevices.push_back(devInfo);
					}
				}
#ifdef ENABLE_LOGGING
				else
					printf("Can't get MAC address\n");
#endif
			}
#ifdef ENABLE_LOGGING
			else
			{
				printf("Can't open the device\n");
				printf("Error: %ls\n", hid_error(handle));
			}
#endif
			cur_dev = cur_dev->next;
		}
		hid_free_enumeration(devs);
	}
	catch (...)
	{
		printf("An Unexpected Error Occurred in DeviceManager::CheckDevicesInitStatus\n");
		HandleErrorCallstack("DeviceManager::CheckDevicesInitStatus");
		return false;
	}
	return (mDevices.size() == mDeviceConfig.readersCount + 1);
}

void DeviceManager::InitializeStep2(const std::vector<DeviceInfo>& inDevInfo)
{
	try
	{
		for (auto it = mDevices.begin(); it != mDevices.end(); ++it)
		{
			for (auto& devSettings : inDevInfo)
			{
				if (it->mMACAddress == devSettings.mMACAddress)
				{
					it->mLane = devSettings.mLane;
					break;
				}
			}
			if (it->mLane == -1)
			{
				hid_close(it->mHandle);
				it = mDevices.erase(it);
				if (it == mDevices.end())
					return;
			}
		}
	}
	catch (...)
	{
		printf("An Unexpected Error Occurred in DeviceManager::InitializeStep2\n");
		HandleErrorCallstack("DeviceManager::InitializeStep2");
	}
}
void DeviceManager::LogError(const std::string& errorText)
{
	time_t now = time(0);
	tm* localtm = localtime(&now);
	std::string errorStr(asctime(localtm));
	string logFileName = to_string(localtm->tm_year + 1990) + '-' + to_string(localtm->tm_mon + 1) + '-' + to_string(localtm->tm_mday) + ".txt";

	if (CreateDirectory(L"Logs", NULL) ||
		ERROR_ALREADY_EXISTS == GetLastError())
	{
		if (CreateDirectory(L"Logs\\UnmgLogs", NULL) ||
			ERROR_ALREADY_EXISTS == GetLastError())
		{
			logFileName = "Logs\\UnmgLogs\\" + logFileName;
		}
	}

	errorStr += errorText + "\n\n";


	mErrorList.push_back(errorStr);

	std::ofstream logFile;
	logFile.open(logFileName, ios::out | ios::app | ios::binary);
	logFile << errorStr;
	logFile.close();

	cout << errorStr;
}

void DeviceManager::HandleErrorCallstack(const std::string& funcName)
{
	time_t now = time(0);
	tm* localtm = localtime(&now);
	std::string errorStr(asctime(localtm));
	string logFileName = to_string(localtm->tm_year + 1990) + '-' + to_string(localtm->tm_mon + 1) + '-' + to_string(localtm->tm_mday) + ".txt";

	if (CreateDirectory(L"Logs", NULL) ||
		ERROR_ALREADY_EXISTS == GetLastError())
	{
		if (CreateDirectory(L"Logs\\UnmgLogs", NULL) ||
			ERROR_ALREADY_EXISTS == GetLastError())
		{
			logFileName = "Logs\\UnmgLogs\\" + logFileName;
		}
	}

#ifndef NDEBUG
	Callstack cs(0, 7);
	CallstackFormat csFormatWide(cs, false, 1, 6);

	std::string str(csFormatWide);

	errorStr += "Error callstack : \n" + str + "\n\n";

#else
	errorStr += "An Unexpected Error Occurred in " + funcName + "\n\n";
#endif

	mErrorList.push_back(errorStr);

	std::ofstream logFile;
	logFile.open(logFileName, ios::out | ios::app | ios::binary);
	logFile << errorStr;
	logFile.close();

	//cout << errorStr;
}

void DeviceManager::ResetState()
{
	try
	{
		Stop();
		Clean();
		mDevices.clear();
		mConveyorQueue->clear();

		mOutgoingMsgs.clear();
		mIncomingMsgs.clear();
		mErrorList.clear();

		/*std::ofstream myfile;
		myfile.open("2.txt", ios::out | ios::app | ios::binary);

		std::ofstream myfile1;
		myfile1.open("3.txt", ios::out | ios::app | ios::binary);

		for (size_t i = 0; i < times.size(); ++i)
		{
			myfile << times[i] << std::endl;

			using namespace std::chrono;
			milliseconds ms(times[i]);
			seconds s = duration_cast<seconds>(ms);
			time_t t = s.count();
		}

		for (size_t i = 0; i < times_p.size(); ++i)
		{

			using namespace std::chrono;
			milliseconds ms(times_p[i].second);
			seconds s = duration_cast<seconds>(ms);
			time_t t = s.count();
			tm* timeinfo = gmtime(&t);
			myfile1 << times_p[i].first << " " << timeinfo->tm_hour << "::" << timeinfo->tm_min << "::" << timeinfo->tm_sec << "::" << ms.count() % 1000 << std::endl;
		}
		myfile.close();
		myfile1.close();*/
	}
	catch (...)
	{
		printf("An Unexpected Error Occurred in DeviceManager::ResetState\n");
		HandleErrorCallstack("DeviceManager::ResetState");
	}
}

void DeviceManager::Mark(int lane)
{
	for (auto dev : mDevices)
	{
		if (dev.mType == DeviceType::GPIO)
		{
			unsigned char buf[kMessageSize];
			memset(buf, 0, sizeof(buf));
			buf[1] = 0xee;
			buf[3] = 0xA0;
			buf[4] = lane;
			buf[5] = 0x03;//TODO: we could play with this value and time to start marking
			buf[6] = buf[3] + buf[4] + buf[5];
			Write(dev.mHandle, buf);
			return;
		}
	}
}

void DeviceManager::Punch(int lane)
{
	for (auto dev : mDevices)
	{
		if (dev.mType == DeviceType::GPIO)
		{
			unsigned char buf[kMessageSize];
			memset(buf, 0, sizeof(buf));
			buf[1] = 0xee;
			buf[3] = 0xA1;
			buf[4] = lane;
			buf[5] = 0x03;//TODO: we could play with this value and time to start marking
			buf[6] = buf[3] + buf[4] + buf[5];
			Write(dev.mHandle, buf);
			return;
		}
	}
}


void DeviceManager::Write(hid_device* inHandle, const unsigned char* inWriteBuf)
{
	try
	{
		unsigned char buf[kMessageSize];
		memset(buf, 0, sizeof(buf));
		memcpy(buf + 1, inWriteBuf, kMessageSize - 1);
		int result = hid_write(inHandle, buf, kMessageSize);

		if (result < 0)
			HandleErrorCallstack("DeviceManager::Write: Unable to write");

#ifdef ENABLE_LOGGING
		if (result < 0) {
			printf("Unable to write()\n");
			printf("Error: %ls\n", hid_error(inHandle));
		}
		else
		{
			printf("Data written:\n   ");
			// Print out the returned buffer.
			for (int i = 0; i < result; i++)
				printf("%02hhx ", buf[i]);
			printf("\n");
		}
#endif
	}
	catch (...)
	{
		printf("An Unexpected Error Occurred in DeviceManager::Write\n");
		HandleErrorCallstack("DeviceManager::Write");
	}
}

bool DeviceManager::Read(hid_device* inHandle, unsigned char* outBuf, unsigned int inTimeOutMilliseconds)
{
	try
	{
		memset(outBuf, 0, kMessageSize - 1);
		int result = hid_read_timeout(inHandle, outBuf, kMessageSize, inTimeOutMilliseconds);

		if (result < 0)
			HandleErrorCallstack("DeviceManager::Read: Unable to read");
#ifdef ENABLE_LOGGING
		if (result < 0)
			printf("Unable to read()\n");
		if (result > 0)
		{

			printf("Data read:\n   ");
			// Print out the returned buffer.
			for (int i = 0; i < result; i++)
				printf("%02hhx ", outBuf[i]);
			printf("\n");
		}
#endif

		return result > 0;
	}
	catch (...)
	{
		printf("An Unexpected Error Occurred in DeviceManager::Read\n");
		HandleErrorCallstack("DeviceManager::Read");
		return false;
	}
}

bool DeviceManager::GetDeviceMACAddress(hid_device* inHandle, std::string& outMACAddress)
{
	try
	{
		unsigned char macAddressRequestCommand[] = { 0xEE, 0x01, 0x71, 0x06, 0x78 };
		Write(inHandle, macAddressRequestCommand);
		unsigned char macAddressResponceBuffer[kMessageSize];
		if (Read(inHandle, macAddressResponceBuffer, 300))
		{
			char mac[30];
			memset(mac, 0, sizeof(mac));
			sprintf(mac, "%02hhx-%02hhx-%02hhx-%02hhx-%02hhx-%02hhx", macAddressResponceBuffer[7], macAddressResponceBuffer[8], macAddressResponceBuffer[9], macAddressResponceBuffer[10], macAddressResponceBuffer[11], macAddressResponceBuffer[12]);
			outMACAddress = std::string(mac);

#ifdef ENABLE_LOGGING
			printf("Device MAC address is: %s\n", outMACAddress.c_str());
#endif

			return true;
		}
		return false;
	}
	catch (...)
	{
		printf("An Unexpected Error Occurred in DeviceManager::GetDeviceMACAddress\n");
		HandleErrorCallstack("DeviceManager::GetDeviceMACAddress");
		return false;
	}
}

hid_device*  DeviceManager::FindDevice(const std::string inMACAddress)
{
	for (const auto &device : mDevices)
	{
		if (inMACAddress.compare(device.mMACAddress) == 0)
			return device.mHandle;
	}
	return nullptr;
}

bool DeviceManager::SendCommandToDevice(const std::string& inDeviceMACAddress, const DeviceCommand& inCommand)
{
	try
	{
		if (auto handle = FindDevice(inDeviceMACAddress))
		{
			Write(handle, inCommand.data);//TODO" check write?
			DeviceCommand cmd = inCommand;
			cmd.receivedDate = CDateTime::Now();
			mOutgoingMsgs[inDeviceMACAddress]->push(cmd);
			return true;
		}
		return false;
	}
	catch (...)
	{
		printf("An Unexpected Error Occurred in DeviceManager::SendCommandToDevice\n");
		HandleErrorCallstack("DeviceManager::SendCommandToDevice");
		return false;
	}
}

void DeviceManager::SetConveyorSettings(const ConveyorSettings& inSettings)
{
	mConveyorSettings = inSettings;
}

std::vector<DeviceCommand> DeviceManager::GetOutgoingDataByDevice(const std::string& inDeviceMACAddress)
{
	std::vector<DeviceCommand> result;
	try
	{
		result.reserve(100000);
		auto devCommandsQueue = mOutgoingMsgs.find(inDeviceMACAddress);
		if (devCommandsQueue != mOutgoingMsgs.end())
		{
			SafeQueue<DeviceCommand> copyQueue;
			devCommandsQueue->second->swap(copyQueue);
			while (true)
			{
				DeviceCommand command;
				if (copyQueue.try_pop(command))
				{
					result.push_back(std::move(command));
				}
				else
					break;
			}
		}
		return std::move(result);
	}
	catch (...)
	{
		printf("An Unexpected Error Occurred in DeviceManager::GetOutgoingDataByDevice\n");
		HandleErrorCallstack("DeviceManager::GetOutgoingDataByDevice");
		return std::move(result);
	}
}

std::vector<DeviceCommand> DeviceManager::GetIncommingDataByDevice(const std::string& inDeviceMACAddress)
{
	std::vector<DeviceCommand> result;
	try
	{
		result.reserve(100000);
		auto devCommandsQueue = mIncomingMsgs.find(inDeviceMACAddress);
		if (devCommandsQueue != mIncomingMsgs.end())
		{
			SafeQueue<DeviceCommand> copyQueue;
			devCommandsQueue->second->swap(copyQueue);
			while (true)
			{
				DeviceCommand command;
				if (copyQueue.try_pop(command))
				{
					result.push_back(std::move(command));
				}
				else
					break;
			}
		}
		return result;
	}
	catch (...)
	{
		printf("An Unexpected Error Occurred in DeviceManager::GetIncommingDataByDevice\n");
		HandleErrorCallstack("DeviceManager::GetIncommingDataByDevice");
		return result;
	}
}

std::vector<ConveyorShot> DeviceManager::GetConveyorShots()
{
	std::vector<ConveyorShot> result;
	try
	{
		result.reserve(100000);
		SafeQueue<ConveyorShot> copyQueue;
		mConveyorQueue->swap(copyQueue);
		while (true)
		{
			ConveyorShot shot;
			if (copyQueue.try_pop(shot))
			{
				result.push_back(std::move(shot));
			}
			else
				break;
		}

		return result;
	}
	catch (...)
	{
		printf("An Unexpected Error Occurred in DeviceManager::GetConveyorShots\n");
		HandleErrorCallstack("DeviceManager::GetConveyorShots");
		return result;
	}
}

std::vector<ConveyorShot> DeviceManager::GenerateSnapshotsForMashalingTest()
{	
	std::vector<ConveyorShot> result;
	
	for (size_t i = 0; i < 100; ++i)
	{
		ConveyorShot shot;
		std::string name("name");
		name.append(std::to_string(i));
		shot.name = name;
		shot.totalLanes = i+1;
		shot.velocityInMMPerMSec = 3;

		for (size_t k = 0; k < 100; ++k)
		{
			TagColumn column;
			column.currentXPosition = i * 5;
			column.velocityBOnDistInMmPerMSec = 2;
			LinearEncoderData encData;
			encData.triggerNumber = 3;
			encData.velocityAvgInMMPerMSec = 1;

			for (size_t t = 0; t < 20; ++t)
			{
				LinearEncoderDataItem item(3.3, CDateTime::Now(), i, k, 4.4);
				encData.items.push_back(item);
			}
			
			column.linearEncoderData.reset(new LinearEncoderData(encData));

			RFIDTag tag;
			tag.id = i + k;
			tag.isVNATestPassed = (bool)(k % 2);
			tag.lineYIndex = 1;

			tag.rfidReaderData.reset(new DeviceCommand());
			/*tag.rfidReaderData->epc = "test";
			tag.rfidReaderData->lineYNumber = 1;
			tag.rfidReaderData->tid = 1;*/
			VNADataItem vnaItem;
			vnaItem.radioFrequency = 22;
			vnaItem.value = i*k;
			tag.vnaData.reset(new VNAData);
			tag.vnaData->items.push_back(vnaItem);
			column.tags[i] = tag;
			shot.tagsColumns.push_back(column);
		}	

		result.push_back(shot);
	}
	return result;
}

void DeviceManager::Start()
{
	try
	{
		mDeviceListenerLoop = true;
		mDeviceListenerThread->swap(std::thread([this]()
		{
			ListenDevicesCommands();
		}));
		
		SetThreadPriority(mDeviceListenerThread->native_handle(), THREAD_PRIORITY_TIME_CRITICAL);

		/*cmds.reserve(99999999);*/
		//times.reserve(99999999);
		//times_p.reserve(100000);
	}
	catch (...)
	{
		printf("An Unexpected Error Occurred in DeviceManager::Start\n");
		HandleErrorCallstack("DeviceManager::Start");
	}
}

void DeviceManager::Stop()
{
	try
	{
		mDeviceListenerLoop = false;
		mDeviceListenerThread->join();
	}
	catch (...)
	{
		printf("An Unexpected Error Occurred in DeviceManager::Stop\n");
		HandleErrorCallstack("DeviceManager::Stop");
	}
}

void DeviceManager::StartTest()
{
	mTestStartTime = CDateTime(0);
	mTimerCircleCount = 0;
	mCurrentShot.createDt = CDateTime::NowMilliseconds() + 1;
	mCurrentShot.planToCreateDt = CDateTime::NowMilliseconds() + 1;
	mTestIsRunning = true;
}

void DeviceManager::StopTest()
{
	mTestIsRunning = false;
	mConveyorQueue->clear();

	auto cleanMapFunc = [](std::map<std::string, std::unique_ptr<SafeQueue<DeviceCommand>>>& mapToClean)
	{
		for (auto it = mapToClean.begin(); it != mapToClean.end(); ++it)
		{
			it->second->clear();
		}
	};

	cleanMapFunc(mOutgoingMsgs);
	cleanMapFunc(mIncomingMsgs);

	{
		std::lock_guard<std::mutex> lock(*(mMarkPunchMutex.get()));
		mMarkPunchDataQueue.clear();
	}

	ConveyorShot empty;
	std::swap(empty, mCurrentShot);
}

bool DeviceManager::ProcessResponce(const DeviceInfo& devInfo, const DeviceCommand& cmd)
{
	try
	{
		if (cmd.data[00] != 0xee)
		{
			//throw - 1;
			return false;
		}

		unsigned char cmdId = cmd.data[2];

		if (cmdId == 0x90)
			return Process90Responce(devInfo, cmd);
		if (cmdId == 0xE3)
			return ProcessE3Responce(devInfo, cmd);
		return true;
	}
	catch (...)
	{
		printf("An Unexpected Error Occurred in DeviceManager::ProcessResponce\n");
		HandleErrorCallstack("DeviceManager::ProcessResponce");
		return false;
	}
}

bool DeviceManager::Process90Responce(const DeviceInfo& devInfo, const DeviceCommand& cmd)
{
	//times.push_back(CDateTime::NowMilliseconds());

	if (cmd.data[2] != 0x90)
		return false;
	if (cmd.data[3] != 0x55) //not streaming mode
		return true;
	EncoderHexData hex;

	hex.sensor0Count = cmd.data[6];
	hex.sensor0Count = (hex.sensor0Count << 8) + cmd.data[7];
	hex.sensor0Count = (hex.sensor0Count << 8) + cmd.data[8];
	hex.sensor0Count = (hex.sensor0Count << 8) + cmd.data[9];

	hex.encoderACount = cmd.data[10];
	hex.encoderACount = (hex.encoderACount << 8) + cmd.data[11];
	hex.encoderACount = (hex.encoderACount << 8) + cmd.data[12];
	hex.encoderACount = (hex.encoderACount << 8) + cmd.data[13];
	hex.tmrHiLo = (cmd.data[4] << 8) + cmd.data[5];

	//times_p.push_back(std::pair<int, long long>(hex.sensor0Count, CDateTime::NowMilliseconds()));

	mCurrentShot.totalLanes = mDevices.size() - 1;
	CDateTime processingTime;
	static long long prevProcessingTime = 0;

	if (mConveyorSettings.exType == ExecutionTypeBy::ByReceivedTime)
		processingTime = cmd.receivedDate;
	else
		processingTime = GetProcessingTime(hex.tmrHiLo, prevProcessingTime);

	long timeStamp = hex.tmrHiLo;
	if (mConveyorSettings.exType == ExecutionTypeBy::ByReceivedTime)
		timeStamp = (processingTime.GetMilliseconds() - mTestStartTime.GetMilliseconds()) % (1 << 16);
	LinearEncoderDataItem encoderItem(0, processingTime, timeStamp, hex.sensor0Count, hex.encoderACount);

	unsigned int triggerNoFromhex = hex.sensor0Count;
	int currentTriggerArrIndex = mCurrentShot.GetTagIndexByTriggerNo(triggerNoFromhex);

	bool newTagFound = currentTriggerArrIndex == -1; // not found in array, so it's a new tags column 
	if (newTagFound)
	{
		TagColumn column;

		column.linearEncoderData.reset(new LinearEncoderData());
		column.linearEncoderData->triggerNumber = triggerNoFromhex;
		column.velocityBOnDistInMmPerMSec = 0;
		mCurrentShot.tagsColumns.insert(mCurrentShot.tagsColumns.begin(), column);
	}

	mCurrentShot.ProcessNewEncoderItem(encoderItem, mConveyorSettings.distanceBetweenTagsInMm, mConveyorSettings.tagLengthInMm, mConveyorSettings.tagsCountForVelocityMeasurement);

	mCurrentShot.UpdateCurrentShotVelocity();

	if (newTagFound)
	{
		mCurrentShot.TrimLaneRight(mConveyorSettings.tagsCountPerOneLane);
	}
	
	if (auto cc = mCurrentShot.GetCurrentColumn())
	{
		//printf("Total offset\t%f \tCurrent tag %u\t \tShot velocity\t%g \tCur-Col Velocity\t%g \tcur.col.1st.enc.d.ts\t%u\n", mCurrentShot.totalLaneOffsetInMm, cc->linearEncoderData != nullptr ? cc->linearEncoderData->triggerNumber : -1, mCurrentShot.velocityInMMPerMSec, cc->velocityBOnDistInMmPerMSec, (cc->linearEncoderData != nullptr && cc->linearEncoderData->GetCurrentItem() != nullptr) ? cc->linearEncoderData->GetCurrentItem()->deviceTimeStamp : -1);
		/*std::ofstream myfile;
		myfile.open("log_c_plus_plus.txt", ios::out | ios::app | ios::binary);
		myfile << "Total offset " << currentShot.totalLaneOffsetInMm << "\nCurrent tag " << (cc->linearEncoderData != nullptr ? cc->linearEncoderData->triggerNumber : -1) << "\nShot velocity " << currentShot.velocityInMMPerMSec << "\nCur-Col Velocity " << cc->velocityBOnDistInMmPerMSec << "\ncur.col.1st.enc.d.ts " << ((cc->linearEncoderData != nullptr && cc->linearEncoderData->GetCurrentItem() != nullptr) ? cc->linearEncoderData->GetCurrentItem()->deviceTimeStamp : -1) << std::endl;
		myfile.close();*/
	}

	return true;
}

bool DeviceManager::ProcessE3Responce(const DeviceInfo& devInfo, const DeviceCommand& cmd)
{
	try
	{
		if (cmd.data[2] != 0xE3)
			return false;
		unsigned char testType = cmd.data[6];
		unsigned char testStatus = cmd.data[7];
		int testCount = cmd.data[8];
		long long tmrHiLo = (cmd.data[4] << 8) + cmd.data[5];

		testCount = (testCount << 8) + cmd.data[9];
		testCount = (testCount << 8) + cmd.data[10];
		testCount = (testCount << 8) + cmd.data[11];
		std::string epcTagID;
		epcTagID.assign(&cmd.data[12], &cmd.data[24]);
		std::string TID;
		TID.assign(&cmd.data[26], &cmd.data[38]);

		long long procTime = CDateTime::NowMilliseconds();
		//assuming that testCount is equal triggerId
		if (auto clmn = mCurrentShot.GetColumnTriggerNo(testCount))
		{
			bool testResult = (testStatus == 0);
			//TODO check test type?
			clmn->tags[devInfo.mLane].isVNATestPassed = testResult;
			clmn->tags[devInfo.mLane].rfidReaderData.reset(new DeviceCommand(cmd));
			if (!testResult)
			{
				if (mConveyorSettings.exType == ExecutionTypeBy::ByHiLoTime && mConveyorSettings.exTypeFrom == ExecutionTypeFrom::FromEncoderTimeStamp)
				{
					//Getting minimum between time based on HiLo and current time. But regarging acync commands need to check for different mTimerCircleCount
					long long tempProcTime = mTestStartTime.GetMilliseconds() + mConveyorSettings.timerCicleLengthMs * (mTimerCircleCount + 1) + tmrHiLo;
					if (procTime >= tempProcTime)
					{
						procTime = tempProcTime;
					}
					else
					{
						tempProcTime -= mConveyorSettings.timerCicleLengthMs;
						if (procTime >= tempProcTime)
						{
							procTime = tempProcTime;
						}
						else
						{
							tempProcTime -= mConveyorSettings.timerCicleLengthMs;
							if (procTime >= tempProcTime)
							{
								procTime = tempProcTime;
							}
						}
					}

					if (std::abs(procTime - CDateTime::NowMilliseconds()) > 100)
						procTime = CDateTime::NowMilliseconds();

				}
				ScheduleMarkAndPunch(devInfo, clmn, procTime);
			}
		}
		else
		{
			assert(0 && "ProcessE3Responce::Wrong data in responce");
		}
	}
	catch (...)
	{
		HandleErrorCallstack("DeviceManager::ProcessE3Responce");
		return false;
	}
	return true;
}

ServerSvcUnmanaged::CDateTime DeviceManager::GetProcessingTime(long long deviceTs, long long& prevProcessingTime)
{
	if (mTestStartTime.GetMilliseconds() == 0)
	{
		mTestStartTime = CDateTime(CDateTime::NowMilliseconds() - deviceTs);
		prevProcessingTime = CDateTime::NowMilliseconds();
		return CDateTime(prevProcessingTime);
	}

	long long procTime = mTestStartTime.GetMilliseconds() + mConveyorSettings.timerCicleLengthMs * mTimerCircleCount + deviceTs;
	if (prevProcessingTime > procTime)
	{
		mTimerCircleCount++;
		procTime += mConveyorSettings.timerCicleLengthMs;
	}
	mTestStartTime = CDateTime(std::min<long long>(mTestStartTime.GetMilliseconds(), (CDateTime::NowMilliseconds() - deviceTs - mConveyorSettings.timerCicleLengthMs * mTimerCircleCount)));

	procTime = mTestStartTime.GetMilliseconds() + mConveyorSettings.timerCicleLengthMs * mTimerCircleCount + deviceTs;;
	prevProcessingTime = procTime;
	return CDateTime(procTime);
}

void DeviceManager::ListenDevicesCommands()
{
	while (mDeviceListenerLoop)
	{
		try
		{
			for (auto& devInfo : mDevices)
			{
				DeviceCommand cmd;
				if (Read(devInfo.mHandle, cmd.data, 0))				{
					cmd.receivedDate = CDateTime::Now();
					if (ProcessResponce(devInfo, cmd))
					{
						mIncomingMsgs[devInfo.mMACAddress]->push(cmd);
					}
				}
			}
			CheckMarkPunchQueue();
			if (mTestIsRunning)
			{
				if (CDateTime::NowMilliseconds() - mCurrentShot.planToCreateDt >= 0)
				{					
					mConveyorQueue->push(mCurrentShot);
					mCurrentShot.createDt = mCurrentShot.planToCreateDt;
					mCurrentShot.planToCreateDt += mConveyorSettings.snapshotIntervalMs;
				}
			}
			Sleep(1);
		}
		catch (...)
		{
			printf("An Unexpected Error Occurred in DeviceManager::ListenDevicesCommands\n");
			HandleErrorCallstack("DeviceManager::ListenDevicesCommands");
			break;
		}
	}
}

void DeviceManager::ScheduleMarkAndPunch(const DeviceInfo& devInfo, const TagColumn* clmn, long long inProcTime)
{
	try
	{
		FailedTagCommand cmd;
		cmd.lane = devInfo.mLane;
		cmd.type = FailedTagCommand::ExecutionType::mark;
		if (!clmn->linearEncoderData || !clmn->linearEncoderData->HasItems())
			throw "Error in data";//TODO:: try catch
		auto curItem = clmn->linearEncoderData->GetCurrentItem();
		if (mConveyorSettings.exTypeFrom == ExecutionTypeFrom::FromEncoderTimeStamp)
			cmd.timeToExecute = CDateTime(curItem->processingMockTime.GetMilliseconds() + ((mConveyorSettings.readerMarkerTagsDistance[devInfo.mMACAddress] + mConveyorSettings.encoderReaderTagsDistance[devInfo.mMACAddress]) * (mConveyorSettings.distanceBetweenTagsInMm + mConveyorSettings.tagLengthInMm)) / mCurrentShot.GetCurrentColumn()->velocityBOnDistInMmPerMSec - mConveyorSettings.markerDelay[devInfo.mLane]/*use velocity from linearEncoderData?*/);
		else
			cmd.timeToExecute = CDateTime(inProcTime + (mConveyorSettings.readerMarkerTagsDistance[devInfo.mMACAddress] * (mConveyorSettings.distanceBetweenTagsInMm + mConveyorSettings.tagLengthInMm)) / mCurrentShot.GetCurrentColumn()->velocityBOnDistInMmPerMSec - mConveyorSettings.markerDelay[devInfo.mLane]/*use velocity from linearEncoderData?*/);
		//TODO: duration? xPosition
		std::lock_guard<std::mutex> lock(*(mMarkPunchMutex.get()));
		mMarkPunchDataQueue.insert(cmd);
		cmd.type = FailedTagCommand::ExecutionType::punch;
		if (mConveyorSettings.exTypeFrom == ExecutionTypeFrom::FromEncoderTimeStamp)
			cmd.timeToExecute = CDateTime(curItem->processingMockTime.GetMilliseconds() + ((mConveyorSettings.readerMarkerTagsDistance[devInfo.mMACAddress] + mConveyorSettings.encoderReaderTagsDistance[devInfo.mMACAddress] + mConveyorSettings.markerPuncherTagsDistance[devInfo.mMACAddress]) * (mConveyorSettings.distanceBetweenTagsInMm + mConveyorSettings.tagLengthInMm)) / mCurrentShot.GetCurrentColumn()->velocityBOnDistInMmPerMSec - mConveyorSettings.puncherDelay[devInfo.mLane]/*use another velocity?*/);
		else
			cmd.timeToExecute = CDateTime(inProcTime + ((mConveyorSettings.readerMarkerTagsDistance[devInfo.mMACAddress] + mConveyorSettings.markerPuncherTagsDistance[devInfo.mMACAddress]) * (mConveyorSettings.distanceBetweenTagsInMm + mConveyorSettings.tagLengthInMm) - curItem->xPosition) / mCurrentShot.GetCurrentColumn()->velocityBOnDistInMmPerMSec - mConveyorSettings.puncherDelay[devInfo.mLane]/*use another velocity?*/);

		mMarkPunchDataQueue.insert(cmd);
	}
	catch (...)
	{
		HandleErrorCallstack("DeviceManager::ScheduleMarkAndPunch");
	}
}

void DeviceManager::CheckMarkPunchQueue()
{
	static bool empty = true;
	static FailedTagCommand cmdToExecute;
	if (empty)
	{
		empty = mMarkPunchDataQueue.empty();
		if (!empty)
		{
			std::lock_guard<std::mutex> lock(*(mMarkPunchMutex.get()));
			auto it = mMarkPunchDataQueue.begin();
			cmdToExecute = *it;
			mMarkPunchDataQueue.erase(it);
		}
	}
	if (!empty)
	{
		if (cmdToExecute.timeToExecute.GetMilliseconds() <= CDateTime::NowMilliseconds() + 1)
		{
			FailedTagCommand cmd = cmdToExecute;
			std::async([this, cmd]()
			{
				if (cmd.type == FailedTagCommand::ExecutionType::mark)
					Mark(cmd.lane);
				else
					Punch(cmd.lane);
			});
			empty = true;
			CheckMarkPunchQueue();
		}
	}
}
