using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "Scriptables")]
public class UnitOriginDB : ScriptableObject
{
	public List<UnitData> humalDBList;
	public List<UnitData> enemyDBList;
}
