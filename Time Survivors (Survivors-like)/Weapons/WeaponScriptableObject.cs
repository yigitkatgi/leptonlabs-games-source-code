using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    /// <summary>
    /// We create getters and setters to declare these as properties and not variables so that nothing is written onto these variables in runtime 
    /// We do this because scriptable objects variables may be changed during runtime (play) and will be saved as so afterwards.
    /// (if something tries this, error message will be displayed)
    /// </summary>
    [SerializeField]
    GameObject prefab;
    public GameObject Prefab { get => prefab; private set => prefab = value; }

    // Base stats for all weapons
    [SerializeField]
    float damage;
    public float Damage { get => damage; private set => damage = value; }

    [SerializeField]
    float speed;
    public float Speed { get => speed; private set => speed = value; }

    [SerializeField]
    float cooldownDuration;
    public float CooldownDuration { get => cooldownDuration; private set => cooldownDuration = value; }

    [SerializeField]
    int pierce;
    public int Pierce { get => pierce; private set => pierce = value; }

    [SerializeField]
    int level; // Not meant to be modified in game only in editor
    public int Level { get => level; private set => level = value; }

    [SerializeField]
    GameObject nextLevelPrefab; // The prefab of the next level ie what the object becomes when it levels up
    public GameObject NextLevelPrefab {  get => nextLevelPrefab; private set => nextLevelPrefab = value; }    
    
    [SerializeField]
    new string name;
    public string Name {  get => name; private set => name = value; }    
    
    [SerializeField]
    string description;
    public string Description {  get => description; private set => description = value; }

    [SerializeField]
    Sprite icon; // Not meant to be modified in game only in editor
    public Sprite Icon { get => icon; private set => icon = value; }


}
