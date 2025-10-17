using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class Platform : MonoBehaviour
{


    [SerializeField] private LayerMask PlatformLayerMask;
    public static event Action<Platform> OnPlatformClicked;
    public static bool towerPanelOpen { get; set; } = false;
    private void Update()
    {
    if (towerPanelOpen || Time.timeScale == 0f)
        return;

#if UNITY_EDITOR || UNITY_STANDALONE
    // PC / Editor input
    if (Mouse.current.leftButton.wasPressedThisFrame)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        HandlePlatformClick(worldPoint);
    }
#elif UNITY_ANDROID || UNITY_IOS
    // Mobile touch input
    if (Touchscreen.current != null)
    {
        var touch = Touchscreen.current.primaryTouch;
        if (touch.press.wasPressedThisFrame)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(touch.position.ReadValue());
            HandlePlatformClick(worldPoint);
        }
    }
#endif

    }


    private void HandlePlatformClick(Vector2 worldPoint)
    {
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



    public void PlaceTower(TowerData data)
    {

        if (data == null || data.prefab == null)
        {
            Debug.LogError("TowerData or Tower prefab is missing!");
            return;
        }


        Instantiate(data.prefab, transform.position, Quaternion.identity,transform);
    }

}
