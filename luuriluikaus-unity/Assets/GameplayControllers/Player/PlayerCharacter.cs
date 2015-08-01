using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour
{
    public float speedDropOff = 0.5f;
    public float speedMultiplier = 0.1f;
    public AnimationCurve speedCurve;

    public float minSpeed = 0.01f;
    public float jumpHeight = 1;
    public float jumpSpeed = 1;
    public float animationSpeedMultipler = 0.1f;
    public float upwardVolleyForce = 1.0f;
    public float forwardVolleyForce = 1.0f;
    private List<Sprite> currentAnimation;
    public List<Sprite> standAnimation = new List<Sprite>();
    public List<Sprite> runAnimation   = new List<Sprite>();
    public List<Sprite> carryAnimation = new List<Sprite>();
    public List<Sprite> holdAnimation  = new List<Sprite>();
    public List<Sprite> jumpAnimation  = new List<Sprite>();
    public List<Sprite> fallAnimation  = new List<Sprite>();
    public List<Sprite> hurlAnimation  = new List<Sprite>();
    public List<Sprite> tripAnimation  = new List<Sprite>();
    public List<Sprite> dieAnimation   = new List<Sprite>();
    public List<Sprite> giveUpAnimation = new List<Sprite>();
    private SpriteRenderer spriteRenderer;
    
    public Transform throwableItemPrefab;
    private ThrowableItem currentItem;
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
    public bool useDebugControls = false;
    public bool gameEnding = false;
    private bool tripAndFall = false;
    private bool dropAndGiveUp = false;
    public bool gameOver = false;
    private Vector3 originalPosition;
    private float gameOverTime = 0;
    public bool hasThrown = false;
    private bool throwing = false;
    private bool justThrew = false;
    public bool CurrentlyThrowing { get { return throwing || justThrew; } } // For sounds
    Transform pointer;

    TextMesh textMesh;
    string tutorialStart = "Dial a number to run";
    string tutorialThrow = "Dial a number to throw";
    string tutorialDial1 = "Dial 1 to jump";
    string tutorialVolley = "Run and volley!";
    string tutorialRestart = "Dial any number to restart";

    void Awake()
    {
        GameObject phone = GameObject.Find("Phone");
        if (phone)
        {
            PhoneController p = phone.GetComponent<PhoneController>();
            p.SubscribeOnRotaryEnd(UnrollFinished);
            p.SubscribeOnRotaryRelease(NumberSelected);
        }
        else
        {
            useDebugControls = true;
        }
    }

    void Start()
    {
        originalPosition = transform.position;
        defaultHeight = transform.position.y;
        mat = GetComponent<MeshRenderer>().material;

        currentAnimation = standAnimation;

        throwFan = transform.FindChild("ThrowFan").gameObject;
        throwFan.SetActive(false);

        {
            currentItem = Instantiate(throwableItemPrefab).GetComponent<ThrowableItem>();
        }

        currentSpeed = minSpeed;
        targetSpeed = currentSpeed;

        textMesh = GameObject.Find("TutorialText").GetComponent<TextMesh>();
        textMesh.text = tutorialStart;

        pointer = transform.FindChild("Pointer");
        Transform spriteTransform = transform.FindChild("PlayerSprite");
        spriteRenderer = spriteTransform.GetComponentInChildren<SpriteRenderer>();
        hasThrown = false;
        throwing = false;
        dropAndGiveUp = false;
        tripAndFall = false;
        gameEnding = false;
        gameOver = false;
        ChangeAnimation("stand");
        selectedNumber = -1;
    }

    void Restart()
    {
        transform.position = originalPosition;
        gameOverTime = 0;
        tripAndFall = false;
        Start();
        textMesh.text = "";
    }

    void Update()
    {
        justThrew = false;

        if (gameOver)
        {
            textMesh.text = tutorialRestart;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Restart();
            }

            DoAnimation();
            return;
        }

        if (!gameOver && gameEnding && !isJumping)
        {
            if (tripAndFall)
            {
                ChangeAnimation("trip");
            }
            else if(dropAndGiveUp)
            {
                ChangeAnimation("giveUp");
                gameOver = true;
            }
            else
            {
                Debug.LogError("FAILURE IN GAME ENDING");
            }    
        }

        if (useDebugControls || Input.GetKeyDown(KeyCode.Alpha9))
        {
            useDebugControls = true;
            DoTempControls();
        }

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
            float oldSpeed = currentSpeed;
            currentSpeed = Mathf.Max(0, currentSpeed - (!gameEnding ? speedDropOff : 0.2f) * localDeltaTime);

            if(gameEnding)
            {
                if(currentSpeed <= minSpeed && currentAnimation == tripAnimation)
                {
                    gameOver = true;
                    ChangeAnimation("die");
                }
            }
            else if (!throwing)
            {
                if (currentSpeed < minSpeed)
                {
                    ChangeAnimation("stand");
                }
                else
                {
                    if (oldSpeed < minSpeed)
                    {
                        ChangeAnimation("run");
                    }
                }
            }
        }
        else if (!slowingDown)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 10 * localDeltaTime);
        }

        if (goingUp)
        {
            currentHeight = currentHeight + jumpSpeed * localDeltaTime;
            if (currentHeight > defaultHeight + jumpHeight)
            {
                currentHeight = defaultHeight + jumpHeight;
                goingUp = false;
                if(!throwing)
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
                if (!throwing)
                    ChangeAnimation("run");
            }
        }

        Vector2 pos = transform.position + new Vector3(currentSpeed * (localDeltaTime * 60.0f), 0, 0);
        pos.y = currentHeight;
        transform.position = pos;
        
        DoAnimation();

        if(pointer != null && !gameEnding)
        {
            Transform target = currentItem.transform;
            pointer.LookAt(target, -Vector3.forward);
            float distance = (target.position - transform.position).magnitude;
            if (distance > 10 || (transform.position.x - target.position.x) > 3 || (transform.position.y - target.position.y) > 5)
            {
                pointer.gameObject.SetActive(true);
            }
            else if (distance < 6)
            {
                pointer.gameObject.SetActive(false);
            }
        }
    }

    void NumberSelected(int number)
    {
        textMesh.text = "";
        if (gameOver)
        {
            Restart();
            return;
        }

        if (number == selectedNumber || gameEnding)
            return;

        if (number < 1 || number > 9)
        {
            selectedNumber = -1;
            return;
        }


        selectedNumber = number;

        if (readyToHurl)
        {
            Debug.Log("HURL NUMBER: " + number);
            Hurl(number);
            readyToHurl = false;
        }
        else
        {
            Debug.Log("RUN NUMBER: " + number);
            slowingDown = false;

            if (number == 1 && !readyToHurl)
            {
                if (!isJumping)
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
        Debug.Log("Set speed: " + number);
        currentSpeed = speedCurve.Evaluate((number - 1) / 8.0f) * 9.0f  * speedMultiplier;
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
        textMesh.text = tutorialThrow;
        slowDownTime = true;
        readyToHurl = true;
        throwFan.SetActive(true);
    }

    void Hurl(int number)
    {
        throwFan.SetActive(false);
        slowDownTime = false;
        goingUp = false; // Start falling if was mid-jump
        hasThrown = true;
        
        currentItem.GetThrown();

        float forward = Mathf.Cos((number - 1) * 10 * Mathf.Deg2Rad);
        float up = Mathf.Sin(number * 10 * Mathf.Deg2Rad);
        
        ThrowMe(currentItem.GetComponent<Rigidbody>(), forward * currentItem.forwardForceMult, up * currentItem.upwardForceMult);

        currentSpeed = 0;
        selectedNumber = -1;
    }

    void DoTempControls()
    {
        return;

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
        List<Sprite> oldAnimation = currentAnimation;
        if (animationName == "run")
        {
            if (hasThrown)
            {
                currentAnimation = runAnimation;
            }
            else
            {
                currentAnimation = carryAnimation;
            }
        }
        else if (animationName == "trip")
        {
            currentAnimation = tripAnimation;
        }
        else if (animationName == "die")
        {
            currentAnimation = dieAnimation;
        }
        else if (animationName == "jump")
        {
            currentAnimation = jumpAnimation;
        }
        else if (animationName == "stand")
        {
            if(gameOver)
            {
                currentAnimation = dieAnimation;
            }
            else if(!hasThrown)
            {
                currentAnimation = holdAnimation;
            }
            else
            {
                currentAnimation = standAnimation;
            }
        }
        else if (animationName == "fall")
        {
            currentAnimation = fallAnimation;
        }
        else if (animationName == "throw")
        {
            currentAnimation = hurlAnimation;
        }
        else if (animationName == "giveUp")
        {
            currentAnimation = giveUpAnimation;
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

    void ChangeAppropriateAnimation()
    {
        if(gameEnding)
        {

        }
        else if (isJumping)
        {
            if (goingUp)
            {
                ChangeAnimation("jump");
            }
            else
            {
                ChangeAnimation("fall");
            }
        }
        else
        {
            if (currentSpeed > minSpeed)
            {
                ChangeAnimation("run");
            }
            else
            {
                ChangeAnimation("stand");
            }
        }
    }

    void DoAnimation()
    {
        float animationFrameSwapTime = lastAnimationFrameSwap + (animationSpeedMultipler / Mathf.Max(1, currentSpeed / speedMultiplier) / ownDeltaTime);

        // Hurl animation is non-looping
        if (currentAnimation == hurlAnimation || tripAnimation == currentAnimation || currentAnimation == giveUpAnimation)
        {
            animationFrameSwapTime = lastAnimationFrameSwap + animationSpeedMultipler / ownDeltaTime * 0.3f;
            if (animationFrameSwapTime <= Time.time && currentAnimation.Count > 0)
            {
                lastAnimationFrameSwap = Time.time;
                animationFrame = animationFrame + 1;
                if (animationFrame < currentAnimation.Count)
                {
                    spriteRenderer.sprite = currentAnimation[animationFrame];
                }
                else
                {
                    if (currentAnimation == hurlAnimation)
                    {
                        throwing = false;
                        justThrew = true;
                    }
                    else if (currentAnimation == tripAnimation)
                        gameOver = currentSpeed <= minSpeed;
                    ChangeAppropriateAnimation();
                }
            }
            return;
        }

        if (animationFrameSwapTime <= Time.time && currentAnimation.Count > 0)
        {
            lastAnimationFrameSwap = Time.time;
            animationFrame = (animationFrame + 1) % (currentAnimation.Count);
            spriteRenderer.sprite = currentAnimation[animationFrame];
        }
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("DropZone"))
        {
            DropZoneReached();
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            TripAndFall();
        }
    }

    public void ThrowMe(Rigidbody r, float forwardForce, float upForce)
    {
        throwing = true;
        ChangeAnimation("throw");
        r.velocity = new Vector3(currentSpeed, 0, 0);
        r.AddForce(new Vector3((forwardVolleyForce * (0.8f + Random.value) + currentSpeed) * forwardForce, upwardVolleyForce * (0.8f + Random.value) + currentHeight) * upForce);
    }

    public void TripAndFall()
    {
        gameEnding = true;
        tripAndFall = true;
        slowingDown = true;
        
        Rigidbody r = currentItem.GetComponent<Rigidbody>();
        currentItem.GetAbandoned();

        if (!hasThrown)
        {
            currentItem.GetThrown();

            r.velocity = new Vector3(currentSpeed, 0, 0);
            r.AddForce(new Vector3((forwardVolleyForce * (0.8f + Random.value) + currentSpeed), upwardVolleyForce * (0.1f + Random.value * 0.2f) + currentHeight));
        }

    }

    public void Stop()
    {
        gameEnding = true;
        dropAndGiveUp = true;
        slowingDown = true;
    }
}
