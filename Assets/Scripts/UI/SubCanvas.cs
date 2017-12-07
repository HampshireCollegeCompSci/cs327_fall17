using UnityEngine;

public class SubCanvas : MonoBehaviour {

    [SerializeField]
    [Tooltip("Reference to the grid object")]
    GameObject grid;
    [SerializeField]
    [Tooltip("Referebce to the canvas object")]
    GameObject canvas;
    [SerializeField]
    [Tooltip("Reference to the top bar object")]
    GameObject topBar;
    [SerializeField]
    [Tooltip("Reference to the energy container object")]
    GameObject energyContainer;
    [SerializeField]
    [Tooltip("Reference to the console object")]
    GameObject console;
    [SerializeField]
    [Tooltip("Reference to the background pipes object")]
    GameObject pipes;
    [SerializeField]
    [Tooltip("Reference to the background image object")]
    GameObject background;
    [SerializeField]
    [Tooltip("Reference to the black bar prefab")]
    GameObject blackBarPrefab;

    RectTransform rectTransform;

    public void WideScreenSupport()
    {
        rectTransform = GetComponent<RectTransform>();
        //Super wide screen (such as iPhone X, Galaxy S8) suport.
        RectTransform gridRT = grid.GetComponent<RectTransform>();
        float originalY = gridRT.rect.height * 1.1f;
        float size = canvas.GetComponent<RectTransform>().rect.width / 1.1f;

        //Adjust top bar
        RectTransform topbarRT = topBar.GetComponent<RectTransform>();
        topbarRT.sizeDelta = new Vector2(rectTransform.rect.width, topbarRT.rect.height);

        if (gridRT.rect.width > size)
        {
            //Adjust Grid size 
            gridRT.sizeDelta = new Vector2(size, size);

            //Calcluate move length
            float newY = gridRT.rect.height * 1.1f;
            float diff = (originalY - newY) / 2;
            float diffRatio = diff / canvas.GetComponent<RectTransform>().rect.height;

            //Resize sub-canvas
            rectTransform.anchorMax = new Vector2(1, 1 - diffRatio);
            rectTransform.anchorMin = new Vector2(0, diffRatio);

            //Move grid up
            gridRT.anchoredPosition = new Vector2(gridRT.anchoredPosition.x, gridRT.anchoredPosition.y + diff * (1 - gridRT.anchorMax.y) / 1.1f);

            //topbarRT.anchorMax = new Vector2(topbarRT.anchorMax.x, topbarRT.anchorMax.y - diffRatio);
            //topbarRT.anchorMin = new Vector2(topbarRT.anchorMin.x, topbarRT.anchorMin.y - diffRatio);

            //Move energy container, console and background pipes up
            RectTransform ecRT = energyContainer.GetComponent<RectTransform>();
            ecRT.anchoredPosition = new Vector2(ecRT.anchoredPosition.x, ecRT.anchoredPosition.y + diff);

            RectTransform consoleRT = console.GetComponent<RectTransform>();
            consoleRT.anchoredPosition = new Vector2(consoleRT.anchoredPosition.x, consoleRT.anchoredPosition.y + diff);

            RectTransform pipeRT = pipes.GetComponent<RectTransform>();
            pipeRT.anchoredPosition = new Vector2(pipeRT.anchoredPosition.x, pipeRT.anchoredPosition.y + diff);

            //Add black bars to cover the screen top and bottom
            GameObject blackBarObjTop = Instantiate(blackBarPrefab);
            blackBarObjTop.transform.SetParent(canvas.transform);
            RectTransform btRT = blackBarObjTop.GetComponent<RectTransform>();
            btRT.localScale = new Vector3(1, 1, 1);
            btRT.anchorMin = new Vector2(0, 0);
            btRT.anchorMax = new Vector2(1, diffRatio);
            btRT.offsetMin = new Vector2(0, 0);
            btRT.offsetMax = new Vector2(0, 0);

            GameObject blackBarObjBottom = Instantiate(blackBarPrefab);
            blackBarObjBottom.transform.SetParent(canvas.transform);
            RectTransform bbRT = blackBarObjBottom.GetComponent<RectTransform>();
            bbRT.localScale = new Vector3(1, 1, 1);
            bbRT.anchorMin = new Vector2(0, 1 - diffRatio);
            bbRT.anchorMax = new Vector2(1, 1);
            bbRT.offsetMin = new Vector2(0, 0);
            bbRT.offsetMax = new Vector2(0, 0);
            
        }
    }
}
