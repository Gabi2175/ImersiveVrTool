using System.Collections;
using UnityEngine;
using System.Net.Sockets;
using PassthroughCameraSamples;

public class CameraSender : MonoBehaviour
{
    public WebCamTextureManager camManager;

    private Texture2D texture2D;
    private Texture2D resizedTexture;
    private UdpClient client;

    private byte[] latestFrameData = null;
    private bool hasNewFrame = false;

    [Range(0.05f, 1.0f)]
    public float sendInterval = 0.05f; 

    void Start()
    {
        client = new UdpClient();
        StartCoroutine(CaptureCoroutine());
        StartCoroutine(SendCoroutine());
    }

    IEnumerator CaptureCoroutine()
    {
        while (true)
        {
            if (camManager != null && camManager.WebCamTexture != null && camManager.WebCamTexture.isPlaying)
            {
                WebCamTexture camTex = camManager.WebCamTexture;

                if (camTex.width > 16 && camTex.height > 16)
                {
                    
                    if (texture2D == null || texture2D.width != camTex.width || texture2D.height != camTex.height)
                    {
                        texture2D = new Texture2D(camTex.width, camTex.height, TextureFormat.RGB24, false);
                    }

                    texture2D.SetPixels(camTex.GetPixels());
                    texture2D.Apply();

                    // Reescala a imagem pela metade
                    int targetWidth = camTex.width / 2;
                    int targetHeight = camTex.height / 2;

                    if (resizedTexture == null || resizedTexture.width != targetWidth || resizedTexture.height != targetHeight)
                    {
                        resizedTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);
                    }

                    ResizeTexture(texture2D, resizedTexture);

                    
                    latestFrameData = resizedTexture.EncodeToJPG(40); 
                    hasNewFrame = true;
                }
            }

            yield return new WaitUntil(() => !hasNewFrame);
        }
    }

    IEnumerator SendCoroutine()
    {
        while (true)
        {
            if (hasNewFrame)
            {
                try
                {
                    client.Send(latestFrameData, latestFrameData.Length, "192.168.137.1", 5005);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Erro ao enviar: " + e.Message);
                }

                hasNewFrame = false;
            }

            yield return null;
        }
    }

    void OnDestroy()
    {
        client.Close();
    }

    // Função para reduzir resolução (interpolação bilinear simples)
    void ResizeTexture(Texture2D source, Texture2D dest)
    {
        Color[] pixels = new Color[dest.width * dest.height];

        float incX = 1.0f / ((float)dest.width);
        float incY = 1.0f / ((float)dest.height);

        for (int y = 0; y < dest.height; y++)
        {
            for (int x = 0; x < dest.width; x++)
            {
                float u = x * incX;
                float v = y * incY;
                pixels[y * dest.width + x] = source.GetPixelBilinear(u, v);
            }
        }

        dest.SetPixels(pixels);
        dest.Apply();
    }
}
