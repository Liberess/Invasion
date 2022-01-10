using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DircTypes
{
    Up = 0,
    Left,
    Down,
    Right
}

public class DircType : MonoBehaviour
{
    [SerializeField] private DircTypes type;
    public DircTypes Type { get => type; }
}