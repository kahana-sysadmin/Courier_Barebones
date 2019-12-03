using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Threading;

public class Syncbox : MonoBehaviour 
{
    // is really an inheritance structure, but using injection instead until
    // time for a refactor is available 
    private UPennSyncbox upennSync;
    private FreiburgSyncbox freiburgSync;
    public ScriptedEventReporter scriptedInput = null;

    public void Start() {
        upennSync = new UPennSyncbox(scriptedInput);
        freiburgSync = new FreiburgSyncbox(scriptedInput); 

        if(!upennSync.Init()) {
            upennSync = null;
        }

        if(!freiburgSync.Init()) {
            freiburgSync = null;
        }
    }

    public void StartPulse() {
        Debug.Log("Starting Pulses");
        upennSync?.StartPulse();
        freiburgSync?.StartPulse();
    }

    public void StopPulse() {
        upennSync?.StopPulse();
        freiburgSync?.StopPulse();
    }

    public void TestPulse() {
        Debug.Log("Testing");
        upennSync?.TestPulse();
        freiburgSync?.TestPulse();
    }
}
