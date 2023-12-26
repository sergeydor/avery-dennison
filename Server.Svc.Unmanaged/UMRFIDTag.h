#pragma once
#include "UMVNAData.h"
#include "UMTypes.h"
#include <memory>

namespace ServerSvcUnmanaged {

	struct RFIDTag
	{
		int id = 0;
		int lineYIndex = 0;
		std::shared_ptr<DeviceCommand> rfidReaderData = nullptr;
		std::shared_ptr<VNAData> vnaData = nullptr;
		bool isVNATestPassed = true;
	};

}
