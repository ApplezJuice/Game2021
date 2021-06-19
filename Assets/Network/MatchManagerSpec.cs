using UnityEngine;

[CreateAssetMenu(fileName = "New Match Manager Spec", menuName = "Design/Match Manager Spec")]
public class MatchManagerSpec : ScriptableObject
{
	[Header("Hover over fields with your mouse to see their description.")]

	[Tooltip("Gold refresh timer (in seconds)")]
	public float goldRefreshTimer = 5.0f;
}