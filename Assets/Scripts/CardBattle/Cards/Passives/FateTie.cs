using System;
using System.Collections.Generic;
using System.Linq;
using CardBattle.Card;
using UnityEngine;
using UnityEngine.Events;

namespace CardBattle.Passives {
	public class FateTie : MonoBehaviour {
		private UnityAction<HealthState, HealthState> playerFunction, monsterFunction;
		public MonsterCardBase monster;

		public void Setup(MonsterCardBase monster) {
			this.monster = monster;

			playerFunction = (old, @new) => OnHealthStateChanged(false, old, @new);
			CardGameManager.instance.playerHealthStateChange.AddListener(playerFunction);

			monsterFunction = (old, @new) => OnHealthStateChanged(true, old, @new);
			this.monster.healthStateChanged.AddListener(monsterFunction);
		}
		public void OnDisable() {
			CardGameManager.instance.playerHealthStateChange.RemoveListener(playerFunction);
			monster.healthStateChanged.RemoveListener(monsterFunction);
		}

		// This function will indefinitely get called by its own changes, this variable tracks if we have another instance of the function already running
		private bool alreadyProcessed = false;
		public void OnHealthStateChanged(bool monsterDamaged, HealthState oldState, HealthState newState) {
			if (alreadyProcessed) return;

			alreadyProcessed = true;
			try {
				var delta = newState.health - oldState.health;
				if (delta >= 0) return;
				delta = Mathf.Abs(delta);
				if (monsterDamaged)
					CardGameManager.instance.playerHealthState =
						CardGameManager.instance.playerHealthState.ApplyDamage(delta);
				else
					monster.healthState = monster.healthState.ApplyDamage(delta);
			} finally {
				alreadyProcessed = false;
			}
		}
	}
}