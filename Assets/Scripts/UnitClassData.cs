﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitClassData : MonoBehaviour
{
    public List<UnitClass> UnitClasses;
    public List<GrowthsStruct> UnitGrowths;
    public List<GrowthsStruct> ClassGrowths;
    public List<ClassAnimation> ClassAnimations;
}

[System.Serializable]
public class GrowthsStruct
{
    public string Name;
    [Header("Growths (STR, END, PIR, ARM, PRE, EVA)")]
    public int[] Growths = new int[6];
}

[System.Serializable]
public class UnitClass
{
    public string Unit;
    public string Class;
}