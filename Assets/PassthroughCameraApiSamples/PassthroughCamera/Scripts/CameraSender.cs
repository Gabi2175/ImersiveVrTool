using System.Collections;
using System.IO;
using UnityEngine;
using System.Net.Sockets;
using PassthroughCameraSamples;

public class CameraSender : MonoBehaviour
{
    public WebCamTextureManager camManager;

    private Texture2D texture2D;
    private UdpClient client;

    [Range(0.05f, 1.0f)]
    public float sendInterval = 0.05f; 

    void Start()
    {
        client = new UdpClient();
        StartCoroutine(SendFramesCoroutine());
    }

    IEnumerator SendFramesCoroutine()
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

                    byte[] imageBytes = texture2D.EncodeToJPG(50); //qualidade da imagem da para ajustar depois se eu quiser

                    try
                    {
                        client.Send(imageBytes, imageBytes.Length, "192.168.137.1", 5005);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Erro ao enviar frame: " + e.Message);
                    }
                }
            }

            yield return new WaitForSeconds(sendInterval); 
        }
    }

    void OnDestroy()
    {
        client.Close();
    }
}
