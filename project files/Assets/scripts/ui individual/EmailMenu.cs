using TMPro;
using UnityEngine;

public class EmailMenu : MonoBehaviour
{
    public TMP_Dropdown reportType;
    public TMP_InputField reportBodyText;

    public void SendReport()
    {
        string introString = reportType.value == 1 ? "A user has requested a feature: " : "A user has submitted a bug report: ";
        EmailHandler.SendEmail(introString + reportBodyText.text);
    }
}
