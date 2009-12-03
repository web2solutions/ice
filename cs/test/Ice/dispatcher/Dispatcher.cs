// **********************************************************************
//
// Copyright (c) 2003-2009 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;

public class Dispatcher : Ice.Dispatcher
{
    private static void test(bool b)
    {
        if(!b)
        {
            throw new System.Exception();
        }
    }

    public Dispatcher()
    {
        Debug.Assert(_instance == null);
        _instance = this;
        _thread = new Thread(run);
        _thread.Start();
    }

    public void 
    run()
    {
        while(true)
        {
            Ice.DispatcherCall call = null;
            lock(this)
            {
                if(!_terminated && _calls.Count == 0)
                {
                    Monitor.Wait(this);
                }
                
                if(_calls.Count > 0)
                {
                    call = _calls.Dequeue();
                }
                else if(_terminated)
                {
                    // Terminate only once all calls are dispatched.
                    return;
                }
            }
            
            if(call != null)
            {
                try
                {
                    call();
                }
                catch(System.Exception)
                {
                    // Exceptions should never propagate here.
                    test(false);
                }
            }
        }
    }
    
    public void
    dispatch(Ice.DispatcherCall call, Ice.Connection con)
    {
        lock(this)
        {
            _calls.Enqueue(call);
            if(_calls.Count == 1)
            {
                Monitor.Pulse(this);
            }
        }
    }

    static public void
    terminate()
    {
        lock(_instance)
        {
            _instance._terminated = true;
            Monitor.Pulse(_instance);
        }

        _instance._thread.Join();
    }
    
    static public bool
    isDispatcherThread()
    {
        return Thread.CurrentThread == _instance._thread;
    }

    static Dispatcher _instance; 

    private Queue<Ice.DispatcherCall> _calls = new Queue<Ice.DispatcherCall>();
    Thread _thread;
    bool _terminated = false;
};