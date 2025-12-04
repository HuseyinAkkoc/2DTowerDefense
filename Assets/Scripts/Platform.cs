using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class Platform : MonoBehaviour
{


    [SerializeField] private LayerMask PlatformLayerMask;
    public static event Action<Platform> OnPlatformClicked;
    public static bool towerPanelOpen { get; set; } = false;
    public bool isAlreadyPlaced= false;

    UIController uicontroller =new UIController();
    private void Update()
    {
        if (isAlreadyPlaced)
        {
            Debug.Log("This platform already has a tower. Ignoring placement.");
            return;
        }
        if (towerPanelOpen || Time.timeScale == 0f)
        return;

#if UNITY_EDITOR || UNITY_STANDALONE
    // PC / Editor input
    if (Mouse.current.leftButton.wasPressedThisFrame)
    {
            UIButtonSound.Instance.PlayClick();
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            if (!isAlreadyPlaced)
            {
               //Debug.Log("This platform already has a tower. Ignoring placement.");
                
                HandlePlatformClick(worldPoint);

            }
            
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
            if (isAlreadyPlaced)
            {
                Debug.Log("This platform already has a tower. Ignoring placement.");
                return;
            }
            if (platform != null || !isAlreadyPlaced)
            {
                OnPlatformClicked?.Invoke(platform);
                Debug.Log("Platform clicked!");
            }
        }
    }

    


public void PlaceTower(TowerData data)
    {
        if (isAlreadyPlaced)
        {
            Debug.Log("This platform already has a tower. Ignoring placement.");
            return;
        }

        if (data == null || data.prefab == null )
        {
            Debug.LogError("TowerData or Tower prefab is missing!");
            return;
        }


        Instantiate(data.prefab, transform.position, Quaternion.identity,transform);
        isAlreadyPlaced = true;
    }

}
