using UnityEngine;

public class FollowTo : MonoBehaviour
{
    public Transform followTo;

    public float translationSpeed = 1;
    public float rotationSpeed = 45;
    public float scaleSpeed = 1;

    public bool position = true;
    public bool rotation = true;
    public bool scale = true;

    // Update is called once per frame
    void Update()
    {
        var dTime = Time.deltaTime;
        if (position)
            transform.position = Vector3.MoveTowards(transform.position, followTo.position, translationSpeed * dTime);

        if (rotation)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, followTo.rotation, rotationSpeed * dTime);

        if (scale)
            transform.localScale = Vector3.MoveTowards(transform.localScale, followTo.localScale, translationSpeed * dTime);
    }
}