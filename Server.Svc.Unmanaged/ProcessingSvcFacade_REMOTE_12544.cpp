#include "stdafx.h"
#include "ProcessingSvcFacade.h"

using namespace System;
using namespace Common::Domain;
using namespace System::Collections::Generic;

ProcessingSvcFacade::ProcessingSvcFacade()
{
	_deviceManager = new DeviceManager();
}

List<DeviceOutgoingCommand^>^ ProcessingSvcFacade::GetOutgoingDataByDevice(DeviceIdentity deviceIdentity)
{
	return nullptr;
}

List<DeviceIncommingCommand^>^ ProcessingSvcFacade::GetIncommingDataByDevice(DeviceIdentity deviceIdentity)
{
	return nullptr;
}

void ProcessingSvcFacade::SendDataToDevice(DeviceOutgoingCommand commandData)
{	
}

void ProcessingSvcFacade::StartTestInternalProcessing()
{	
}

void ProcessingSvcFacade::StopTestInternalProcessing()
{
}

List<Conveyor::ConveyorShot^>^ ProcessingSvcFacade::Test(String ^ str, System::DateTime dt)
{	
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
	//return InitializeStatus::Success;
}

void ProcessingSvcFacade::SetConveyorSettings(Conveyor::ConveyorSettings^ conveyorSettings)
{

}

List<DeviceIdentity^>^ ProcessingSvcFacade::GetDeviceListInfo()
{
	List<DeviceIdentity^>^ result = gcnew List<DeviceIdentity^>();

	return result;
}

List<Conveyor::ConveyorShot^>^ ProcessingSvcFacade::ConvertShapshots(std::vector<ConveyorShot>& from)
{
	std::vector<ConveyorShot>& shots = from;

	int shotsCnt = shots.size();
	List<Conveyor::ConveyorShot^>^ managedShots = gcnew List<Conveyor::ConveyorShot^>();

	for (int i = 0; i < shotsCnt; i++)
	{
		ConveyorShot& curUnmgShot = shots[i];
		Conveyor::ConveyorShot^ curMgShotCopy = gcnew Conveyor::ConveyorShot();
		curMgShotCopy->Name = this->ToString(curUnmgShot.name);
		curMgShotCopy->VelocityInMMPerMSec = curUnmgShot.velocityInMMPerMSec;
		curMgShotCopy->TotalLanes = curUnmgShot.totalLanes;
		curMgShotCopy->TotalLaneOffsetInMm = curUnmgShot.totalLaneOffsetInMm;

		curMgShotCopy->TagsColumns = gcnew List<Conveyor::TagColumn>(150);
		int colsCnt = curUnmgShot.tagsColumns.size();
		for (int j = 0; j < colsCnt; j++)
		{
			TagColumn& curUnmgCol = curUnmgShot.tagsColumns[j];
			Conveyor::TagColumn curMgColCopy;
			curMgColCopy.CurrentXPosition = curUnmgCol.currentXPosition;
			curMgColCopy.VelocityBOnDistInMmPerMSec = curUnmgCol.velocityBOnDistInMmPerMSec;

			Conveyor::LinearEncoderData ledMgCopy;
			ledMgCopy.Items = gcnew List<Conveyor::LinearEncoderDataItem>();
			LinearEncoderData& ledUnmg = curUnmgCol.linearEncoderData;
			int ledItemsCnt = ledUnmg.items.size();
			for (int k = 0; k < ledItemsCnt; k++)
			{
				LinearEncoderDataItem& ledUnmgItem = ledUnmg.items[k];
				Conveyor::LinearEncoderDataItem ledMgItemCopy = Conveyor::LinearEncoderDataItem();
				ledMgItemCopy.DeviceTimeStamp = System::TimeSpan(ledUnmgItem.deviceTimeStamp * 10000); // assume c++ value is in ms, so convert to ticks // just long in ms
				ledMgItemCopy.ProcessingMockTime = System::DateTime(ledUnmgItem.processingMockTime.GetMilliseconds() * 10000); // assume c++ value is in ms, so convert to ticks  // ms from 1970
				ledMgItemCopy.TriggerNumber = ledUnmgItem.triggerNumber;
				ledMgItemCopy.VelocityInMMPerMSec = ledUnmgItem.velocityInMMPerMSec;
				ledMgItemCopy.XPosition = ledUnmgItem.xPosition;
				ledMgCopy.Items->Add(ledMgItemCopy);
			}

			curMgColCopy.LinearEncoderData = ledMgCopy;
			int tagsCnt = curUnmgCol.tags.size();
			curMgColCopy.Tags = gcnew array<Conveyor::RFIDTag>(tagsCnt);

			for (int k = 0; k < tagsCnt; k++)
			{
				RFIDTag& unmgTag = curUnmgCol.tags[k];
				Conveyor::RFIDTag mgTagCopy = Conveyor::RFIDTag();
				mgTagCopy.Id = Guid(); // todo
				mgTagCopy.IsVNATestPassed = unmgTag.isVNATestPassed;
				mgTagCopy.LineYIndex = unmgTag.lineYIndex;

				Conveyor::RFIDReaderData mnRdData = Conveyor::RFIDReaderData();;
				mnRdData.EPC = this->ToString(unmgTag.rfidReaderData.epc);
				mnRdData.LineYNumber = unmgTag.rfidReaderData.lineYNumber;
				mnRdData.RSSI = unmgTag.rfidReaderData.rssi;
				mnRdData.TID = this->ToString(unmgTag.rfidReaderData.tid);
				mgTagCopy.RFIDReaderData = mnRdData;

				Conveyor::VNAData mgVnaData = Conveyor::VNAData();
				List<Conveyor::VNADataItem>^ vnaDataItems = gcnew List<Conveyor::VNADataItem>(10);

				int vnaItemsCnt = unmgTag.vnaData.items.size();
				for (int n = 0; n < vnaItemsCnt; n++)
				{
					VNADataItem& unmgVnaItem = unmgTag.vnaData.items[n];
					Conveyor::VNADataItem mgVnaItem = Conveyor::VNADataItem();
					mgVnaItem.RadioFrequency = unmgVnaItem.radioFrequency;
					mgVnaItem.Value = unmgVnaItem.value; // TODO - update re. to hex command
					vnaDataItems->Add(mgVnaItem);
				}

				mgVnaData.Items = vnaDataItems;
				mgTagCopy.VNAData = mgVnaData;

				curMgColCopy.Tags[k] = mgTagCopy;
			}
			curMgShotCopy->TagsColumns->Add(curMgColCopy);
		}

		managedShots->Add(curMgShotCopy);
	}
	return managedShots;
}

String^ ProcessingSvcFacade::ToString(std::string str)
{
	return msclr::interop::marshal_as<System::String^, std::string>(str);
}

std::string ProcessingSvcFacade::ToStdString(String^ str)
{
	return msclr::interop::marshal_as<std::string, System::String^>(str);
}
