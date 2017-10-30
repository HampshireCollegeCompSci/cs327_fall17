// Author(s): Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class VoidEvent
{
    // Invoked when the event is started.
    public delegate void StartedHandler(EventType eventType, int tier);
    public event StartedHandler Started;
    // Invoked when the event is finished.
    public delegate void FinishedHandler(EventType eventType, int tier);
    public event FinishedHandler Finished;

    public enum EventType
    {
        Junkyard,
        Radiation,
        Asteroids
    }

    // The type of the event.
    EventType eventType;
    // The tier of the event.
    int tier;

    // Constructor.
    public VoidEvent(EventType eventTypeIn, int tierIn)
    {
        eventType = eventTypeIn;
        tier = tierIn;
    }

    public void Start()
    {
        OnStarted();
    }

    public void Finish()
    {
        OnFinished();
    }

    private void OnStarted()
    {
        if (Started != null)
        {
            Started(eventType, tier);
        }
    }
    private void OnFinished()
    {
        if (Finished != null)
        {
            Finished(eventType, tier);
        }
    }
}

// A collection of void events, grouped together.
public class VoidEventGroup
{
    public enum Status
    {
        Waiting, // Score hasn't passed begin.
        InProgress, // Score is between begin and end.
        Finished // Score is past end.
    }

    // The score at which the event group begins.
    int begin;
    // The score at which the event group ends. Use -1 for endless.
    int end;
    // The list of void events.
    List<VoidEvent> voidEvents;
    // The current status of the event group.
    Status status = Status.Waiting;

    // Constructor.
    public VoidEventGroup(List<VoidEvent> voidEventsIn, int beginIn, int endIn)
    {
        voidEvents = voidEventsIn;
        begin = beginIn;
        end = endIn;
    }

    public void UpdateProgressFromScore(int score)
    {
        if (status == Status.Waiting)
        {
            if (score >= begin)
            {
                status = Status.InProgress;
                foreach (VoidEvent ve in voidEvents)
                {
                    ve.Start();
                }
            }
        }
        if (status == Status.InProgress)
        {
            if (score >= end && end != -1)
            {
                status = Status.Finished;
                foreach (VoidEvent ve in voidEvents)
                {
                    ve.Finish();
                }
            }
        }
    }

    /*
    // Returns true if the given score is between beginning (inclusive) and end (exclusive).
    public bool IsScoreInRange(int givenScore)
    {
        return (givenScore >= begin && givenScore < end);
    }
    */
}

public class VoidEventController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the ScoreCounter instance.")]
    ScoreCounter scoreCounter;
    [SerializeField]
    [Tooltip("Reference to the BlockSpawner instance.")]
    BlockSpawner blockSpawner;
    [SerializeField]
    [Tooltip("Reference to the Grid instance.")]
    Grid grid;
    [SerializeField]
    [Tooltip("Reference to the void events JSON.")]
    TextAsset voidEventsJSON;
    /*
    [SerializeField]
    [Tooltip("Reference to the tuning JSON.")]
    TextAsset tuningJSON;
    */

    // The list of VoidEventGroups to keep track of.
    List<VoidEventGroup> voidEventGroups = new List<VoidEventGroup>();

    // The bindings of letters to event types.
    Dictionary<char, VoidEvent.EventType> letterBindings
        = new Dictionary<char, VoidEvent.EventType>();

    // Maps tiers to values.
    Dictionary<int, int> tierToVestigeCount = new Dictionary<int, int>();
    Dictionary<int, int> tierToVestigeLevel = new Dictionary<int, int>();
    Dictionary<int, int> tierToDecayBonus = new Dictionary<int, int>();
    Dictionary<int, int> tierToAsteroidCount = new Dictionary<int, int>();

    private void Start()
    {
        // Subscribe to delegate events.
        scoreCounter.ScoreChanged += ScoreCounter_ScoreChanged;

        // Randomize letter bindings.
        List<char> letters = new List<char>(3);
        letters.Add('a');
        letters.Add('b');
        letters.Add('c');
        List<VoidEvent.EventType> types = new List<VoidEvent.EventType>();
        types.Add(VoidEvent.EventType.Junkyard);
        types.Add(VoidEvent.EventType.Radiation);
        types.Add(VoidEvent.EventType.Asteroids);
        for (int i = 0; i < 3; ++i)
        {
            int index = Random.Range(0, letters.Count);
            char letter = letters[index];
            letters.RemoveAt(index);
            letterBindings.Add(letter, types[i]);
        }

        // Read from tuning data.
        Tune();
    }

    // Read variables from JSON data.
    private void Tune()
    {
        JSONNode json = JSON.Parse(voidEventsJSON.ToString());

        // Read JSON data for tier-specific data.
        JSONNode nodeTierData = json["tier-specific data"];
        for (int i = 0; i < nodeTierData.Count; ++i)
        {
            JSONNode nodeTier = nodeTierData[i];
            int tier = nodeTier["tier"].AsInt;
            int radiationVestigeCount = nodeTier["radiation vestige count"].AsInt;
            tierToVestigeCount.Add(tier, radiationVestigeCount);
            int radiationVestigeLevel = nodeTier["radiation vestige level"].AsInt;
            tierToVestigeLevel.Add(tier, radiationVestigeLevel);
            int radiationBEDRB = nodeTier["radiation base energy decay rate bonus"].AsInt;
            tierToDecayBonus.Add(tier, radiationBEDRB);
            int asteroidCount = nodeTier["asteroid count"].AsInt;
            tierToAsteroidCount.Add(tier, asteroidCount);
        }

        // Read the actual void event JSON data.
        JSONNode nodeEvents = json["events"];
        for (int i = 0; i < nodeEvents.Count; ++i)
        {
            JSONNode nodeEvent = nodeEvents[i];
            int begin = nodeEvent["begin"].AsInt;
            int end = nodeEvent["end"].AsInt;
            JSONArray types = nodeEvent["types"].AsArray;

            List<VoidEvent> myEvents = new List<VoidEvent>();
            for (int j = 0; j < types.Count; ++j)
            {
                string typeString = types[j];
                char tierLetter = typeString[0];
                char typeLetter = typeString[1];

                VoidEvent.EventType theType = letterBindings[typeLetter];
                int tier = (int)char.GetNumericValue(tierLetter);

                VoidEvent newEvent = new VoidEvent(theType, tier);
                newEvent.Started += VoidEvent_Started;
                newEvent.Finished += VoidEvent_Finished;
                myEvents.Add(newEvent);
            }

            voidEventGroups.Add(new VoidEventGroup(myEvents, begin, end));
        }
    }

    // Callback function for the score changing.
    private void ScoreCounter_ScoreChanged(int newScore)
    {
        foreach (VoidEventGroup eventGroup in voidEventGroups)
        {
            eventGroup.UpdateProgressFromScore(newScore);
        }
    }

    private void VoidEvent_Started(VoidEvent.EventType eventType, int tier)
    {
        switch (eventType)
        {
            case VoidEvent.EventType.Junkyard:
                Debug.Log("Junkyard " + tier + " begin.");
                blockSpawner.SetJunkyardTier(tier);
                break;

            case VoidEvent.EventType.Radiation:
                Debug.Log("Radiation " + tier + " begin.");
                blockSpawner.SetVestigesPerBlock(tierToVestigeCount[tier]);
                blockSpawner.SetVestigeLevel(tierToVestigeLevel[tier]);
                grid.SetBaseEnergyDecayRateBonus(tierToDecayBonus[tier]);
                break;

            case VoidEvent.EventType.Asteroids:
                Debug.Log("Asteroids " + tier + " begin.");
                grid.AddAsteroids(tierToAsteroidCount[tier]);
                break;
        }
    }

    private void VoidEvent_Finished(VoidEvent.EventType eventType, int tier)
    {
        switch (eventType)
        {
            case VoidEvent.EventType.Junkyard:
                Debug.Log("Junkyard " + tier + " end.");
                blockSpawner.SetJunkyardTier(0);
                break;

            case VoidEvent.EventType.Radiation:
                Debug.Log("Radiation " + tier + " end.");
                blockSpawner.SetVestigesPerBlock(0);
                grid.SetBaseEnergyDecayRateBonus(0);
                // Don't need to do anything to vestige level.
                break;

            case VoidEvent.EventType.Asteroids:
                Debug.Log("Asteroids " + tier + " end.");
                grid.ClearAllAsteroids();
                break;
        }
    }
}