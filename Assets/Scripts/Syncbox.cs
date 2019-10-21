using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Threading;

public class Syncbox : EventLoop 
{

    //Function from Corey's Syncbox plugin (called "ASimplePlugin")
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr OpenUSB();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr CloseUSB();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnLEDOn();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnLEDOff();
	[DllImport ("ASimplePlugin")]
	private static extern float SyncPulse();

    private const int PULSE_START_DELAY = 1000; // ms
    private const int TIME_BETWEEN_PULSES_MIN = 800;
    private const int TIME_BETWEEN_PULSES_MAX = 1200;

    private volatile bool stopped = true;

    private System.Random rnd;
    
    // from editor
    public ScriptedEventReporter scriptedInput = null;

    public void Awake() {
        Init();
        rnd = new System.Random();
    }

    public void Init() {
        //Debug.Log(Marshal.PtrToStringAuto(OpenUSB()));

        StopPulse();
        StartLoop();
    }

    public bool IsRunning() {
        return !stopped;
    }

    public void TestPulse() {
        if(!IsRunning()) {
            Do(new EventBase(StartPulse));
            DoIn(new EventBase(StopPulse), 5000);
        }
    }

    public void StartPulse() {
        if (!IsRunning())
        {
            stopped = false;
            DoIn(new EventBase(Pulse), PULSE_START_DELAY);
        }
    }

	private void Pulse ()
    {
		if(!stopped)
        {
            // Send a pulse
            if(scriptedInput != null)
                scriptedInput.ReportScriptedEvent("syncPulse", new System.Collections.Generic.Dictionary<string, object>());

            SyncPulse();

            // Wait a random interval between min and max
            int timeBetweenPulses = (int)(TIME_BETWEEN_PULSES_MIN + (int)(rnd.NextDouble() * (TIME_BETWEEN_PULSES_MAX - TIME_BETWEEN_PULSES_MIN)));
            DoIn(new EventBase(Pulse), timeBetweenPulses);
		}
	}

    public void StopPulse() {
        stopped = true;
    }

    public void OnDisable() {
        StopPulse();
    }
}
