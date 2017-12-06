//]// Author(s): Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;

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

    public EventType GetEventType()
    {
        return eventType;
    }

    public int GetTier()
    {
        return tier;
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
    // Invoked when the event group is started.
    public delegate void StartedHandler(VoidEventGroup.EventGroupType type, int tier);
    public event StartedHandler Started;
    // Invoked when the event group is finished.
    public delegate void FinishedHandler(VoidEventGroup.EventGroupType type, int tier);
    public event FinishedHandler Finished;

    public enum EventGroupType
    {
        None,
        Asteroids,
        Junkyard,
        Radiation,
        Meltdown,
        Overload
    }

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
    // The type of the event group.
    EventGroupType type;
    // The overall tier of the event.
    int tier;

    // Constructor.
    public VoidEventGroup(List<VoidEvent> voidEventsIn, int beginIn, int endIn)
    {
        voidEvents = voidEventsIn;
        begin = beginIn;
        end = endIn;
        tier = voidEventsIn[0].GetTier();

        switch (voidEventsIn.Count)
        {
            case 1:
                switch (voidEventsIn[0].GetEventType())
                {
                    case VoidEvent.EventType.Asteroids:
                        type = EventGroupType.Asteroids;
                        break;
                    case VoidEvent.EventType.Junkyard:
                        type = EventGroupType.Junkyard;
                        break;
                    case VoidEvent.EventType.Radiation:
                        type = EventGroupType.Radiation;
                        break;
                }
                break;

            case 2:
                type = EventGroupType.Meltdown;
                break;

            case 3:
                type = EventGroupType.Overload;
                break;
        }
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
                OnStarted();
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
                OnFinished();
            }
        }
    }

    public EventGroupType GetEventGroupType()
    {
        return type;
    }

    public int GetEventGroupTier()
    {
        return tier;
    }

    public int GetEventGroupBegin()
    {
        return begin;
    }

    public int GetEventGroupEnd()
    {
        return end;
    }

    private void OnStarted()
    {
        if (Started != null)
        {
            Started(type, tier);
        }
    }
    private void OnFinished()
    {
        if (Started != null)
        {
            Finished(type, tier);
        }
    }
}

public class VoidEventController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the canvas instance.")]
    GameObject canvas;
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
    [SerializeField]
    [Tooltip("Reference to the void tuning JSON.")]
    TextAsset tuningJSON;
    [SerializeField]
    [Tooltip("Reference to the event popup window image")]
    GameObject eventPopup;
    [SerializeField]
    [Tooltip("Reference to the you win menu.")]
    UIYouWin youWinMenu;
    [SerializeField]
    [Tooltip("Reference to the event over bar.")]
    GameObject eventOver;
    [SerializeField]
    [Tooltip("Reference to the event over text.")]
    Text textEventOver;
    [SerializeField]
    [Tooltip("Reference to the event slider.")]
    EventSlider eventSlider;

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

    //Number of seconds for the event popup window to stay
    float secondsToStay;

    // This flag is set to true when a Reactor Overload event hits.
    bool eventsNeverEnd = false;

    // Used for the game over screen.
    VoidEventGroup.EventGroupType latestGroupType = VoidEventGroup.EventGroupType.None;
    int latestTier = 0;

    // The biggest end score for any event, presumed to be the end of the game.
    int endScore;

	UILanguages translator;

	void Awake() {
		translator = FindObjectOfType<UILanguages>();
	}

    private void Start()
    {
        // Subscribe to delegate events.
        //scoreCounter.ScoreChanged += ScoreCounter_ScoreChanged;
        grid.SquaresCleared += CheckScore;

        // Don't do event stuff in Zen Mode.
        if (!Settings.Instance.IsZenModeEnabled() &&
            !Settings.Instance.IsTutorialModeEnabled())
        {
            // Read from tuning data and populate event groups.
            Tune();
        }
    }

    private void PrintLetterBindings()
    {
        List<VoidEvent.EventType> orderedEvents = new List<VoidEvent.EventType>();
        orderedEvents.Add(letterBindings['a']);
        orderedEvents.Add(letterBindings['b']);
        orderedEvents.Add(letterBindings['c']);
        string printstr = "The order of events is: ";
        foreach (VoidEvent.EventType type in orderedEvents)
        {
            switch (type)
            {
                case VoidEvent.EventType.Asteroids:
                    printstr += "Asteroids";
                    break;
                case VoidEvent.EventType.Junkyard:
                    printstr += "Junkyard";
                    break;
                case VoidEvent.EventType.Radiation:
                    printstr += "Radiation";
                    break;
            }
            printstr += " -> ";
        }
        Debug.Log(printstr);
    }

    // Read variables from JSON data.
    private void Tune()
    {
        JSONNode json = JSON.Parse(voidEventsJSON.ToString());

        bool randomizeEventOrder = json["event order is randomized"].AsBool;

        if (randomizeEventOrder)
        {
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
        }
        else
        {
            JSONNode orderedEvents = json["event order"];
            string strJunkyard = orderedEvents["junkyard"];
            string strAsteroids = orderedEvents["asteroids"];
            string strRadiation = orderedEvents["radiation"];
            char letterJunkyard = strJunkyard[0];
            char letterAsteroids = strAsteroids[0];
            char letterRadiation = strRadiation[0];
            letterBindings.Add(letterJunkyard, VoidEvent.EventType.Junkyard);
            letterBindings.Add(letterAsteroids, VoidEvent.EventType.Asteroids);
            letterBindings.Add(letterRadiation, VoidEvent.EventType.Radiation);
        }

        PrintLetterBindings();

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

            if (end > endScore)
            {
                endScore = end;
            }

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

            VoidEventGroup newEventGroup = new VoidEventGroup(myEvents, begin, end);
            newEventGroup.Started += VoidEventGroup_Started;
            newEventGroup.Finished += VoidEventGroup_Finished;
            voidEventGroups.Add(newEventGroup);
        }
        JSONNode tuning = JSON.Parse(tuningJSON.ToString());
        secondsToStay = tuning["event warning wait time"].AsFloat;
    }

    // Callback function for checking the score and changing event status accordingly.
    private void CheckScore()
    {
        int newScore = scoreCounter.GetScore();
        foreach (VoidEventGroup eventGroup in voidEventGroups)
        {
            eventGroup.UpdateProgressFromScore(newScore);
        }
    }

    private void VoidEvent_Started(VoidEvent.EventType eventType, int tier)
    {
        TutorialController.Instance.TriggerEvent(TutorialController.Triggers.FIRST_EVENT);
        switch (eventType)
        {
            case VoidEvent.EventType.Junkyard:
                blockSpawner.SetJunkyardTier(tier);
                blockSpawner.BeginJunkyardEvent();
                break;

            case VoidEvent.EventType.Radiation:
                blockSpawner.SetVestigesPerBlock(tierToVestigeCount[tier]);
                blockSpawner.SetVestigeLevel(tierToVestigeLevel[tier]);
                grid.SetBaseEnergyDecayRateBonus(tierToDecayBonus[tier]);
                break;

            case VoidEvent.EventType.Asteroids:
                grid.AddAsteroids(tierToAsteroidCount[tier]);
                break;
        }
    }

    private void VoidEvent_Finished(VoidEvent.EventType eventType, int tier)
    {
        if (eventsNeverEnd)
        {
            // Prevent events from finishing!
            return;
        }
        switch (eventType)
        {
            case VoidEvent.EventType.Junkyard:
                //Debug.Log("Junkyard " + tier + " end.");
                blockSpawner.SetJunkyardTier(0);
                blockSpawner.EndJunkyardEvent();
                break;

            case VoidEvent.EventType.Radiation:
                //Debug.Log("Radiation " + tier + " end.");
                blockSpawner.SetVestigesPerBlock(0);
                grid.SetBaseEnergyDecayRateBonus(0);
                // Don't need to do anything to vestige level.
                break;

            case VoidEvent.EventType.Asteroids:
                //Debug.Log("Asteroids " + tier + " end.");
                grid.ClearAllAsteroids();
                break;
        }
    }

    private void VoidEventGroup_Started(VoidEventGroup.EventGroupType type, int tier)
    {
        latestGroupType = type;
        latestTier = tier;

        AudioController.Instance.StartEventGroup(type);
        eventSlider.SetCurrentState(type);
		EventPopupWindow(GetEventName(type, tier) + translator.Translate(" begin!"));
        switch (type)
        {
            case VoidEventGroup.EventGroupType.Junkyard:
                TutorialController.Instance.TriggerEvent(TutorialController.Triggers.FIRST_URANIUM);
                break;

            case VoidEventGroup.EventGroupType.Radiation:
                TutorialController.Instance.TriggerEvent(TutorialController.Triggers.FIRST_CONTAMINATION);
                break;

            case VoidEventGroup.EventGroupType.Asteroids:
                TutorialController.Instance.TriggerEvent(TutorialController.Triggers.FIRST_BREACH);
                break;

            case VoidEventGroup.EventGroupType.Meltdown:
                TutorialController.Instance.TriggerEvent(TutorialController.Triggers.FIRST_MELTDOWN);
                break;

            case VoidEventGroup.EventGroupType.Overload:
                eventsNeverEnd = true;
                TutorialController.Instance.TriggerEvent(TutorialController.Triggers.FIRST_OVERLOAD);
                break;
        }
    }

    private void VoidEventGroup_Finished(VoidEventGroup.EventGroupType type, int tier)
    {
        if (type == VoidEventGroup.EventGroupType.Overload)
        {
            youWinMenu.gameObject.SetActive(true);
            youWinMenu.Init();
        }
        else
        {
            textEventOver.text = "CRISIS AVERTED: REACTOR RESTORED";
            eventOver.SetActive(true);
            eventSlider.SetCurrentState(VoidEventGroup.EventGroupType.None);
        }
    }

    private void EventPopupWindow(string eventText)
    {
        GameObject eventPopupWindow = Instantiate(eventPopup, canvas.transform, false);
        StartCoroutine(eventPopupWindow.GetComponent<EventPopup>().Translation(eventText, canvas, secondsToStay));
    }

    private string GetEventName(VoidEventGroup.EventGroupType type, int tier)
    {
        string result = "";
        bool levelsImportant = true;
        switch (type)
        {
            case VoidEventGroup.EventGroupType.None:
				result += translator.Translate ("Nothing");
                levelsImportant = false;
                break;

            case VoidEventGroup.EventGroupType.Junkyard:
				result += translator.Translate ("Unrefined Uranium");
                break;

            case VoidEventGroup.EventGroupType.Radiation:
				result += translator.Translate ("Waste Contamination");
                break;

            case VoidEventGroup.EventGroupType.Asteroids:
				result += translator.Translate ("Reactor Breach");
                break;

            case VoidEventGroup.EventGroupType.Meltdown:
				result += translator.Translate ("Reactor Meltdown");
                levelsImportant = false;
                break;

            case VoidEventGroup.EventGroupType.Overload:
				result += translator.Translate ("Reactor Overload");
                levelsImportant = false;
                break;
        }
        if (levelsImportant)
        {
            result += " Lv. " + tier;
        }
        return result;
    }

    public string GetLatestEventName()
    {
        return GetEventName(latestGroupType, latestTier);
    }

    // Get how close the player is to reaching the end of the game, as a percentage.
    public float GetProgress()
    {
        return GetProgress(scoreCounter.GetScore());
    }
    public float GetProgress(int score)
    {
        float percentage = (float)score / endScore;
        if (percentage > 1.0f)
        {
            percentage = 1.0f;
        }
        return percentage;
    }

    public List<VoidEventGroup> GetVoidEventGroups()
    {
        return voidEventGroups;
    }
}