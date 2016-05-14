using UnityEngine;
using System.Collections;

public class AngelKingAnimationEvents : MonoBehaviour {

    [SerializeField]
    private AngelKingController angel;

	public void AttackOver()
    {
        angel.AttackOver();
    }
}
