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

    [SerializeField]
    [Tooltip("The overlay image to use for raycasting.")]
    Image overlayImage;

    public enum Triggers
    {
        FIRST_OPEN,
        FIRST_OPEN_2,
        FIRST_BLOCK,
        FIRST_SQUARE,
        FIRST_WASTE,
        FIRST_WASTE_2,
        FIRST_WASTE_3,
        FIRST_LOW_ENERGY,
        FIRST_EVENT,
        FIRST_BREACH,
        FIRST_URANIUM,
        FIRST_CONTAMINATION,
        FIRST_MELTDOWN,
        FIRST_OVERLOAD
    }

    [Serializable]
    public class TriggerData
    {
        public Triggers trigger;
        public string textInfo;
        public int panelNumber;

        public TriggerData(string trig, string text, int panel)
        {
            Triggers parsed_enum = (Triggers) Enum.Parse(typeof(Triggers), trig);

            trigger = parsed_enum;
            textInfo = text;
            panelNumber = panel;
        }
    }

    public List<TriggerData> triggerData = new List<TriggerData>();
    Dictionary<Triggers, bool> triggerRecord = new Dictionary<Triggers, bool>();

    public Grid grid;

    public float tapOffset = 0.5f;

    float tapCountdown;
    bool overlayDone;
    bool currentlyPlaying;

    public TextAsset tutorialJSON;

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
            string textInfo = trigger["TextInfo"];
            int panelNumber = trigger["PanelNumber"].AsInt;
            
            triggerData.Add(new TriggerData(triggerName, textInfo, panelNumber));
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
    }

    private void Start()
    {
        TriggerEvent(Triggers.FIRST_OPEN);
        TriggerEvent(Triggers.FIRST_OPEN_2);

        grid.SquareFormed += OnSquare;
    }

    public void TriggerEvent(Triggers trigger)
    {
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


        string textInfo = triggerData.Find((x) => x.trigger == trigger).textInfo;
        if (textInfo == null)
        {
            Debug.LogError("No text data found for Trigger!");
        }

        int panelNumber = triggerData.Find((x) => x.trigger == trigger).panelNumber;

        if (Panels[panelNumber] == null)
        {
            Debug.LogError("No panel found for Trigger!");
        }

        if (Panels[panelNumber] != null && textInfo != null)
        {
            PanelTexts[panelNumber].text = textInfo;
            ActivatePanel(trigger, panelNumber);
        }
    }

    public void ActivatePanel(Triggers trigger, int panelNumber)
    {
        StartCoroutine(OpenPanel(trigger, panelNumber));
    }

    IEnumerator OpenPanel(Triggers trigger, int panelNumber)
    {
        currentlyPlaying = true;

        if (Panels[panelNumber] != null)
        {
            Panels[panelNumber].gameObject.SetActive(true);
        }

        overlayDone = false;
        overlayImage.enabled = true;
        overlayImage.raycastTarget = true;
        
        while (!overlayDone)
        {
            tapCountdown += Time.deltaTime;

            yield return null;
        }

        tapCountdown = 0f;
        currentlyPlaying = false;
        overlayImage.enabled = false;

        if (Panels[panelNumber] != null)
        {
            Panels[panelNumber].gameObject.SetActive(false);
        }

        if (nextTriggers.Count > 0)
        {
            Triggers next = nextTriggers[0];
            nextTriggers.Remove(next);
            TriggerEvent(next);
        }
    }

    //Call this instead of TriggerEvent if you want the panel to move to a block location
    public void PanelToBlockLocation(int row, int col, Triggers trigger)
    {
        
        string textInfo = triggerData.Find((x) => x.trigger == trigger).textInfo;
        if (textInfo == null)
        {
            Debug.LogError("No text data found for Trigger!");
        }

        int panelNumber = triggerData.Find((x) => x.trigger == trigger).panelNumber;

        if (Panels[panelNumber] == null)
        {
            Debug.LogError("No panel found for Trigger!");
        }

        float xPos;
        float yPos;

        if (row < grid.GetWidth() / 2)
        {
            xPos = grid.GetTilePosition(row, col).x + offset;
        }
        else
        {
            xPos = grid.GetTilePosition(row, col).x - offset;
        }

        if (col < grid.GetHeight() / 2)
        {
            yPos = grid.GetTilePosition(row, col).y - offset;
        }
        else
        {
            yPos = grid.GetTilePosition(row, col).y + offset;
        }

        Panels[panelNumber].transform.localPosition = new Vector2(xPos, yPos);

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

        if (Panels[panelNumber] != null && textInfo != null)
        {
            PanelTexts[panelNumber].text = textInfo;
            ActivatePanel(trigger, panelNumber);
        }
    }

    private void OnSquare(int size, Vector3 textPos)
    {
        TriggerEvent(Triggers.FIRST_SQUARE);
    }
    
    private void OnDestroy()
    {
        grid.SquareFormed -= OnSquare;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (tapCountdown >= tapOffset)
        {
            overlayDone = true;
            overlayImage.raycastTarget = false;
        }
    }
}
