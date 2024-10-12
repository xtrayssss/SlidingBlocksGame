using UnityEngine;
using UnityEngine.UI;

public class HorizontalScrollSnap : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform contentPanel;
    public RectTransform[] elements;
    public float snapSpeed = 10f;
    public float snapThreshold = 0.5f;

    private int currentIndex = 0;
    private Vector2 contentVector;

    void Start()
    {
        contentVector = contentPanel.anchoredPosition;
    }

    void Update()
    {
        float scrollPercentage = scrollRect.horizontalNormalizedPosition;
        int nearestIndex = Mathf.RoundToInt(scrollPercentage * (elements.Length - 1));

        if (nearestIndex != currentIndex && Mathf.Abs(scrollPercentage - (float)nearestIndex / (elements.Length - 1)) < snapThreshold)
        {
            SnapToElement(nearestIndex);
        }
    }

    void SnapToElement(int index)
    {
        currentIndex = index;
        float targetX = -(elements[index].anchoredPosition.x);
        contentVector.x = Mathf.Lerp(contentPanel.anchoredPosition.x, targetX, snapSpeed * Time.deltaTime);
        contentPanel.anchoredPosition = contentVector;
    }
}
