
/// <summary>
/// Health bars are currently a burning rope that say HP instead of Secs
/// </summary>
/// <author>Joshua Dahl</author>
// TODO: Improve
public class HealthBar : BurningRope {
	protected new void Update() {
		base.Update();
		
		text.text = $"{(int)current} HP";
	}
}
