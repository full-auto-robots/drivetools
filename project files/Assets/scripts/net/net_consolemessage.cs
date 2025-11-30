using UnityEngine;

[System.Serializable]
public class net_consolemessage
{
    public string message;
    public string tag;

    public net_consolemessage() { }

    public net_consolemessage(string message, string tag)
    {
        this.message = message;
        this.tag = tag;
    }
}
