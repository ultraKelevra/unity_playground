using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CardSwipeController : MonoBehaviour
{
    protected void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    protected void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
    
    void StartTouch()
    {
        sliding = true;
        _cardAxis = 0;
    }

    
    public enum CardMenuState
    {
        Idle,
        TransitionLeave,
        LoadingCard,
        TransitionEnter
    }

    private bool sliding;
    private Vector2 touchStart;
    private float time;
    private float countDown = 0;
    private const float CardInertiaDeadRange = 0.01f;

    public float pauseAfterDiscarding = 1.0f;
    public float pauseAfterAppear = 1.0f;
    public CardMenuState menuState = CardMenuState.LoadingCard;

    public bool Interactable => menuState == CardMenuState.Idle;
    private float _cardAxis = 0;
    private float _cardSlideSpeed = 0;
    
    [Range(0,.5f)]
    public float cardSlideDeadRange = 0.35f;


    public float cardInertiaSpeed = 0.1f;
    // [HideInInspector]
    public float dampenAxis = 0;
    public Animator anim;
    private static readonly int _discard_id = Animator.StringToHash("Discard");
    private static readonly int _slide_id = Animator.StringToHash("Slide");
    private static readonly int _appear_id = Animator.StringToHash("Appear");

    void Start()
    {
        //TODO: do this on an asset load callback
        Appear();
    }
    void LoadNextCard()
    {
        //do something with addressables
        
        //change current menuState
        menuState = CardMenuState.LoadingCard;
        
        // once loaded the info, update the card graphics and call Appear()
    }

    void Appear()
    {
        anim.SetTrigger(_appear_id);
        menuState = CardMenuState.TransitionEnter;
        SetNonInteractableTime(pauseAfterAppear);
    }

    void Disappear()
    {
        anim.SetTrigger(_discard_id);
        menuState = CardMenuState.TransitionLeave;
        Debug.Log("Disappearing");
        SetNonInteractableTime(pauseAfterDiscarding);
    }
    void SetNonInteractableTime(float t)
    {
        time = Time.time;
        countDown = t;
    }

    void SlideLeft()
    {
        //do things on the left
        Disappear();
    }

    void SlideRight()
    {
        //do things for the right
        Disappear();
    }
    
    void EndTouch()
    {
        if (sliding)
        {
            sliding = false;
            if (1.0f - cardSlideDeadRange < Mathf.Abs(dampenAxis))
            {
                if(_cardAxis > 0)
                    SlideRight();
                else
                    SlideLeft();

                SetNonInteractableTime(pauseAfterDiscarding);
                menuState = CardMenuState.TransitionLeave;
            }
        }

        _cardAxis = 0;
    }

    void UpdateTouch(Touch touch)
    {
        var h = Screen.height;
        var w = Screen.width;
        var touchPos = touch.screenPosition;
        var touchStartPos = touch.startScreenPosition;
        var vector = new Vector2((touchPos.x - touchStartPos.x) / w, (touchPos.y - touchStartPos.y) / h);
        
        var axis = vector.x;
        _cardAxis = axis * (1 + cardSlideDeadRange);
    }

    void UpdateCardGraphics()
    {
        if ( !sliding && Mathf.Abs(dampenAxis) < CardInertiaDeadRange)
        {
            dampenAxis = 0;
            return;
        }

        if (menuState == CardMenuState.TransitionLeave)
            return;
        dampenAxis = Mathf.SmoothDamp(dampenAxis, _cardAxis, ref _cardSlideSpeed, cardInertiaSpeed);
        anim.SetFloat(_slide_id, dampenAxis);
    }

    void CheckAfterNonInteractableStage()
    {
        switch (menuState)
        {
            //TODO: this case will be removed once the loading is handled by another entity
            case CardMenuState.LoadingCard:
                Appear();
                break;
            case CardMenuState.TransitionEnter:
                //TODO: IDK... seems it's all good
                menuState = CardMenuState.Idle;
                break;
            case CardMenuState.TransitionLeave:
                LoadNextCard();
                //TODO: make this into a callback instead of another timer
                //after this card loads the Appear() method should be called
                SetNonInteractableTime(1.0f);
                //TODO: CheckAfterNonInteractableStage() should never call SetNonInteractableTime() but this is mocking the load card time (0.1 sec)
                break;
        }
    }

    protected void Update()
    {
        UpdateCardGraphics();

        if (!Interactable)
        {
            if (Time.time > time + countDown)
            {
                CheckAfterNonInteractableStage();
            }
            else
                return;
        }

        var activeTouches = Touch.activeTouches;
        if (activeTouches.Count == 0)
        {
            EndTouch();
        }
        else
        {
            var currentTouch = activeTouches[0];

            if (currentTouch.began && menuState == CardMenuState.Idle)
            {
                StartTouch();
                Debug.Log("startingTouch");
                return;
            }

            UpdateTouch(currentTouch);
        }
    }
}
