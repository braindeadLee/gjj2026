using System.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "Attribute", menuName = "Scriptable Objects/Attribute")]
public class AttributeSO : ScriptableObject
{
    public RuleType category;
    public string displayName;
}
