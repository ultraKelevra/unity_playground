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
    
    void StartTouch(Vector2 p)
    {
        sliding = true;
        _cardAxis = 0;
        touchStart = p;
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
    public float pauseAfterDiscarding = 1.0f;
    public float pauseAfterAppear = 1.0f;
    public CardMenuState menuState = CardMenuState.LoadingCard;
    
    public bool interactable = false;    
    private float _cardAxis = 0;
    private float _cardSlideSpeed = 0;
    [Range(0,.5f)]
    public float cardSlideDeadRange = 0.2f;
    public float cardInertiaDeadRange = 0.01f;

    public float cardInertiaSpeed = 3.5f;
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
        SetNonInteractableTime(pauseAfterDiscarding);
    }
    void SetNonInteractableTime(float t)
    {
        time = Time.time;
        countDown = t;
        interactable = false;
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

    void UpdateTouch(Vector2 p)
    {
        var pos = p - touchStart;
        var axis = pos.x;
        _cardAxis = axis * (1+ cardSlideDeadRange);
    }

    void UpdateCardGraphics()
    {
        if ( !sliding && Mathf.Abs(dampenAxis) < cardInertiaDeadRange)
        {
            dampenAxis = 0;
            return;
        }

        dampenAxis = Mathf.SmoothDamp(dampenAxis, _cardAxis, ref _cardSlideSpeed, cardInertiaSpeed);
        anim.SetFloat(_slide_id, dampenAxis);
    }

    void CheckAfterNonInteractableStage()
    {
        interactable = true;

        switch (menuState)
        {
            case CardMenuState.LoadingCard:
                anim.SetTrigger(_appear_id);
                menuState = CardMenuState.TransitionEnter;
                //TODO: load next card
                Debug.Log("loading -> transition");
                // SetNonInteractableTime(pauseAfterAppear);
                break;
            case CardMenuState.TransitionEnter:
                //TODO: IDK... seems it's all good
                menuState = CardMenuState.Idle;
                break;
            case CardMenuState.TransitionLeave:
                //TODO: load next card and leave a callback to replace shown graphics and display appearing animation
                menuState = CardMenuState.LoadingCard;
                break;
        }
    }

    protected void Update()
    {
        if (!interactable)
        {
            if (Time.time < time + countDown)
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
            var position = currentTouch.screenPosition;
            var w = Screen.width;
            var h = Screen.height;
            var normalized = new Vector2(position.x / w, position.y / h);

            if (currentTouch.began)
            {
                StartTouch(normalized);
                return;
            }

            UpdateTouch(normalized);
        }
        UpdateCardGraphics();
    }
}
