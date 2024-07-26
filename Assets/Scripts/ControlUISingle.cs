using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlUISingle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notificationText;

    public void SetNotification(string notification) {
        notificationText.text = notification;
    }


}
