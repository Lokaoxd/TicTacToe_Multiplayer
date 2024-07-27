using UnityEngine;

public class Tools
{
    public static bool IsInMainCamera(Vector3 position)
    {
        var temp = Camera.main.WorldToViewportPoint(position);
        if (temp.x > 1f || temp.x < 0f || temp.y > 1f || temp.y < 0f) return false;
        return true;
    }

    public static string RemoveCloneName(string name) => name.Remove(name.IndexOf("(Clone)"));

    public static string ColorirTextoHtml(string text, string textToColor, Color color)
    {
        string temp = text;
        temp = temp.Insert(temp.IndexOf(textToColor), $"<Color={RGBToHEXA(color)}>");
        temp = temp.Insert(temp.IndexOf(textToColor) + textToColor.Length, "</Color>");
        return temp;
    }

    public static string ColorirTextoHtml(string text, string textToColor, string color)
    {
        string temp = text;
        temp = temp.Insert(temp.IndexOf(textToColor), $"<Color={color}>");
        temp = temp.Insert(temp.IndexOf(textToColor) + textToColor.Length, "</Color>");
        return temp;
    }

    public static string RGBToHEXA(Color color)
    {
        // Converta os valores de ponto flutuante para inteiros no intervalo de 0 a 255
        int r = Mathf.RoundToInt(color.r * 255);
        int g = Mathf.RoundToInt(color.g * 255);
        int b = Mathf.RoundToInt(color.b * 255);

        // Crie o código hexadecimal
        string codigoHex = string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);

        return codigoHex;
    }
}
