using UnityEngine;

[CreateAssetMenu(fileName = "New Player Spec", menuName = "Design/Player Spec")]
public class PlayerSpec : ScriptableObject
{
	[Header("Hover over fields with your mouse to see their description.")]

	[Tooltip("How much gold each player starts the match with")]
	public int gold = 100;
    [Tooltip("How much gold each player gets per resource tick")]
	public int goldIncrement = 10;
}