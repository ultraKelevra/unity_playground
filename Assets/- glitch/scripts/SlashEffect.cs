using DG.Tweening;
using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    private Transform slash_0;
    private Transform slash_1;
    private SpriteRenderer slash_rend_0;
    private SpriteRenderer slash_rend_1;
    private float offset = 1;
    public float duration = 0.5f;
    public SpriteRenderer original;
    public Vector2 slashDirection;

    private Material slash_0_mat;
    private Material slash_1_mat;

    // Use this for initialization
    void Start()
    {
        slash_0 = transform.GetChild(0);
        slash_1 = transform.GetChild(1);
        slash_rend_0 = slash_0.GetComponent<SpriteRenderer>();
        slash_rend_1 = slash_1.GetComponent<SpriteRenderer>();
        slash_0_mat = slash_rend_0.material;
        slash_1_mat = slash_rend_1.material;
        Slash(slashDirection);
    }

    public void SetCenter(Vector2 center)
    {
        slash_0_mat.SetTextureOffset("_Mask", center);
        slash_1_mat.SetTextureOffset("_Mask", center);
    }

    public void Slash(Vector2 direction)
    {
        slash_0.transform.DOLocalMove(Vector3.Normalize(direction) * offset, duration).SetLoops(2, LoopType.Yoyo);
        slash_1.transform.DOLocalMove(-Vector3.Normalize(direction) * offset, duration).SetLoops(2, LoopType.Yoyo);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        slash_rend_0.sprite = original.sprite;
        slash_rend_1.sprite = original.sprite;
    }
}