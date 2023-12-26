#pragma once

#include <string>
namespace ServerSvcUnmanaged {

	class CDateTime
	{
	public:
		CDateTime() {};
		CDateTime(long long inMilliseconds) : mMilliseconds(inMilliseconds) {};
		~CDateTime() {};

		std::string ToString();
		long long GetMilliseconds() const { return mMilliseconds; };
		void AddMilliseconds(long long inMilliseconds) { mMilliseconds += inMilliseconds; };
		bool operator == (const CDateTime& rVal) { return mMilliseconds == rVal.mMilliseconds; };
		bool operator > (const CDateTime& rVal) { return mMilliseconds > rVal.mMilliseconds; };
		bool operator < (const CDateTime& rVal) { return mMilliseconds < rVal.mMilliseconds; };

		static CDateTime Now();
		static long long NowMilliseconds();

	private:
		long long mMilliseconds = 0;
	};
}
