#include <sys/types.h>

#include <comutil.h>
#include "System/Platform.h"
#include "Version.h"
#include "System/Config.h"
#include "System/Events/EventLoop.h"
#include "System/IO/IOProcessor.h"
#include "Framework/ReplicatedLog/ReplicatedLog.h"

#ifdef DEBUG
#define VERSION_FMT_STRING "PaxosLease v" VERSION_STRING " (DEBUG build date " __DATE__ " " __TIME__ ")"
#else
#define VERSION_FMT_STRING "PaxosLease v" VERSION_STRING 
#endif

extern "C" __declspec(dllexport) void Start(char* configFileName)
{	
	enum		{ single, replicated, missing } mode;
	int			logTargets;

	mode = replicated;

	if (!Config::Init(configFileName))
		STOP_FAIL("Cannot open config file", 1);

	logTargets = 0;
	if (Config::GetListNum("log.targets") == 0)
		logTargets = LOG_TARGET_STDOUT;
	for (int i = 0; i < Config::GetListNum("log.targets"); i++)
	{
		if (strcmp(Config::GetListValue("log.targets", i, ""), "file") == 0)
		{
			logTargets |= LOG_TARGET_FILE;
			Log_SetOutputFile(Config::GetValue("log.file", NULL), 
							Config::GetBoolValue("log.truncate", false));
		}
		if (strcmp(Config::GetListValue("log.targets", i, NULL), "stdout") == 0)
			logTargets |= LOG_TARGET_STDOUT;
		if (strcmp(Config::GetListValue("log.targets", i, NULL), "stderr") == 0)
			logTargets |= LOG_TARGET_STDERR;
	}
	Log_SetTarget(logTargets);
	Log_SetTrace(Config::GetBoolValue("log.trace", false));
	Log_SetTimestamping(Config::GetBoolValue("log.timestamping", false));

	Log_Message(VERSION_FMT_STRING " started");

	//run:
	{
		if (!IOProcessor::Init(Config::GetIntValue("io.maxfd", 1024), true))
			STOP_FAIL("Cannot initalize IOProcessor!", 1);

		if (!RCONF->Init())
			STOP_FAIL("Cannot initialize paxos!", 1);

		RLOG->Init(Config::GetBoolValue("paxos.useSoftClock", true));

		EventLoop::Init();
		EventLoop::Run();
		EventLoop::Shutdown();

		RLOG->Shutdown();
		IOProcessor::Shutdown();

		//goto run;
	}

	Log_Message("PaxosLease shutting down.");	
	Config::Shutdown();
	Log_Shutdown();
}

extern "C" __declspec(dllexport) void Stop()
{	
	EventLoop::Stop();
}

extern "C" __declspec(dllexport) bool IsRunning()
{	
	return EventLoop::IsRunning();
}

extern "C" __declspec(dllexport) int GetMaster()
{
	return RLOG->GetMaster();
}

extern "C" __declspec(dllexport) bool IsMasterLeaseActive()
{
	return RLOG->IsMasterLeaseActive();
}

//extern "C" __declspec(dllexport) BSTR Init(wchar_t* configFileName)
//{		
//	return ::SysAllocString(L"configFileName ");
//}

//BSTR ANSItoBSTR(char* input)
//{
//    BSTR result = NULL;
//    int lenA = lstrlenA(input);
//    int lenW = ::MultiByteToWideChar(CP_ACP, 0, input, lenA, NULL, 0);
//    if (lenW > 0)
//    {
//        BSTR result = ::SysAllocStringLen(0, lenW);
//        ::MultiByteToWideChar(CP_ACP, 0, input, lenA, result, lenW);
//    } 
//    return result;
//}