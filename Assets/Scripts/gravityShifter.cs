using UnityEngine;

public class gravityShifter : MonoBehaviour
{
    public Vector3 gravityShiftVector;

    void OnTriggerEnter(Collider target)
    {
        CharacterMovement player;
        if(target.transform.TryGetComponent<CharacterMovement>(out player))
            player.shiftGravity(gravityShiftVector);
    }
}
