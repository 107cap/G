using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    [SerializeField] UnityEvent JoinGame;
    [SerializeField] UnityEvent ExitGame;
    [SerializeField] UnityEvent StartRace;
    [SerializeField] UnityEvent EndRace;

    public Dictionary<EventType, UnityEvent> eventDict;

    private void Awake()
    {
        //TODO - Dict �� �ڵ� �������� ����
        eventDict = new Dictionary<EventType, UnityEvent>()
        {
            //{EventType.JOIN_GAME, JoinGame },
            //{EventType.EXIT_GAME, ExitGame },
            {EventType.START_RACE, StartRace },
            {EventType.END_RACE, EndRace }
        };
    }
}
