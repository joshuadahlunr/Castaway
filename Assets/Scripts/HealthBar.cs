using System;
using UnityEngine;

public class HealthBar : BurningRope {
	protected new void Update() {
		base.Update();
		
		text.text = $"{(int)current} HP";
	}
}
