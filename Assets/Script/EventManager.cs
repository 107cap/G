using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    [SerializeField] UnityEvent JoinGame;
    [SerializeField] UnityEvent ExitGame;
    [SerializeField] UnityEvent StartRace;
    [SerializeField] UnityEvent EndRace;
    [SerializeField] UnityEvent AddPlayer;

    public Dictionary<EventType, UnityEvent> eventDict;

    private void Awake()
    {
        //TODO - Dict 값 자동 설정으로 변경
        eventDict = new Dictionary<EventType, UnityEvent>()
        {
            {EventType.JOIN_GAME, JoinGame },
            //{EventType.EXIT_GAME, ExitGame },
            {EventType.START_RACE, StartRace },
            {EventType.END_RACE, EndRace },
            {EventType.ADD_PLAYER, AddPlayer }
        };
    }

    public void Register(EventType type, UnityAction listener) => eventDict[type].AddListener(listener);

    public void UnRegister(EventType type, UnityAction listener) => eventDict[type].RemoveListener(listener);

    public void Invoke(EventType type) => eventDict[type].Invoke();
}
