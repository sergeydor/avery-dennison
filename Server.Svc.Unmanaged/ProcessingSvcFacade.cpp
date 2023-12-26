#include "stdafx.h"
#include "ProcessingSvcFacade.h"

using namespace System;
using namespace Common::Domain;
using namespace System::Collections::Generic;
using namespace Server::Device::Communication::Domain;
using namespace Common::Enums::GSTCommands;
using namespace Server::Device::Communication;
using namespace System::Runtime::InteropServices;
using namespace Server::Device::Communication::CommandInterpretators;

ProcessingSvcFacade::ProcessingSvcFacade()
{
	_deviceManager = new DeviceManager();
}

List<DeviceOutgoingCommand^>^ ProcessingSvcFacade::GetOutgoingDataByDevice(DeviceIdentity^ deviceIdentity)
{
	std::string macAddress = msclr::interop::marshal_as<std::string, System::String^>(deviceIdentity->MacAddress);
	std::vector<DeviceCommand> unmgdDevCommands = _deviceManager->GetOutgoingDataByDevice(macAddress);
	int count = unmgdDevCommands.size();

	List<DeviceOutgoingCommand^>^ result = gcnew List<DeviceOutgoingCommand^>();

	for (int i = 0; i < count; i++)
	{
		ServerSvcUnmanaged::DeviceCommand& unmgdDevCmd = unmgdDevCommands[i];
				
		DeviceOutgoingCommand^ devOutCmd = gcnew DeviceOutgoingCommand();
		array<Byte>^ bytes = gcnew array<Byte>(64);
		Marshal::Copy((IntPtr)unmgdDevCmd.data, bytes, 0, 64);
		devOutCmd = devOutCmd->FromBytes(bytes);				
		devOutCmd->ReceiveDt = ToDateTime(unmgdDevCmd.receivedDate);
		result->Add(devOutCmd);
	}

	return result;
}

List<DeviceIncommingCommand^>^ ProcessingSvcFacade::GetIncommingDataByDevice(DeviceIdentity^ deviceIdentity)
{
	std::string macAddress = msclr::interop::marshal_as<std::string, System::String^>(deviceIdentity->MacAddress);
	std::vector<DeviceCommand> unmgdDevCommands = _deviceManager->GetIncommingDataByDevice(macAddress);
	int count = unmgdDevCommands.size();

	List<DeviceIncommingCommand^>^ result = gcnew List<DeviceIncommingCommand^>();

	for (int i = 0; i < count; i++)
	{
		ServerSvcUnmanaged::DeviceCommand& unmgdDevCmd = unmgdDevCommands[i];

		DeviceIncommingCommand^ devInCmd = gcnew DeviceIncommingCommand();
		array<Byte>^ bytes = gcnew array<Byte>(64);
		Marshal::Copy((IntPtr)unmgdDevCmd.data, bytes, 0, 64);
		devInCmd = devInCmd->FromBytes(bytes);
		devInCmd->DeviceIdentity = deviceIdentity;	
		devInCmd->ReceiveDt = ToDateTime(unmgdDevCmd.receivedDate);
		result->Add(devInCmd);
	}

	return result;
}

void ProcessingSvcFacade::SendDataToDevice(DeviceOutgoingCommand^ commandData)
{
	ServerSvcUnmanaged::DeviceCommand deviceCommand = ServerSvcUnmanaged::DeviceCommand();
	array<Byte>^ bytes = commandData->GetBytes();
	int count = bytes->Length;
	
	for (int i = 0; i < count; i++)
	{
		deviceCommand.data[i] = bytes[i];
	}

	std::string macAddress = this->ToStdString(commandData->DeviceIdentity->MacAddress);
	_deviceManager->SendCommandToDevice(macAddress, deviceCommand);
}

void ProcessingSvcFacade::StartTest()
{
	_deviceManager->StartTest();
}

void ProcessingSvcFacade::StopTest()
{
	_deviceManager->StopTest();
}

List<Conveyor::ConveyorShot^>^ ProcessingSvcFacade::GetShapshotsFromQueue()
{
	std::vector<ConveyorShot>& unmshots = _deviceManager->GetConveyorShots();
	List<Conveyor::ConveyorShot^>^ result = ConvertShapshots(unmshots);
	return result;
}

List<Conveyor::ConveyorShot^>^ ProcessingSvcFacade::Test(String ^ str, System::DateTime dt)
{	
	//this->_deviceManager->Start();
	//return nullptr;
	return nullptr;

	std::vector<ConveyorShot> shots;
	try {
		shots = _deviceManager->GenerateSnapshotsForMashalingTest();
	}
	catch (Exception^ ex)
	{
		Console::WriteLine(ex->GetType());
	}
	catch (char const* ch)
	{
		Console::WriteLine("std::exception");
	}

	System::Diagnostics::Stopwatch^ w = System::Diagnostics::Stopwatch::StartNew();
	List<Conveyor::ConveyorShot^>^ managedShots = this->ConvertShapshots(shots);
	Console::WriteLine(w->ElapsedMilliseconds + "ms");

	shots = _deviceManager->GenerateSnapshotsForMashalingTest();
	w = System::Diagnostics::Stopwatch::StartNew();
	managedShots = this->ConvertShapshots(shots);
	Console::WriteLine(w->ElapsedMilliseconds + "ms");

	shots = _deviceManager->GenerateSnapshotsForMashalingTest();
	w = System::Diagnostics::Stopwatch::StartNew();
	managedShots = this->ConvertShapshots(shots);
	Console::WriteLine(w->ElapsedMilliseconds + "ms");

	return managedShots;
}

void ProcessingSvcFacade::Initialize(Common::Domain::Device::DeviceConfig^ config)
{
	ServerSvcUnmanaged::DeviceConfig deviceUnmgdConfig;
	deviceUnmgdConfig.vendorId = config->VendorId;
	deviceUnmgdConfig.readerProductId = config->ReaderProductId;
	deviceUnmgdConfig.GPIOProductId = config->GPIOProductId;
	deviceUnmgdConfig.readersCount = config->NumberOfReadersSetOnUI;

	bool res = _deviceManager->Initialize(deviceUnmgdConfig);
	if (!res) {
		throw gcnew System::Exception("Initialization has been failed");
	}
}

void ProcessingSvcFacade::StartDeviceListening()
{
	_deviceManager->Start(); // accept all found devices
}

void ProcessingSvcFacade::SetConveyorSettings(Conveyor::ConveyorSettings^ conveyorSettings)
{
	ServerSvcUnmanaged::ConveyorSettings unmgdSettings;
	unmgdSettings.tagLengthInMm = conveyorSettings->TagLengthInMm;
	unmgdSettings.distanceBetweenTagsInMm = conveyorSettings->DistanceBetweenTagsInMm;
	unmgdSettings.tagsCountPerOneLane = conveyorSettings->TagsCountPerOneLane;
	//unmgdSettings.encoderReaderTagsDistance = conveyorSettings->EncoderReaderTagsDistance;
	//unmgdSettings.readerMarkerTagsDistance = conveyorSettings->ReaderMarkerTagsDistance;
	//unmgdSettings.markerPuncherTagsDistance = conveyorSettings->MarkerPuncherTagsDistance;
	unmgdSettings.timerCicleLengthMs = conveyorSettings->TimerCicleLengthMs;

	_deviceManager->SetConveyorSettings(unmgdSettings);
}

List<DeviceIdentity^>^ ProcessingSvcFacade::GetDeviceListInfo()
{
	List<DeviceIdentity^>^ result = gcnew List<DeviceIdentity^>();

	std::vector<ServerSvcUnmanaged::DeviceInfo>& devicesList = _deviceManager->GetDeviceListInfo();
	int dvcCount = devicesList.size();
	for (int i = 0; i < dvcCount; i++)
	{
		ServerSvcUnmanaged::DeviceInfo& unmgdDevice = devicesList[i];
		Common::Domain::Device::DeviceIdentity^ mngdDevice = gcnew Common::Domain::Device::DeviceIdentity();

		mngdDevice->DeviceType = (Common::Enums::GSTCommands::HighSpeedTestDeviceType)unmgdDevice.mType;
		mngdDevice->MacAddress = msclr::interop::marshal_as<System::String^, std::string>(unmgdDevice.mMACAddress);
		mngdDevice->Lane = unmgdDevice.mLane;

		result->Add(mngdDevice);
	}

	return result;
}

void ProcessingSvcFacade::InitializeStep2(List<DeviceIdentity^>^ devices)
{
	int count = devices->Count;
	std::vector<DeviceInfo>& unmgdDevices = std::vector<ServerSvcUnmanaged::DeviceInfo>(count);
	for (int i = 0; i < count; i++)
	{
		DeviceIdentity^ mngdDeviceIdentity = devices[i];
		ServerSvcUnmanaged::DeviceInfo deviceInfo;
		deviceInfo.mType = (ServerSvcUnmanaged::DeviceType)mngdDeviceIdentity->DeviceType;
		deviceInfo.mMACAddress = msclr::interop::marshal_as<std::string, System::String^>(mngdDeviceIdentity->MacAddress);
		deviceInfo.mLane = mngdDeviceIdentity->Lane;

		unmgdDevices[i] = deviceInfo;
	}
	_deviceManager->InitializeStep2(unmgdDevices);
}

List<Conveyor::ConveyorShot^>^ ProcessingSvcFacade::ConvertShapshots(std::vector<ConveyorShot>& from)
{
	std::vector<ConveyorShot>& shots = from;

	int shotsCnt = shots.size();
	List<Conveyor::ConveyorShot^>^ managedShots = gcnew List<Conveyor::ConveyorShot^>();
	UnsolicitedReplyCommandsInterpretator^ unsCmdInterpretator = gcnew UnsolicitedReplyCommandsInterpretator();

	for (int i = 0; i < shotsCnt; i++)
	{
		ConveyorShot& curUnmgShot = shots[i];
		Conveyor::ConveyorShot^ curMgShotCopy = gcnew Conveyor::ConveyorShot();
		curMgShotCopy->VelocityInMMPerMSec = curUnmgShot.velocityInMMPerMSec;
		curMgShotCopy->TotalLanes = curUnmgShot.totalLanes;
		curMgShotCopy->TagsColumns = gcnew List<Conveyor::TagColumn^>(150);

		int colsCnt = curUnmgShot.tagsColumns.size();
		for (int j = 0; j < colsCnt; j++)
		{
			TagColumn& curUnmgCol = curUnmgShot.tagsColumns[j];
			Conveyor::TagColumn^ curMgColCopy = gcnew Conveyor::TagColumn();
			curMgColCopy->VelocityBOnDistInMmPerMSec = curUnmgCol.velocityBOnDistInMmPerMSec;

			if (auto ledUnmg = curUnmgCol.linearEncoderData)
			{
				Conveyor::LinearEncoderData ledMgCopy;
				ledMgCopy.Items = gcnew List<Conveyor::LinearEncoderDataItem>();

				int ledItemsCnt = ledUnmg->items.size();
				for (int k = 0; k < ledItemsCnt; k++)
				{
					LinearEncoderDataItem& ledUnmgItem = ledUnmg->items[k];
					Conveyor::LinearEncoderDataItem ledMgItemCopy = Conveyor::LinearEncoderDataItem();
					ledMgItemCopy.DeviceTimeStamp = System::TimeSpan(ledUnmgItem.deviceTimeStamp * 10000); // assume c++ value is in ms, so convert to ticks // just long in ms
					ledMgItemCopy.ProcessingMockTime = ToDateTime(ledUnmgItem.processingMockTime); 
					ledMgItemCopy.TriggerNumber = ledUnmgItem.triggerNumber;
					ledMgItemCopy.VelocityInMMPerMSec = ledUnmgItem.velocityInMMPerMSec;
					ledMgItemCopy.XPosition = ledUnmgItem.xPosition;
					ledMgCopy.Items->Add(ledMgItemCopy);
				}

				curMgColCopy->LinearEncoderData = ledMgCopy;
			}
			int tagsCnt = curUnmgCol.tags.size();
			//curMgColCopy.Tags = gcnew array<Conveyor::RFIDTag>(tagsCnt);
			if (tagsCnt > 0)
			{
				curMgColCopy->Tags = gcnew System::Collections::Generic::List<System::Tuple<int, Conveyor::RFIDTag>^>(); // gcnew System::Collections::Generic::Dictionary<int, Conveyor::RFIDTag>();
			}

			for (auto it = curUnmgCol.tags.begin(); it != curUnmgCol.tags.end(); ++it)
			{
				int laneNum = it->first;
				RFIDTag& unmgTag = it->second;
				//RFIDTag& unmgTag = curUnmgCol.tags[k];
				Conveyor::RFIDTag mgTagCopy = Conveyor::RFIDTag();
				mgTagCopy.IsVNATestPassed = unmgTag.isVNATestPassed;
				mgTagCopy.LineYIndex = unmgTag.lineYIndex;

				if (unmgTag.rfidReaderData)
				{
					DeviceIncommingCommand^ devInCmd = nullptr;
					array<Byte>^ bytes = gcnew array<Byte>(64);
					Marshal::Copy((IntPtr)unmgTag.rfidReaderData->data, bytes, 0, 64);
					devInCmd = DeviceIncommingCommand::FromBytes(bytes);					
					devInCmd->ReceiveDt = ToDateTime(unmgTag.rfidReaderData->receivedDate);
					mgTagCopy.RFIDReaderData = unsCmdInterpretator->ConvertResponseDataToTestE3Result(devInCmd->Data);
				}

				/*
				if (unmgTag.vnaData)
				{
					Conveyor::VNAData mgVnaData = Conveyor::VNAData();
					List<Conveyor::VNADataItem>^ vnaDataItems = gcnew List<Conveyor::VNADataItem>(10);

					int vnaItemsCnt = unmgTag.vnaData->items.size();
					for (int n = 0; n < vnaItemsCnt; n++)
					{
						VNADataItem& unmgVnaItem = unmgTag.vnaData->items[n];
						Conveyor::VNADataItem mgVnaItem = Conveyor::VNADataItem();
						mgVnaItem.RadioFrequency = unmgVnaItem.radioFrequency;
						mgVnaItem.Value = unmgVnaItem.value; // TODO - update re. to hex command
						vnaDataItems->Add(mgVnaItem);
					}

					mgVnaData.Items = vnaDataItems;
					mgTagCopy.VNAData = mgVnaData;
				}*/

				curMgColCopy->SetTagOnLane(laneNum, mgTagCopy);
			}
			curMgShotCopy->TagsColumns->Add(curMgColCopy);
		}

		managedShots->Add(curMgShotCopy);
	}
	return managedShots;
}

System::Boolean ProcessingSvcFacade::CheckDevicesInitStatus()
{
	bool result = _deviceManager->CheckDevicesInitStatus();

	return result;
}

//void ProcessingSvcFacade::ResetState() 
//{
//	_deviceManager->ResetState();
//}

String^ ProcessingSvcFacade::ToString(std::string str)
{
	return msclr::interop::marshal_as<System::String^, std::string>(str);
}

std::string ProcessingSvcFacade::ToStdString(String^ str)
{
	return msclr::interop::marshal_as<std::string, System::String^>(str);
}

System::DateTime ProcessingSvcFacade::ToDateTime(CDateTime& cdt)
{
	UInt64 ticksTill_1970 = DateTime(1970, 1, 1, 0, 0, 0).Ticks;
	DateTime dtUtc = System::DateTime(ticksTill_1970 + cdt.GetMilliseconds() * 10000);
	double utcDiff = (DateTime::Now - DateTime::UtcNow).TotalMilliseconds;
	System::DateTime result = dtUtc.AddMilliseconds(utcDiff);
	return result;
}

