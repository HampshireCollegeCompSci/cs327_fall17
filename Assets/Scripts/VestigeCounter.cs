//Author: Wm. Josiah Erikson
//This class keep track of the current number of vestiges and also the peak number of vestiges, for analytics
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VestigeCounter : MonoBehaviour {

    int peakVestiges;
    int currentVestiges;

    public int GetCurrentVestiges () {
        return currentVestiges;
    }

    public void SetCurrentVestiges (int vestiges)
    {
        currentVestiges = vestiges;

        if (currentVestiges > peakVestiges) {
            peakVestiges = currentVestiges;
        }
    }

    public int GetPeakVestiges ()
    {
        return peakVestiges;
    }
}
