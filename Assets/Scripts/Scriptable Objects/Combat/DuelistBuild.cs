using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Duelist Build Definition", menuName = "Combat/Duelist Build Definition", order = 2)]
public class DuelistBuild : ScriptableObject
{
    //Eventually replace these with stats

    [Header("Stats")]
    [Tooltip("The maximum energy a duelist can have")]
    [Range(4f, 6f)]
    public float MaxEnergy; //STAMINA?

    [Tooltip("Multiplies the base rate at which a duelist recovers spent energy")]
    [Range(1f, 2f)]
    public float EnergyRecoveryRate; //STRENGTH?

    [Tooltip("Multiplies the base rate at which a duelist recovers lost balance")]
    [Range(1f, 3f)]
    public float BalanceRecoveryRate; //AGILITY?

    [Header("Abilities")]
    [Tooltip("The duelist's primary attack")]
    public AttackDefinition PrimaryAttack;
    
    [Tooltip("The duelist's follow-up attack")]
    public AttackDefinition SecondaryAttack;

    [Tooltip("The duelist's finisher attack")]
    public AttackDefinition TertiaryAttack;

    [Tooltip("The duelist's dodge")]
    public DodgeDefinition Dodge;

    [Tooltip("The duelist's block")]
    public BlockDefinition Block;
}
