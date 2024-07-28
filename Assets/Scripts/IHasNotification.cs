using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasNotification
{
    public event EventHandler<OnNotificationAddedEventArgs> OnNotificationAdd;
    public class OnNotificationAddedEventArgs : EventArgs {
        public string notification;
    }
}
