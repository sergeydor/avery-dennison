#pragma once

#include <iostream>
#include <string>  
#include <vector>
#include <msclr\marshal_cppstd.h>
//#include "hidapi.h"
#include "UMConveyorShot.h"
#include "Server.Svc.Unmanaged.h"


using namespace System;
using namespace ServerSvcUnmanaged;
using namespace Common::Domain;
using namespace System::Collections::Generic;
using namespace Server::Device::Communication::Domain;
using namespace Common::Domain::Device;
using namespace Common::Services::Output; 
using namespace Common::Infrastructure::ErrorHandling::Output;
using namespace Common::Infrastructure::ErrorHandling::Enums;

public ref class ProcessingSvcFacade
{
private: 
	DeviceManager* _deviceManager;

public:	
	ProcessingSvcFacade();	
	List<Conveyor::ConveyorShot^>^ Test(String^ str, System::DateTime dt);
	List<DeviceOutgoingCommand^>^ GetOutgoingDataByDevice(DeviceIdentity^ deviceIdentity);
	List<DeviceIncommingCommand^>^ GetIncommingDataByDevice(DeviceIdentity^ deviceIdentity);
	void SendDataToDevice(DeviceOutgoingCommand^ commandData);
	/*void StopTestInternalProcessing();
	void StartTestInternalProcessing();*/

	void StartTest();
	void StopTest();

	void Initialize(Common::Domain::Device::DeviceConfig^ config);
	void InitializeStep2(List<DeviceIdentity^>^ devices);
	void SetConveyorSettings(Conveyor::ConveyorSettings^ conveyorSettings);
	List<Conveyor::ConveyorShot^>^ GetShapshotsFromQueue();
	List<DeviceIdentity^>^ GetDeviceListInfo();
	System::Boolean CheckDevicesInitStatus();
	void ProcessingSvcFacade::StartDeviceListening();
	
	//void ResetState();

private:
	List<Conveyor::ConveyorShot^>^ ConvertShapshots(std::vector<ConveyorShot>& from);
	String^ ToString(std::string str);
	std::string ToStdString(String^ str);
	System::DateTime ToDateTime(CDateTime& cdt);
};

