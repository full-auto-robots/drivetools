
// not used unless writing to disk

using System.Collections.Generic;

// the recording idea is this:
// ONLY STORE DATA ON NT for every timestamp
// the layout need only be stored once, and separately
// this class contains both data and layout for simplicity

[System.Serializable]
public class net_ntrecording
{
    public List<net_ntdatalist> snapshots;
    public List<double> timeStamps; // times since recording started, not boot time

    public dsp_layout layoutUponRecording; // dsp_layouts do have their own NT values that are flagged, but these can be ignored

    public net_ntdatalist GetSnapShot(double time)
    {
        for (int i = 0; i < timeStamps.Count; i++)
        {
            if (timeStamps[i] >= time)
            {
                return snapshots[i];
            }
        }

        return snapshots[snapshots.Count - 1]; // time is too big, so the closesnt snapshot we have is the last one
        // better to return actual useful data than just returning null, right?
    }
}
