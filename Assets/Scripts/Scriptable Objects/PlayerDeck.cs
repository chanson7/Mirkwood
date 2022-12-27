using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerDeck", menuName = "ScriptableObjects/PlayerDeck", order = 1)]
public class PlayerDeck : ScriptableObject
{
    public List<Card> cardsInDeck = new List<Card>();

}
