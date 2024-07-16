using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ColorChange : MonoBehaviour
{
    [SerializeField]
    PLayerGlobalSettings setClr;    
    public Image PeackedClr;
    public Material mat;
    public Color color;
    [SerializeField]
    GameObject canvas;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RayCast();
            if (!(localpos.x > r.x && localpos.y > r.y && localpos.x < r.x + r.width && localpos.y < r.y + r.height))
            {
                print("за рамкой");
                return;
            }
            if (color.a == 0)
                color = Color.black;
            //mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            PeackedClr.color = color;

        }
    }
    public RawImage CololrPanel;
    [SerializeField]
    Rect r;
    [SerializeField]
    Vector2 localpos;
    int px;
    int py;
    void RayCast()
    {
        Texture2D tex2 = CololrPanel.texture as Texture2D;

        r = CololrPanel.rectTransform.rect;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CololrPanel.rectTransform, Input.mousePosition, null, out localpos);
        px = Mathf.Clamp(0, (int)(((localpos.x - r.x) * tex2.width) / r.width), tex2.width);
        py = Mathf.Clamp(0, (int)(((localpos.y - r.y) * tex2.height) / r.height), tex2.height);

        if ((localpos.x > r.x && localpos.y > r.y && localpos.x < r.x + r.width && localpos.y < r.y + r.height))
            color = tex2.GetPixel(px, py);
    }

    public void ChooseColor()
    {
        setClr.SetColor(color);
        canvas.SetActive(false);
    }
}
