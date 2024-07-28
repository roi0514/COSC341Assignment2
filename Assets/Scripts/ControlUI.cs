using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUI : MonoBehaviour
{
    [SerializeField] private GameObject mainScript;
    [SerializeField] private Transform container;
    [SerializeField] private Transform notificationTemplate;
    private List<string> notificationsList = new List<string>();

    private IHasNotification hasNotification;

    private void Awake() {
        hasNotification = mainScript.GetComponent<IHasNotification>();

        hasNotification.OnNotificationAdd += HasNotification_OnNotificationAdd;
    }

    private void HasNotification_OnNotificationAdd(object sender, IHasNotification.OnNotificationAddedEventArgs e) {
        UpdateVisual(e);
    }

    private void UpdateVisual(IHasNotification.OnNotificationAddedEventArgs e) {
        foreach (Transform child in container) {
            if (child == notificationTemplate) continue;
            Destroy(child.gameObject);
        }
        notificationsList.Add(e.notification);

        foreach(string notifications in notificationsList) {
            notificationTemplate.gameObject.SetActive(true);
            notificationTemplate.GetComponent<ControlUISingle>().SetNotification(notifications);
        }


    }
}
