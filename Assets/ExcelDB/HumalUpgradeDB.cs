using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "Scriptables")]
public class HumalUpgradeDB : ScriptableObject
{
	public List<HumalUpgradeLevelEntity> Level;
	public List<HumalUpgradeGradeEntity> Grade;
}
