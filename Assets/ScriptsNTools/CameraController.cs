using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothing;
    public Vector2 minPos, maxPos;

    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(this);
        
    }
    private void Update()
    {
        if(Camera.main != transform.gameObject.GetComponent<Camera>())
        {
            Camera.SetupCurrent( transform.gameObject.GetComponent<Camera>());
        }
        
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if(transform.position != target.position)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, -1);

            targetPosition.x = Mathf.Clamp(targetPosition.x, minPos.x, maxPos.x);

            if(target.GetComponent<PlayerController>())
            { if (target.GetComponent<PlayerController>().CameraInStoryMode())
                {
                    targetPosition.y = Mathf.Clamp(targetPosition.y, minPos.y, maxPos.y);
                }
                else
                {
                    targetPosition.y = Mathf.Clamp(targetPosition.y + 50, minPos.y, maxPos.y);
                }
            }
            

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }
}
