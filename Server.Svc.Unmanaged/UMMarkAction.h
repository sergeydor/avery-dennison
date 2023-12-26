#pragma once

#include "UMRFIDTag.h"
#include "UMActionBase.h"

namespace ServerSvcUnmanaged {

	struct MarkAction : ActionBase
	{
		RFIDTag tagToPunch;
	};

}