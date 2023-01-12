using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "Scriptables")]
public class UnitOriginDB : ScriptableObject
{
	public List<UnitDataSO> HumalDBList;
	public List<UnitDataSO> EnemyDBList;
}
