using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour
{
    public float speendDropOff;
    public float speedMultiplier;
    public float jumpHeight;
    public float jumpSpeed;

    public float currentSpeed;
    public float currentHeight;
    public int selectedNumber;

    public bool slowingDown = false;
    public bool goingUp = false;
    public bool isJumping = false;

    public float defaultHeight = 0;

    public float ownDeltaTime = 1;
    public bool slowDownTime = false;

    void Start()
    {
        defaultHeight = transform.position.y;

        // TODO: rekisteröidy delegaatteihin
        // laita vauhti
    }

    void Update()
    {
        if(slowDownTime)
        {
            ownDeltaTime *= (1 - 0.1f * Time.deltaTime); 
        }
        else
        {
            ownDeltaTime = 1;
        }

        if (slowingDown)
        {
            currentSpeed = Mathf.Max(0, currentSpeed - speendDropOff * Time.deltaTime);
        }

        if (goingUp)
        {
            currentHeight = currentHeight + jumpSpeed * Time.deltaTime * ownDeltaTime;
            if(currentHeight > defaultHeight + jumpHeight)
            {
                currentHeight = defaultHeight + jumpHeight;
                goingUp = false;
            }
        }
        else if (currentHeight > defaultHeight)
        {
            currentHeight = currentHeight - jumpSpeed * Time.deltaTime * ownDeltaTime;
            if(currentHeight < defaultHeight)
            {
                currentHeight = defaultHeight;
                isJumping = false;
            }
        }

        Vector2 pos = transform.position + new Vector3(currentSpeed, 0, 0);
        pos.y = currentHeight;
        transform.position = pos;
    }

    void NumberSelected(int number)
    {
        selectedNumber = number;
        slowingDown = false;

        currentSpeed = number * speedMultiplier;

        if (number == 1)
        {
            Jump();
        }
    }

    void UnrollFinished()
    {
        slowingDown = true;
    }

    void Jump()
    {
        goingUp = true;
        isJumping = true;
    }

    void SlowDown()
    {
        slowDownTime = true;
    }

    void Hurl()
    {
        slowDownTime = false;
    }
}
