#include "Stdafx.h"
#include "UMLinearEncoderData.h"

using namespace ServerSvcUnmanaged;

LinearEncoderDataItem* LinearEncoderData::GetCurrentItem()
{
	if (items.empty())
		return nullptr;
	else
		return &items[items.size() - 1]; // it's always last-added item
}

bool LinearEncoderData::HasItems()
{
	return !items.empty();
}

void LinearEncoderData::ProcessNewEncoderItem(LinearEncoderDataItem& item)
{
	if (HasItems())
	{
		LinearEncoderDataItem firstItem = items[0];
		CDateTime startProcTime = firstItem.processingMockTime;
		CDateTime currentProcTime = item.processingMockTime;
		// consider only timespan from 1st item, because timer is not reset for a new item
		//item.velocityInMMPerMSec = (item.xPosition - firstItem.xPosition) / (currentProcTime.GetMilliseconds() - startProcTime.GetMilliseconds());
		item.velocityInMMPerMSec = 0;
	}
	else
	{
		item.velocityInMMPerMSec = 0;
	}

	assert(item.triggerNumber == 0 || item.triggerNumber == triggerNumber);
	triggerNumber = item.triggerNumber;

	items.push_back(item);
}

/// <summary>
/// calculate velocity based on encoder triggers of the tag
/// </summary>
void LinearEncoderData::UpdateMiddleTriggersVelocity()
{
	int count = items.size();
	int cntInCalc = 0;
	double totVel = 0;
	for (int i = 0; i< count; i++)
	{
		double v = items[i].velocityInMMPerMSec;
		if (v > 0)
		{
			totVel += v;
			cntInCalc++;
		}
	}

	if (cntInCalc > 0)
	{
		velocityAvgInMMPerMSec = totVel / cntInCalc;
	}
	else
	{
		velocityAvgInMMPerMSec = 0;
	}
}