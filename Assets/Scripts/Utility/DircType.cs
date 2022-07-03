using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DircTypes
{
    ToLobby = 0,
    ToFacility,
    ToDoctor,
}

public class DircType : MonoBehaviour
{
    [SerializeField] private DircTypes type;
    public DircTypes Type { get => type; }
}