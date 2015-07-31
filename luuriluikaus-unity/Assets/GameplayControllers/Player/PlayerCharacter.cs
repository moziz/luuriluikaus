using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour
{
    public float speendDropOff = 0.1f;
    public float speedMultiplier = 0.1f;
    public float jumpHeight = 1;
    public float jumpSpeed = 1;
    public float animationSpeedMultipler = 0.1f;
    public float upwardVolleyForce = 1.0f;
    public float forwardVolleyForce = 1.0f;
    private List<Texture> currentAnimation;
    public List<Texture> standAnimation = new List<Texture>();
    public List<Texture> runAnimation = new List<Texture>();
    public List<Texture> jumpAnimation = new List<Texture>();
    public List<Texture> fallAnimation = new List<Texture>();
    public List<Texture> hurlAnimation = new List<Texture>();

    public Transform throwableItem;
    public GameObject throwFan;

    private Material mat;
    private int animationFrame;
    private float animationSpeed;
    private float lastAnimationFrameSwap;

    public float currentSpeed = 0;
    public float targetSpeed = 0;
    public float currentHeight = 0;
    public int selectedNumber = 0;

    public bool slowingDown = false;
    public bool goingUp = false;
    public bool isJumping = false;

    public float defaultHeight = 0;

    public float ownDeltaTime = 1;
    public bool slowDownTime = false;
    public bool readyToHurl = false;
    void Start()
    {
        defaultHeight = transform.position.y;
        mat = GetComponent<MeshRenderer>().material;

        currentAnimation = standAnimation;

        throwFan = transform.FindChild("ThrowFan").gameObject;
        throwFan.SetActive(false);
        
        // TODO: rekisteröidy delegaatteihin
    }

    void Update()
    {
        DoTempControls();

        if (slowDownTime)
        {
            ownDeltaTime *= (1 - 1.0f * Time.deltaTime) * 0.9f;
        }
        else
        {
            ownDeltaTime = 1;
        }

        float localDeltaTime = Time.deltaTime * ownDeltaTime;

        if (slowingDown && !isJumping)
        {
            currentSpeed = Mathf.Max(0, currentSpeed - speendDropOff * localDeltaTime);
        }
        else if (!slowingDown)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 1 * localDeltaTime);
        }

        if (goingUp)
        {
            currentHeight = currentHeight + jumpSpeed * localDeltaTime;
            if (currentHeight > defaultHeight + jumpHeight)
            {
                currentHeight = defaultHeight + jumpHeight;
                goingUp = false;
                ChangeAnimation("fall");
            }
        }
        else if (currentHeight > defaultHeight)
        {
            currentHeight = currentHeight - jumpSpeed * localDeltaTime;
            if (currentHeight < defaultHeight)
            {
                currentHeight = defaultHeight;
                isJumping = false;
                SetSpeed(1);
            }
        }

        Vector2 pos = transform.position + new Vector3(currentSpeed * (localDeltaTime * 60.0f), 0, 0);
        pos.y = currentHeight;
        transform.position = pos;

        DoAnimation();
    }

    void NumberSelected(int number)
    {
        if (number == selectedNumber)
            return;
        Debug.Log("NumberSelected OK -> " + number + ", old: " + selectedNumber);


        selectedNumber = number;

        if (readyToHurl)
        {
            Hurl(number);
            readyToHurl = false;
        }
        else
        {
            slowingDown = false;

            if (number == 1)
            {
                if(!isJumping)
                    Jump();
            }
            else
            {
                ChangeAnimation("run");
                SetSpeed(number);
                targetSpeed = currentSpeed;
                currentSpeed *= 0.5f;
            }
        }
    }

    void SetSpeed(float number) // Number from 1 to 9
    {
        currentSpeed = number * speedMultiplier;
    }

    void UnrollFinished()
    {
        slowingDown = true;
        selectedNumber = -1;
    }

    void Jump()
    {
        ChangeAnimation("jump");
        goingUp = true;
        isJumping = true;
        currentSpeed = Mathf.Max(selectedNumber * speedMultiplier, currentSpeed);
    }

    void DropZoneReached()
    {
        slowDownTime = true;
        readyToHurl = true;
        throwFan.SetActive(true);
    }

    void Hurl(int number)
    {
        throwFan.SetActive(false);
        ChangeAnimation("throw");
        slowDownTime = false;

        ThrowableItem it = GameObject.Find("ThrowableItem").GetComponent<ThrowableItem>();
        it.GetThrown();
        ThrowMe(it.GetComponent<Rigidbody>());

        SetSpeed(1);
        selectedNumber = -1;
    }

    void DoTempControls()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            NumberSelected(1);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            NumberSelected(2);
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            NumberSelected(3);
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            NumberSelected(4);
        }
        else if (Input.GetKey(KeyCode.Alpha5))
        {
            NumberSelected(5);
        }
        else if (Input.GetKey(KeyCode.Alpha6))
        {
            NumberSelected(6);
        }
        else if (Input.GetKey(KeyCode.Alpha7))
        {
            NumberSelected(7);
        }
        else if (Input.GetKey(KeyCode.Alpha8))
        {
            NumberSelected(8);
        }
        else if (Input.GetKey(KeyCode.Alpha9))
        {
            NumberSelected(9);
        }
        else
        {
            if (!slowingDown)
                UnrollFinished();
        }
    }

    void ChangeAnimation(string animationName)
    {
        List<Texture> oldAnimation = currentAnimation;
        if (animationName == "run")
        {
            currentAnimation = runAnimation;
        }
        else if (animationName == "jump")
        {
            currentAnimation = jumpAnimation;
        }
        else if (animationName == "stand")
        {
            currentAnimation = standAnimation;
        }
        else if (animationName == "fall")
        {
            currentAnimation = fallAnimation;
        }
        else if (animationName == "throw")
        {
            currentAnimation = fallAnimation;
        }
        else
        {
            Debug.LogError("Strange animation: " + animationName);
        }

        if (currentAnimation != oldAnimation)
        {
            Debug.Log("ChangingAnimation: " + animationName);
            lastAnimationFrameSwap = -100;
            animationFrame = -1;
        }
    }

    void DoAnimation()
    {
        float animationFrameSwapTime = lastAnimationFrameSwap + (animationSpeedMultipler / Mathf.Max(1, currentSpeed / speedMultiplier)) * ownDeltaTime;
        if (animationFrameSwapTime <= Time.time && currentAnimation.Count > 0)
        {
            lastAnimationFrameSwap = Time.time;
            animationFrame = (animationFrame + 1) % (currentAnimation.Count);
            mat.mainTexture = currentAnimation[animationFrame];
        }
    }


    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("DropZone"))
        {
            DropZoneReached();
        }
    }

    public void ThrowMe(Rigidbody r)
    {
        r.velocity = new Vector3(currentSpeed, 0, 0);
        r.AddForce(new Vector3(forwardVolleyForce + currentSpeed, upwardVolleyForce + currentHeight));
    }
}
