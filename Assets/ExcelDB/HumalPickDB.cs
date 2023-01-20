using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "Scriptables")]
public class HumalPickDB : ScriptableObject
{
	public List<HumalPickDBEntity> Entities;
}
