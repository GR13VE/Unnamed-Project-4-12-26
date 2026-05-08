using UnityEngine;

public class gravityShifter : MonoBehaviour
{
    public Vector3 gravityShiftVector;

    void OnTriggerEnter(Collider target)
    {
        AltCharecterMovement player;
        if(target.transform.TryGetComponent<AltCharecterMovement>(out player))
            player.shiftGravity(gravityShiftVector);
    }
}
