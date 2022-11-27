namespace Card.Renderers {
	public class Health: Base {
		public TMPro.TMP_Text health;

		public void Update() {
			base.Update();

			if (card is HealthCardBase hCard)
				health.text = "" + hCard.health;
		}
	}
}