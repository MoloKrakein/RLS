using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// make scriptable object
[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public int damage;
    public float attackSpeed;
    public float range;
    public Sprite weaponSprite;
}
