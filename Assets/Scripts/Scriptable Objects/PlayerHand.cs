using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerHand", menuName = "ScriptableObjects/PlayerHand", order = 1)]
public class PlayerHand : ScriptableObject
{
    public List<Card> cardsInHand = new List<Card>();

}
