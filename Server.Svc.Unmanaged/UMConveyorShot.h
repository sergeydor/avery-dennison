#pragma once
#include "UMTagColumn.h"

namespace ServerSvcUnmanaged {

	/// <summary>
	/// The current state of conveyor.
	/// I assume, that shots will be made each ~5ms
	/// </summary>
	struct ConveyorShot
	{
		/// <summary>
		/// 0th is the most left which is just appered
		/// </summary>
		std::vector<TagColumn> tagsColumns;

		/// <summary>
		/// Must be set from outside
		/// </summary>
		int totalLanes = 0;

		/// <summary>
		/// Calculated, based on encoder data, tag length and distance
		/// </summary>
		double velocityInMMPerMSec = 0;

		std::string name = "";

		long long createDt = 0;

		long long planToCreateDt = 0;


		int GetTagIndexByTriggerNo(unsigned int triggerNo);

		TagColumn* GetColumnTriggerNo(unsigned int triggerNo);

		TagColumn* GetCurrentColumn();

		void SetCurrentColumn(const TagColumn& inTagClmn);

		void ProcessNewEncoderItem(LinearEncoderDataItem& item, int distanceInMm, int lengthInMm, unsigned int tagsCountForVelocityMeasurement);

		void UpdateCurrentShotVelocity();

		void TrimLaneRight(int maxTagsOnLane);

		ConveyorShot CloneShapshot();
	};
}

