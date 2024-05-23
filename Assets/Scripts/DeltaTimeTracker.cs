using System.Collections.Generic;
using System.Linq;

public class DeltaTimeTracker
{
    public string name;
    public Dictionary<string, float> trackedData = new Dictionary<string, float>();
    private List<float> deltaTimes = new List<float>();


    public DeltaTimeTracker(string nameIn) { name = nameIn; }

    public void addTime(float deltaTime) { deltaTimes.Add(deltaTime); }

    public void resetTime() { deltaTimes.Clear(); }

    public void calculateAverage(string current)
    {
        trackedData.Add(current, deltaTimes.Sum() / deltaTimes.Count());

        resetTime();
    }



}
