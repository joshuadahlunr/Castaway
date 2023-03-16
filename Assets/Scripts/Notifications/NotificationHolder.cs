using UnityEngine.UI.Extensions;

public class NotificationHolder : FlowLayoutGroup {
	public static NotificationHolder instance;

	public Notification notificationPrefab;

	private void Awake() => instance = this;

	public Notification CreateNotification(string message, float lifetime = 5) {
		var ret = Instantiate(notificationPrefab.gameObject, transform).GetComponent<Notification>();
		ret.text.text = message;
		ret.lifetime = lifetime;
		
		CalculateLayoutInputVertical();

		return ret;
	}
}