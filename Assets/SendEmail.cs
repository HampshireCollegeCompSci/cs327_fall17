using UnityEngine;
using System.Collections;

public class SendEmail : MonoBehaviour
{
    [SerializeField]
    [Tooltip("email Id to send the mail to")]
    string email = "cs327feedback@lists.hampshire.edu";

    public void EmailUs()
    {
        //subject of the mail
        string subject = MyEscapeURL("Feedback/Suggestion");
        //body of the mail which consists of Device Model and its Operating System
        string body = MyEscapeURL("Please Enter your message here\n\n\n\n" +
         "________" +
         "\n\nPlease Do Not Modify This\n\n" +
         "Model: " + SystemInfo.deviceModel + "\n\n" +
            "OS: " + SystemInfo.operatingSystem + "\n\n" +
         "________");
        //Open the Default Mail App
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
}