using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CoreStatusView : MonoBehaviour
{
    public ImageBar fireImageBar;
    public ImageBar warpIconImageBar;
    private ImageBar usingBar;
    public Image image;
    public TextMeshProUGUI healthText;
    public Button button;
    public int coreId;
    void Awake() {
        DisplayPlacing();
        if(!usingBar) {
            Debug.Log("USINGBAR"+coreId);
        }
    }

    public void Remove() {
        Destroy(gameObject);
    }
    public void DisplayCoreHealth(float hp) {
        //Texture2D texture = ImageToExpressfireHP.texture;
        healthText.text = hp.ToString();
        float healthPercentage = hp/100;
        //if(!usingBar) {
        //    Debug.Log(healthText.text);
        //}
        if(!usingBar) {
            Debug.Log("USINGBAR__"+coreId);
        }
        usingBar.UpdateBar(healthPercentage);
    }
    //AI
    Texture2D CombineTextures(Texture2D baseTex, Texture2D overlayTex, Vector2 pivot)
    {
        int width = Mathf.Max(baseTex.width, overlayTex.width);
        int height = Mathf.Max(baseTex.height, overlayTex.height);

        Texture2D result = new Texture2D(width, height);
        Color[] basePixels = baseTex.GetPixels();
        Color[] overlayPixels = overlayTex.GetPixels();

        // ピボット位置を考慮したオフセット計算
        int offsetX = Mathf.RoundToInt((width - overlayTex.width) * pivot.x);
        int offsetY = Mathf.RoundToInt((height - overlayTex.height) * pivot.y);

        // ベース画像をコピー
        result.SetPixels(0, 0, baseTex.width, baseTex.height, basePixels);

        // オーバーレイ画像をピボット位置で合成
        for (int y = 0; y < overlayTex.height; y++)
        {
            for (int x = 0; x < overlayTex.width; x++)
            {
                int targetX = x + offsetX;
                int targetY = y + offsetY;

                if (targetX >= 0 && targetX < width && targetY >= 0 && targetY < height)
                {
                    Color baseColor = result.GetPixel(targetX, targetY);
                    Color overlayColor = overlayTex.GetPixel(x, y);

                    // 透明度を考慮したブレンド
                    result.SetPixel(targetX, targetY, Color.Lerp(baseColor, overlayColor, overlayColor.a));
                }
            }
        }

        result.Apply();
        return result;
    }
    public void DisplayTransporting() {
        //image.color = Color.blue;
        fireImageBar.gameObject.SetActive(true);
        usingBar = fireImageBar;
        warpIconImageBar.gameObject.SetActive(false);
    }
    public void DisplayPlacing() {
        //image.color = Color.white;
        warpIconImageBar.gameObject.SetActive(true);
        usingBar = warpIconImageBar;
        fireImageBar.gameObject.SetActive(false);
    }
    public void SetId(int id) {
        coreId = id;
    }
    /*void Start() {
    }
    void Clicked() {
        
    }*/
}
