#ifndef ProcessState_H_
#define ProcessState_H_

#include "time/time_base.h"
#include "support/support.h"

#define USECS_100THSEC  10000
#define USECS_10THSEC   100000

struct ProcessState
{
	Time_Clock   _system_time;
    Time_Out     _flush_interval;
    bool         _flush_time;


    ProcessState(long t_sec) : _flush_interval(Time_Span(t_sec,USECS_10THSEC))
    {
    }

    void Process() 
    {
        _flush_time = false;

        _system_time.ToCurrentTime();

        if(_flush_interval.Expired(_system_time,true))
            _flush_time = true;

        xsleep(1);
    }
};


#endif 

