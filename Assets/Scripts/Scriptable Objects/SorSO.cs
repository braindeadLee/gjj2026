using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SorSO", menuName = "Scriptable Objects/SorSO")]
public class SorSO : ItemSO
{
    public AttributeSO[] color;
    public AttributeSO[] quality;
    public AttributeSO[] theme;
}
