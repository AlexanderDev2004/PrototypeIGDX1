using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    public Camera playerCamera;
    public float pickUpRange = 5f;
    public GameObject pickedUpItem;
    public Image itemImage;
    public RectTransform itemSlot;
    public GameObject blockPrefab;  // Reference to the block prefab
    public Text messageText;  // Reference to the UI Text

    private Vector2 originalPosition;
    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private GameObject targetBall;

    void Start()
    {
        if (itemSlot != null)
        {
            originalPosition = itemSlot.anchoredPosition;
            originalAnchorMin = itemSlot.anchorMin;
            originalAnchorMax = itemSlot.anchorMax;
        }
        messageText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickUpItem();
        }

        if (Input.GetMouseButton(0) && pickedUpItem != null)
        {
            PlaceItemInCenter();
        }
        else if (Input.GetMouseButtonUp(0) && pickedUpItem != null)
        {
            ResetItemPosition();
        }

        if (itemSlot.anchoredPosition == Vector2.zero)
        {
            CheckForTarget();
        }

        if (Input.GetKeyDown(KeyCode.F) && targetBall != null)
        {
            ReplaceBallWithBlock();
        }
    }

    void TryPickUpItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickUpRange))
        {
            if (hit.transform.CompareTag("Collectible"))
            {
                pickedUpItem = hit.transform.gameObject;
                pickedUpItem.SetActive(false);
                ShowItemInHUD(pickedUpItem);
            }
        }
    }

    void ShowItemInHUD(GameObject item)
    {
        SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            itemImage.sprite = spriteRenderer.sprite;
            itemSlot.gameObject.SetActive(true); // Enable the Image
        }
    }

    void PlaceItemInCenter()
    {
        itemSlot.anchorMin = new Vector2(0.5f, 0.5f);
        itemSlot.anchorMax = new Vector2(0.5f, 0.5f);
        itemSlot.anchoredPosition = Vector2.zero;
    }

    void ResetItemPosition()
    {
        itemSlot.anchorMin = originalAnchorMin;
        itemSlot.anchorMax = originalAnchorMax;
        itemSlot.anchoredPosition = originalPosition;
    }

    void CheckForTarget()
    {
        RaycastHit hit;
        Vector3 forwardDirection = playerCamera.transform.forward;
        Vector3 rayOrigin = playerCamera.transform.position;

        // Draw the ray in the Scene view
        Debug.DrawRay(rayOrigin, forwardDirection * pickUpRange, Color.red);

        if (Physics.Raycast(rayOrigin, forwardDirection, out hit, pickUpRange))
        {
            if (hit.transform.CompareTag("TargetBall"))
            {
                targetBall = hit.transform.gameObject;
                // messageText.gameObject.SetActive(true);
                Debug.Log("Hit TargetBall: " + hit.transform.gameObject.name);
            }
            else
            {
                targetBall = null;
                // messageText.gameObject.SetActive(false);
                Debug.Log("Not Hit TargetBall: " + hit.transform.gameObject.name);
            }
        }
    }


    void ReplaceBallWithBlock()
    {
        Instantiate(blockPrefab, targetBall.transform.position, targetBall.transform.rotation);
        Destroy(targetBall);
        messageText.gameObject.SetActive(false);
        itemSlot.gameObject.SetActive(false);
    }
}

