using UnityEngine;
using System.Collections;
using System.Drawing.Text;
using CardBattle.Containers;

namespace CardBattle
{

    /// <summary>
    /// Flame attack, which damages the target and
    /// shuffles a Burn into the deck; also has a 
    /// chance to rotate the ship
    /// </summary>
    /// <author>Misha Desear</author>
    public class Flame : Card.ActionCardBase
    {
        public GameObject rotatorPrefab;
        private int rotateChance;

        // Can only target monsters and equipment
        public override CardFilterer.CardFilters TargetingFilters =>
            ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay/*| CardFilterer.CardFilters.Equipment*/);
        public override CardFilterer.CardFilters MonsterTargetingFilters =>
            TargetingFilters | CardFilterer.CardFilters.Monster;

        /// <summary>
        /// When the player targets something, damage it (if it exists) and then send this card to the graveyard
        /// </summary>
        public override void OnTarget(Card.CardBase _target)
        {
            var target = _target?.GetComponent<Card.HealthCardBase>();
            if (NullAndPlayerCheck(target)) return; // Make sure the target isn't null if owned by the player

            // Damage target (falling back to player if we are monster and not targeting anything!)
            DamageTargetOrPlayer(properties["primary"], target);

            // RNG to determine if the ship rotates after using this card
            rotateChance = Random.Range(1, 10);

            if (OwnedByPlayer) 
            {
                CardGameManager.instance.playerDeck.cardDB.Instantiate("Burn");
            }

            if (rotateChance == 1)
            {
                StartCoroutine(RotateNextFrame());

                IEnumerator RotateNextFrame()
                {
                    yield return null;
                    var angle = Mathf.Round(Random.Range(0f, 360f) / 30) * 30;
                    CardGameManager.instance.ship.transform.rotation = Quaternion.Euler(0, angle, 0);
                }

            }
            SendToGraveyard();
        }
    }
}