using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Build Definition", menuName = "Combat/Player Build Definition", order = 2)]
public class PlayerBuildDefinition : ScriptableObject
{
    //Eventually replace these with stats

    [Header("Stats")]
    [Tooltip("The maximum energy a player can have")]
    [Range(4f, 6f)]
    public float MaxEnergy; //STAMINA?

    [Tooltip("Multiplies the base rate at which a player recovers spent energy")]
    [Range(1f, 2f)]
    public float EnergyRecoveryRate; //STRENGTH?

    [Tooltip("Multiplies the base rate at which a player recovers lost balance")]
    [Range(1f, 3f)]
    public float BalanceRecoveryRate; //AGILITY?

    [Header("Abilities")]
    [Tooltip("The player's primary attack")]
    public AttackDefinition PrimaryAttack;
    
    [Tooltip("The player's follow-up attack")]
    public AttackDefinition SecondaryAttack;

    [Tooltip("The player's finisher attack")]
    public AttackDefinition TertiaryAttack;

    [Tooltip("The player's dodge")]
    public DodgeDefinition Dodge;

    [Tooltip("The player's block")]
    public BlockDefinition Block;
}
