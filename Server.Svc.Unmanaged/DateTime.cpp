#include "stdafx.h"
#include "DateTime.h"
#include <chrono>
#include <ctime>

using namespace ServerSvcUnmanaged;
using namespace std;


CDateTime CDateTime::Now()
{
	using namespace std::chrono;
	milliseconds ms = duration_cast<milliseconds>(
		system_clock::now().time_since_epoch()
		);
	return CDateTime(ms.count());
}

long long CDateTime::NowMilliseconds()
{
	using namespace std::chrono;
	milliseconds ms = duration_cast<milliseconds>(
		system_clock::now().time_since_epoch()
		);
	return ms.count();
}

string CDateTime::ToString()
{
	using namespace std::chrono;
	milliseconds ms(mMilliseconds);
	seconds s = duration_cast<seconds>(ms);
	time_t t = s.count();

	return string(ctime(&t));
}