#pragma once
#include <map>
#include "UMRFIDTag.h"
#include "UMLinearEncoderData.h"
#include "UMLinearEncoderDataItem.h"
#include <memory>

namespace ServerSvcUnmanaged {

	struct TagColumn
	{
		std::map<int, RFIDTag> tags;
		std::shared_ptr<LinearEncoderData> linearEncoderData = nullptr;
		double velocityBOnDistInMmPerMSec = 0.0;
		double currentXPosition = 0.0;

		void ProcessNewEncoderItem(LinearEncoderDataItem& item)
		{
			if(linearEncoderData)
				linearEncoderData->ProcessNewEncoderItem( item);
		}
	};

}
