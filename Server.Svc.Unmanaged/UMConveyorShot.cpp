#include "Stdafx.h"
#include "UMConveyorShot.h"
#include <algorithm>

using namespace ServerSvcUnmanaged;

int ConveyorShot::GetTagIndexByTriggerNo(unsigned int triggerNo)
{
	for (int i = 0; i < tagsColumns.size(); ++i)
	{
		if (tagsColumns[i].linearEncoderData && tagsColumns[i].linearEncoderData->triggerNumber == triggerNo)
			return i;
	}
	return -1;
}

TagColumn* ConveyorShot::GetColumnTriggerNo(unsigned int triggerNo)
{
	for (int i = 0; i < tagsColumns.size(); ++i)
	{
		if (tagsColumns[i].linearEncoderData && tagsColumns[i].linearEncoderData->triggerNumber == triggerNo)
			return &tagsColumns[i];
	}
	return nullptr;
}

TagColumn* ConveyorShot::GetCurrentColumn()
{
	if (tagsColumns.empty())
		return nullptr;
	else
		return &tagsColumns[0]; // because of tagcolumn is struct - a copy is always retrieved from array
}

void ConveyorShot::SetCurrentColumn(const TagColumn& inTagClmn)
{
	tagsColumns[0] = inTagClmn;
}

void ConveyorShot::ProcessNewEncoderItem(LinearEncoderDataItem& item, int distanceInMm, int lengthInMm, unsigned int tagsCountForVelocityMeasurement)
{
	// in case if we're aware about tag length and distance - it's possible to get velocity 
	// based on distance between two neighbour tags

	if (TagColumn* currentColumn = GetCurrentColumn())
	{
		bool needCalcVelocity = currentColumn->linearEncoderData && currentColumn->linearEncoderData->HasItems() && tagsColumns.size() > 1 && distanceInMm > 0 && lengthInMm > 0;
		currentColumn->ProcessNewEncoderItem(item);

		if (needCalcVelocity)
		{
			unsigned int index = std::min<unsigned int>(tagsColumns.size() - 1, tagsCountForVelocityMeasurement - 1);
			TagColumn& prevTagCol = tagsColumns[index];
			if (!prevTagCol.linearEncoderData)
				return;
			LinearEncoderDataItem& prevTagFirstEncItem = prevTagCol.linearEncoderData->items[0];
			double tagsDistTime = (item.processingMockTime.GetMilliseconds() - prevTagFirstEncItem.processingMockTime.GetMilliseconds());
			double v = (lengthInMm + distanceInMm) * index / tagsDistTime;
			currentColumn->velocityBOnDistInMmPerMSec = v;
		}
	}
}

void ConveyorShot::UpdateCurrentShotVelocity()
{
	if (TagColumn* currentColumn = GetCurrentColumn())
	{
		double tagsDistanceBasedVelocity = 0;
		double encItemsBasedVelocity = 0;
		tagsDistanceBasedVelocity = currentColumn->velocityBOnDistInMmPerMSec;

		if (!currentColumn->linearEncoderData)
			return;
		auto led = currentColumn->linearEncoderData;
		led->UpdateMiddleTriggersVelocity();

		encItemsBasedVelocity = currentColumn->linearEncoderData->velocityAvgInMMPerMSec;

		if (tagsDistanceBasedVelocity > 0 && encItemsBasedVelocity > 0)
		{
			velocityInMMPerMSec = (tagsDistanceBasedVelocity + encItemsBasedVelocity) / 2;
		}
		else if (tagsDistanceBasedVelocity > 0)
		{
			velocityInMMPerMSec = tagsDistanceBasedVelocity;
		}
		else if (encItemsBasedVelocity > 0)
		{
			velocityInMMPerMSec = encItemsBasedVelocity;
		}
	}
}

void ConveyorShot::TrimLaneRight(int maxTagsOnLane)
{
	//int cnt = tagsColumns.size();
	//for (int i = 0; i < cnt - maxTagsOnLane; i++)
	//	this.TagsColumns.RemoveAt(maxTagsOnLane);//TODO: error

	int cnt = tagsColumns.size();
	for (int i = 0; i < cnt - maxTagsOnLane; i++)
		tagsColumns.pop_back();
}

ConveyorShot ConveyorShot::CloneShapshot()
{
	ConveyorShot shot(*this);
	return shot;
};