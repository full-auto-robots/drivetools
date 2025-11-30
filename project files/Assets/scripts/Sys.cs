using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the different types of data that are possible, organized by index
public enum Data
{
    Text,
    Vector3, // $vector3()
    Field, // $field()
}

public class Sys : MonoBehaviour
{
    // I like the size of the icons in the unity editor window,
    // but not when a build is made 
    // the solution is to have the entire system built off of a UI scale:
    // I could have some icon size variables but to make life easier I'm gonna just have a multiplier
    public static float uiScaleMultiplier;
    [Range(0.5f, 2f)]
    public float ins_uiScaleMultiplier;

    public Sprite[] ins_icons;
    public static Sprite[] icons;

    public Color[] ins_typeColors;
    public static Color[] typeColors;

    void Awake()
    {
        Sys.icons = ins_icons;
        Sys.typeColors = ins_typeColors;

        ins_uiScaleMultiplier = 1;
        uiScaleMultiplier = ins_uiScaleMultiplier;
    }

    // allowing devs to use the slider in the inspector and see real-time results
    void Update()
    {
        uiScaleMultiplier = ins_uiScaleMultiplier;
    }

    // converts a value in seconds to a minute:seconds format
    public static string ConvertToClockFormat(float input)
    {
        string seconds = Mathf.Round(input % 60).ToString();
        if (seconds.Length < 2)
        {
            seconds = "0" + seconds;
        }
        return Mathf.Floor(input / 60f).ToString() + ":" + seconds;
    }

    public static string ReformatDate(string input)
    {
        string result = "";

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '/' || input[i] == '\\' || input[i] == ' ' || input[i] == ':')
            {
                result += "_";
            }
            else
            {
                result += input[i];
            }
        }

        return result;
    }

    public static Vector3 AdjustVectorXY(Vector3 v, float theta)
    {
        return new Vector3(v.x * Mathf.Cos(theta / 180 * Mathf.PI) - v.y * Mathf.Sin(theta / 180 * Mathf.PI), v.y * Mathf.Cos(theta / 180 * Mathf.PI) + v.x * Mathf.Sin(theta / 180 * Mathf.PI), v.z);
    }
    public static bool IsPointInBounds(RectTransform bounds, Vector2 point)
    {
        return point.x > bounds.position.x - bounds.sizeDelta.x / 2 && point.x < bounds.position.x + bounds.sizeDelta.x / 2 && point.y > bounds.position.y - bounds.sizeDelta.y / 2 && point.y < bounds.position.y + bounds.sizeDelta.y / 2;
    }

    public static ushort GetDisplayObjectIndex(string input)
    {
        if (input[0] == '$')
        {
            string className = input.Substring(1, FindFirstOccurance(input, '(') - 1);

            if (className == "vector3")
            {
                return 0;
            }
            else if (className == "field")
            {
                return 1;
            }
            else
            {
                return 999; // used as a sort of error code here
            }
        }
        else
        {
            return 0;
        }
    }
    
    public static ushort GetTypeIndex(string input)
    {
        if (input[0] == '$')
        {
            string className = input.Substring(1, FindFirstOccurance(input, '(') - 1);

            if (className == "vector3")
            {
                return 1;
            }
            else if (className == "field")
            {
                return 2;
            }
            else
            {
                return 999; // used as a sort of error code here
            }
        }
        else
        {
            return 0;
        }
    }

    // one-stop-shop
    public static string ParseData(string rawData)
    {
        if (rawData.Length == 0) return rawData;
        
        if (rawData[0] == '$')
        {
            string className = rawData.Substring(1, FindFirstOccurance(rawData, '(') - 1);

            if (className == "vector3")
            {
                DoubleVector3 decoded = DoubleVector3.DecodeFromString(rawData);

                return "vector3(" + decoded.x + "," + decoded.y + "," + decoded.z + ")";
            }

            return "class error";
        }
        else
        {
            return rawData; // no need to parse as it is not a special class
        }
    }
    public static string[] Combine(string[] a, string[] b)
    {
        string[] result = new string[a.Length + b.Length];

        for (int i = 0; i < result.Length; i++)
        {
            if (i >= a.Length)
            {
                result[i] = b[i - a.Length];
            }
            else
            {
                result[i] = a[i];
            }
        }

        return result;
    }
    public static int[] Combine(int[] a, int[] b)
    {
        int[] result = new int[a.Length + b.Length];

        for (int i = 0; i < result.Length; i++)
        {
            if (i >= a.Length)
            {
                result[i] = b[i - a.Length];
            }
            else
            {
                result[i] = a[i];
            }
        }

        return result;
    }

    public static string[] SplitString(string input, char splitCharacter)
    {
        char openChar = '(';
        if (splitCharacter == '(') { openChar = '('; }
        if (splitCharacter == '[') { openChar = '['; }
        if (splitCharacter == '{') { openChar = '{'; }

        List<string> result = new List<string>();

        int startIndex = -1;
        int endIndex = -1;

        int limit = -1;

        for (int i = 0; i < input.Length; i++)
        {
            if (startIndex == -1 && input[i] == openChar && i > limit) // looking for the character that starts a split range
            {
                startIndex = i;
                endIndex = FindClosingBracket(input, startIndex);
            }

            if (startIndex != -1 && endIndex != -1)
            {
                result.Add(input.Substring(startIndex, endIndex - startIndex + 1));

                limit = endIndex;

                startIndex = -1;
                endIndex = -1;
            }
        }

        return result.ToArray();
    }

    public static string SnipString(string input, int dataStartIndex, int dataEndIndex)
    {
        string result = "";

        for (int i = 0; i < input.Length; i++)
        {
            if (i >= dataStartIndex && i <= dataEndIndex)
            {
                result += input[i].ToString();
            }
        }

        return result;
    }

    public static int FindClosingBracket(string input, int startingIndex)
    {
        char openChar = '(';
        char closeChar = ')';
        if (input[startingIndex] == '(') { openChar = '('; closeChar = ')'; }
        if (input[startingIndex] == '{') { openChar = '{'; closeChar = '}'; }
        if (input[startingIndex] == '[') { openChar = '['; closeChar = ']'; }

        int bracketChildLevel = 1;

        for (int i = startingIndex + 1; i < input.Length; i++)
        {
            if (input[i] == openChar)
            {
                bracketChildLevel++;
            }
            else if (input[i] == closeChar)
            {
                bracketChildLevel--;
            }

            if (bracketChildLevel == 0)
            {
                return i;
            }
        }

        return -1; // bc fuck you
    }

    public static string SubtractVariable(string input)
    {
        return SnipString(input, FindClosingBracket(input, 0) + 1, input.Length - 1);
    }

    public static int FindFirstOccurance(string input, char toLookFor)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == toLookFor)
            {
                return i;
            }
        }

        return -1; // because fuck you
    }

    public static int FindLastOccurance(string input, char toLookFor)
    {
        for (int i = input.Length - 1; i >= 0; i--)
        {
            if (input[i] == toLookFor)
            {
                return i;
            }
        }

        return -1; // because fuck you
    }

    public static int FindOccurance(string input, char toLookFor, int numSkips)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == toLookFor)
            {
                if (numSkips == 0)
                {
                    return i;
                }
                else
                {
                    numSkips--;
                }
            }
        }

        return -1; // because fuck you
    }

    public static int FindOccurance(string input, char toLookFor, int numSkips, int startIndex)
    {
        for (int i = startIndex; i < input.Length; i++)
        {
            if (input[i] == toLookFor)
            {
                if (numSkips == 0)
                {
                    return i;
                }
                else
                {
                    numSkips--;
                }
            }
        }

        return -1; // because fuck you
    }
}
