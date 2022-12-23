using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Let's have a 'build a tower' section in the game, that would be cool.
[CreateAssetMenu(fileName = "BuildingData", menuName = "Building")]
public class BuildingData : ScriptableObject //buildingdata for basic tower type.
{
    public Sprite sprite;
    public float maxHealth; //most likely this stat will never be used.
    public float range;
    public float damage;
    public float attackSpeed; //time between attacks.
    public float attackAnimationSpeed; //duration of attack animation. Should not exceed attack speed.

    //todo: projectile type information.
    //aoe, projectile, beam..? line aoe, bounce?


    //todo: let's add these as 'attackModifiers'
    //public float coneAoe; //degrees radial from center.
    //public float splash; //circle around contact.
    //public float lineSplash; //length of line behind target position.
    //public float bounces;



}
