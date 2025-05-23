using UnityEngine;

/*
 * https://www.youtube.com/watch?v=7t218LaeiiI&ab_channel=AllThingsGameDev
 */

public class Door : MonoBehaviour, InteractEvent
{
    [SerializeField] GameObject door;
    [SerializeField] float openRot, closeRot;
    [SerializeField] float speed;
    [SerializeField] bool opening;

    [SerializeField] SingleAudio singleAudio;

    void Update()
    {
        Vector3 currentRot = door.transform.localEulerAngles;
        if (opening)
        {
            if (currentRot.y < openRot)
            {
                door.transform.localEulerAngles = Vector3.Lerp(currentRot, new Vector3(currentRot.x, openRot, currentRot.z), speed * Time.deltaTime);
            }
        }
        else // closing
        {
            if (currentRot.y > closeRot)
            {
                door.transform.localEulerAngles = Vector3.Lerp(currentRot, new Vector3(currentRot.x, closeRot, currentRot.z), speed * Time.deltaTime);
            }
        }
    }

    // Doors within the house don't use this interact?
    public void Interact()
    {
        if(singleAudio)
            singleAudio.PlaySFX("door_open");
        opening = !opening;
    }
}
