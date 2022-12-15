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

    public bool sliding;
    public Vector2 touchStart;
    void StartTouch(Vector2 p)
    {
        sliding = true;
        cardAxis = 0;
        touchStart = p;
    }

    private float time;
    private float countDown = 0;
    
    void SetNonInteractableTime(float t)
    {
        time = Time.time;
        countDown = t;
        interactable = false;
    }

    void SlideLeft()
    {
        //do things on the left
        anim.SetTrigger(Slide);
    }

    void SlideRight()
    {
        //do things for the right
        anim.SetTrigger(Slide);
    }
    void EndTouch()
    {
        if (sliding)
        {
            sliding = false;
        }

        cardAxis = 0;
    }

    public bool interactable = false;    
    public float cardAxis = 0;
    public float cardDeadRange = 0.2f;
    public float cardSlideSpeed = 0;
    public float cardInertiaSpeed = 3.5f;
    public float dampenAxis = 0;
    public Animator anim;
    private static readonly int Slide = Animator.StringToHash("Slide");
    private static readonly int CardSlide = Animator.StringToHash("CardSlide");
    public Vector2 n;
    void UpdateTouch(Vector2 p)
    {
        
        var pos = p - touchStart;
        n = pos;
        var axis = pos.x;
        cardAxis = axis * (1+ cardDeadRange);
    }

    void UpdateCardGraphics()
    {
        dampenAxis = Mathf.SmoothDamp(dampenAxis, cardAxis, ref cardSlideSpeed, cardInertiaSpeed);
        anim.SetFloat(CardSlide, dampenAxis);
        Debug.Log("UpdatingDampenAxis");
    }

    protected void Update()
    {
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
