using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CrosshairController : MonoBehaviour {

    private Image crosshair;
 //   private bool isTarget = false;
 //   private Transform curTarget;

  //  RectTransform CanvasRect;

    void Start()
    {
        crosshair = GetComponent<Image>();
       // CanvasRect = GetComponent<RectTransform>();
        HideCrosshair();
    }

    /*
    void Update()
    {
        if (isTarget)
        {
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(curTarget.position);
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

            CanvasRect.anchoredPosition = WorldObject_ScreenPosition;
        }
    }
    */

    public void ShowCrosshair(Transform target)
    {
        crosshair.enabled = true;
   //     curTarget = target;
   //     isTarget = true;
    }

    public void HideCrosshair()
    {
   //     isTarget = false;
        crosshair.enabled = false;
  //      curTarget = null;
    }
}
