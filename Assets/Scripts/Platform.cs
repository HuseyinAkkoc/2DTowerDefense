using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class Platform : MonoBehaviour
{


    [SerializeField] private LayerMask PlatformLayerMask;
    public static event Action<Platform> OnPlatformClicked;
    private void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 worldPoint= Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());  // fix here for touch screen input

            RaycastHit2D raycastHit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, PlatformLayerMask);



            if (raycastHit.collider != null)
            {
                Platform platform = raycastHit.collider.GetComponent<Platform>();

                if (platform != null)
                {
                    OnPlatformClicked?.Invoke(platform);
                    Debug.Log("Platform clicked!");
                }
            }

        }


    }
}
