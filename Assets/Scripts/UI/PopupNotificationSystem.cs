using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupNotificationSystem : MonoBehaviour
{
    public static PopupNotificationSystem Instance { get; private set; }

    public GameObject popupPrefab;
    public Transform notificationParent;
    public float notificationDuration = 3f;
    public int maxNotifications = 5;
    public float notificationSpacing = 70f; 

    private List<GameObject> notifications = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowNotification(string itemName, Sprite itemSprite)
    {
        if (notifications.Count >= maxNotifications)
        {
            Destroy(notifications[0]);
            notifications.RemoveAt(0);
        }

        float yOffset = -notifications.Count * notificationSpacing;

        GameObject popupInstance = Instantiate(popupPrefab, notificationParent);
        RectTransform popupRectTransform = popupInstance.GetComponent<RectTransform>();
        popupRectTransform.anchoredPosition = new Vector2(popupRectTransform.anchoredPosition.x, yOffset);

        TextMeshProUGUI itemNameText = popupInstance.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        if (itemNameText != null)
        {
            itemNameText.text = itemName + " x1";
        }

        Image itemIconImage = popupInstance.transform.Find("ItemIcon").GetComponent<Image>();
        if (itemIconImage != null)
        {
            itemIconImage.sprite = itemSprite;
        }

        notifications.Add(popupInstance);
        StartCoroutine(FadeAndDestroy(popupInstance));
    }

    private IEnumerator FadeAndDestroy(GameObject notification)
    {
        float elapsedTime = 0f;

        CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = notification.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1;

        while (elapsedTime < notificationDuration)
        {
            if (notification == null)
            {
                yield break;
            }

            float t = elapsedTime / notificationDuration;
            canvasGroup.alpha = 1 - t;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (notification != null)
        {
            notifications.Remove(notification);
            Destroy(notification);
        }
    }
}
