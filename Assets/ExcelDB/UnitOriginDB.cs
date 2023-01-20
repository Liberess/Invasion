using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "Scriptables")]
public class UnitOriginDB : ScriptableObject
{
	public List<UnitData> HumalDBList;
	public List<UnitData> EnemyDBList;
}
