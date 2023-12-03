using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent
{
    string HeaderName { get; }
    string Description { get; }
    void OnShow();
    void OnAccept();
    void OnCancel();
    void OnReset();
    bool TriggerConditions();
}
