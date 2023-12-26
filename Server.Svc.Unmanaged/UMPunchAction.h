#pragma once
#include "UMRFIDTag.h"
#include "UMActionBase.h"

namespace ServerSvcUnmanaged {

	struct PunchAction : ActionBase
	{
		RFIDTag tagToPunch;
	};

}

