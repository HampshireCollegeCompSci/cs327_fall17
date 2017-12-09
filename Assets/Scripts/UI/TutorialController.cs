// Author(s): Joel Esquilin, Paul Calande

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SimpleJSON;

public class TutorialController : MonoBehaviour, IPointerDownHandler {

    public List<Image> Panels = new List<Image>();
    public List<Text> PanelTexts = new List<Text>();
    public float offset = 100f;

    public enum Triggers
    {
        FIRST_OPEN,
        //FIRST_OPEN_2,
        FIRST_BLOCK,
        FIRST_SQUARE,
        FIRST_SQUARE_NOT_TUTORIAL,
        FIRST_WASTE,
        FIRST_WASTE_2,
        FIRST_WASTE_3,
        FIRST_LOW_ENERGY,
        FIRST_EVENT,
        FIRST_BREACH,
        FIRST_URANIUM,
        FIRST_CONTAMINATION,
        FIRST_MELTDOWN,
        FIRST_OVERLOAD,
        TUTORIAL_BAG_TRIGGER_1,
        TUTORIAL_BAG_TRIGGER_2,
        TUTORIAL_BAG_TRIGGER_3,
        TUTORIAL_BAG_TRIGGER_4,
        TUTORIAL_BAG_TRIGGER_5
    }

    [Serializable]
    public class TriggerData
    {
        public class InfoBite
        {
            public string textInfo;
            public int panelNumber;

            public InfoBite(string textInfoIn, int panelNumberIn)
            {
                textInfo = textInfoIn;
                panelNumber = panelNumberIn;
            }
        }

        public Triggers trigger;
        public List<InfoBite> infoBites;

        public TriggerData(string trig, List<InfoBite> infoBitesIn)
        {
            Triggers parsed_enum = (Triggers) Enum.Parse(typeof(Triggers), trig);

            trigger = parsed_enum;
            infoBites = infoBitesIn;
        }
    }

    [SerializeField]
    [Tooltip("Reference to the end tutorial window.")]
    GameObject endTutorialWindow;

    public List<TriggerData> triggerData = new List<TriggerData>();
    Dictionary<Triggers, bool> triggerRecord = new Dictionary<Triggers, bool>();
    List<Triggers> triggersTemporarilyEnabled = new List<Triggers>();

    public Grid grid;

    public float tapOffset = 0.5f;

    public List<Image> masks = new List<Image>();

    float tapCountdown;
    bool overlayDone;
    bool currentlyPlaying;

    public TextAsset tutorialJSON;

	UILanguages translator;

    List<Triggers> nextTriggers = new List<Triggers>();

    private static TutorialController instance = null;
    public static TutorialController Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        //DontDestroyOnLoad(gameObject);
        
        //Do JSON reading here and setup triggerData text here
        JSONNode triggers = JSONNode.Parse(tutorialJSON.ToString());

        for (int i = 0; i < triggers.Count; ++i)
        {
            JSONNode trigger = triggers[i];
            string triggerName = trigger["TriggerName"];

            List<TriggerData.InfoBite> infoBites = new List<TriggerData.InfoBite>();

            JSONArray infoBitesArray = trigger["Info"].AsArray;
            for (int j = 0; j < infoBitesArray.Count; ++j)
            {
                JSONNode infoBite = infoBitesArray[j];
                string textInfo = infoBite["TextInfo"];
                string panelStr = infoBite["Panel"];
                int panelNumber = PanelStringToInt(panelStr);
                infoBites.Add(new TriggerData.InfoBite(textInfo, panelNumber));
            }

            triggerData.Add(new TriggerData(triggerName, infoBites));
        }

        foreach (Triggers trigger in Enum.GetValues(typeof(Triggers)))
        {
            //Load Triggers from PlayerPrefs
            if (PlayerPrefs.HasKey(trigger.ToString()))
            {
                if (PlayerPrefs.GetInt(trigger.ToString()) == 0)
                {
                    triggerRecord.Add(trigger, false);
                }
                else
                {
                    triggerRecord.Add(trigger, true);
                }
            }
            else
            {
                PlayerPrefs.SetInt(trigger.ToString(), 0);
                triggerRecord.Add(trigger, false);
            }
        }
		translator = FindObjectOfType<UILanguages>();

        StartingTutorialStuff();
    }

    private void StartingTutorialStuff()
    {
        //Debug.Log(Settings.Instance.GetTutorialComplete());
        if (Settings.Instance.GetTutorialComplete())
        {
            if (Settings.Instance.IsTutorialModeEnabled())
            {
                TemporarilyEnableTriggers();
            }
        }
        else
        {
            Settings.Instance.SetTutorialModeEnabled(true);
            TemporarilyEnableTriggers();
        }

        TriggerEvent(Triggers.FIRST_OPEN);
        //TriggerEvent(Triggers.FIRST_OPEN_2);
    }

    private void Start()
    {
        //StartingTutorialStuff();

        grid.SquaresCleared += OnSquare;

        endTutorialWindow.SetActive(false);
    }

    public void TriggerEvent(string trigger)
    {
        // Convert the string to an enum.
        TriggerEvent((Triggers)Enum.Parse(typeof(Triggers), trigger));
    }

    public void TriggerEvent(Triggers trigger)
    {
        // If the trigger doesn't exist, get outta here.
        if (triggerData.Find(x => x.trigger == trigger) == null)
        {
            return;
        }

        if (currentlyPlaying)
        {
            if (!nextTriggers.Contains(trigger))
            {
                nextTriggers.Add(trigger);
                return;
            }
            else
            {
                return;
            }
        }

        //if already played do not trigger
        if (triggerRecord[trigger])
        {
            return;
        }

        triggerRecord[trigger] = true;
        PlayerPrefs.SetInt(trigger.ToString(), 1);

        TriggerData thisTrigger = triggerData.Find((x) => x.trigger == trigger);

        StartCoroutine(OpenPanels(thisTrigger));

        /*
        string textInfo = thisTrigger.textInfo;

        if (textInfo == null)
        {
            Debug.LogError("No text data found for Trigger!");
        }

        int panelNumber = thisTrigger.panelNumber;

        if (Panels[panelNumber] == null)
        {
            Debug.LogError("No panel found for Trigger!");
        }

        if (Panels[panelNumber] != null && textInfo != null)
        {
            PanelTexts[panelNumber].text = textInfo;
            ActivatePanel(trigger, panelNumber);
        }
        */
    }

    IEnumerator OpenPanels(TriggerData data)
    {
        List<TriggerData.InfoBite> infoBites = data.infoBites;

        for (int i = 0; i < infoBites.Count; ++i)
        {
            TriggerData.InfoBite infoBite = infoBites[i];
			string text = translator.Translate (infoBite.textInfo);
            int panel = infoBite.panelNumber;
		
            ActivatePanel(panel, text);

            while (currentlyPlaying)
            {
                yield return null;
            }
        }

        // Now that we're done, move onto the next trigger if possible.
        if (nextTriggers.Count > 0)
        {
            Triggers next = nextTriggers[0];
            nextTriggers.Remove(next);
            TriggerEvent(next);
        }
    }

    public void ActivatePanel(int panelNumber, string text)
    {
        StartCoroutine(OpenPanel(panelNumber, text));
    }

    IEnumerator OpenPanel(int panelNumber, string text)
    {
        currentlyPlaying = true;

        if (Panels[panelNumber] != null)
        {
            PanelTexts[panelNumber].text = text;
            Panels[panelNumber].gameObject.SetActive(true);
        }

        if (masks[panelNumber] != null)
        {
            masks[panelNumber].gameObject.SetActive(true);
        }

        overlayDone = false;
        
        while (!overlayDone)
        {
            tapCountdown += Time.deltaTime;

            yield return null;
        }

        if (Panels[panelNumber] != null)
        {
            Panels[panelNumber].gameObject.SetActive(false);
        }

        if (masks[panelNumber] != null)
        {
            masks[panelNumber].gameObject.SetActive(false);
        }

        tapCountdown = 0f;
        currentlyPlaying = false;
    }

    public void MovePanelToBlockLocation(int panelNumber, int row, int col)
    {
        float xPos;
        float yPos;

        if (row < grid.GetWidth() / 2 - 1)
        {
            xPos = -offset;
        }
        else
        {
            xPos = offset;
        }

        if (col > grid.GetHeight() / 2)
        {
            yPos = offset * 3.5f;
        }
        else
        {
            yPos = 0;
        }

        Panels[panelNumber].transform.localPosition = new Vector2(xPos, yPos);
    }

    private int PanelStringToInt(string panelString)
    {
        switch (panelString)
        {
            case "Grid":
                return 0;
            case "Console":
                return 1;
            case "Reactor":
                return 2;
            case "ScoreBar":
                return 3;
            case "Waste":
                return 4;
            default:
                Debug.LogError("PanelStringToInt: Couldn't find panel with name " + panelString + "!");
                return 5;
        }
    }

    public void EndTutorialMode()
    {
        endTutorialWindow.SetActive(true);
        DisableTemporaryTriggers();
        Settings.Instance.SetTutorialComplete();
    }

    // Re-enable triggers for when the player replays tutorial mode.
    public void TemporarilyEnableTriggers()
    {
        foreach (var pair in triggerRecord)
        {
            if (pair.Value == true)
            {
                Triggers trig = pair.Key;
                //triggerRecord[trig] = false;
                triggersTemporarilyEnabled.Add(trig);
            }
        }
        foreach (Triggers trig in triggersTemporarilyEnabled)
        {
            triggerRecord[trig] = false;
        }
    }
    // Disable the temporarily re-enabled triggers.
    public void DisableTemporaryTriggers()
    {
        //Debug.Log("TutorialController.DisableTemporaryTriggers");
        foreach (Triggers trig in triggersTemporarilyEnabled)
        {
            //Debug.Log("Disabling trigger: " + trig);
            triggerRecord[trig] = true;
        }
        triggersTemporarilyEnabled.Clear();
        /*
        if (Settings.Instance.GetTutorialComplete())
        {
            Settings.Instance.SetTutorialComplete();
        }
        */
    }

    private void OnDestroy()
    {
        DisableTemporaryTriggers();
        grid.SquaresCleared -= OnSquare;
        Settings.Instance.SetTutorialModeEnabled(false);
    }

    private void OnSquare()
    {
        if (Settings.Instance.IsTutorialModeEnabled())
        {
            TriggerEvent(Triggers.FIRST_SQUARE);
        }
        else
        {
            TriggerEvent(Triggers.FIRST_SQUARE_NOT_TUTORIAL);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (tapCountdown >= tapOffset)
        {
            overlayDone = true;
        }
    }
}