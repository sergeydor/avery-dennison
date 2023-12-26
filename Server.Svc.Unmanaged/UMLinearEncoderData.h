#pragma once
#include "UMLinearEncoderDataItem.h"
#include <vector>
#include <assert.h>

namespace ServerSvcUnmanaged {

	struct LinearEncoderData
	{
		std::vector<LinearEncoderDataItem> items;

			/// <summary>
			/// Tag's number on the conveyor, identified by encoder
			/// </summary>
		unsigned int triggerNumber = 0;

		double velocityAvgInMMPerMSec = 0;

		LinearEncoderDataItem* GetCurrentItem();
		bool HasItems();
		void ProcessNewEncoderItem(LinearEncoderDataItem& item);

		/// <summary>
		/// calculate velocity based on encoder triggers of the tag
		/// </summary>
		void UpdateMiddleTriggersVelocity();
	};
}