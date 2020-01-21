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

    public void Awake() {
        upennSync = new UPennSyncbox(scriptedInput);
        freiburgSync = new FreiburgSyncbox(scriptedInput); 

        try {
            if(!upennSync.Init()) {
                Debug.Log("Invalid Handle");
                upennSync = null;
            }
        }
        catch {
            Debug.Log("Failed opening Upenn Sync");
        }

        try{
            if(!freiburgSync.Init()) {
                freiburgSync = null;
            }
        }
        catch {
            Debug.Log("Failed opening freiburg sync");
        }
    }

    public void StartPulse() {
        Debug.Log("Starting Pulses");
        Debug.Log(upennSync);
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

    public void OnDisable() {
        upennSync?.OnDisable();
        freiburgSync?.OnDisable();
    }
}
