using UnityEngine;

public class Guest : MonoBehaviour
{
    Item mask;
    GuestSO guestAssigned;
    Transform tr;
    SpriteRenderer sr;

    private void Awake()
    {
        tr = GetComponent<Transform>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Vector3 offset = mask.itemAssigned.alignmentOffset;
        Instantiate(mask, tr.position + offset, Quaternion.identity, tr);
    }

    private void AssignSprite(SpriteRenderer sprite)
    {
        sr.sprite = sprite.sprite;
    }
}
