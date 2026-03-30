using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Drag Player object here")]
    public Transform player;
    public Vector3 cameraOffset;
    [Range(1,10)]
    public float smoothingFactor;
    //Camera boundry values
    [Header("Camera Boundry")][Tooltip("Camera will not follow player out of bounds")]
    public Vector3 minValues, maxValues;
    private void FixedUpdate()
    {
        Follow();
    }

    void Follow() //Makes the camera follow the player with an offset that the designer can choose
    {
        //Define minimum and maximum x,y,z values for the camera
        
        Vector3 playerPosition = player.position + cameraOffset;
        //Check if the playerPosition is out of bounds or not

        //Limit the camera movement based on min and max values
        Vector3 boundPosition = new Vector3(
            Mathf.Clamp(playerPosition.x,minValues.x,maxValues.x), 
            Mathf.Clamp(playerPosition.y, minValues.y, maxValues.y),
            Mathf.Clamp(playerPosition.z, minValues.z, maxValues.z));
        //Smooths the movement of the camera
        Vector3 smoothCamera = Vector3.Lerp(transform.position, boundPosition, smoothingFactor*Time.fixedDeltaTime);
        transform.position = smoothCamera;
    }
}
