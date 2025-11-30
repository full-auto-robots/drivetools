using UnityEngine.UI;
using UnityEngine;

// this one's a little different than most nodes,
// bc its job is to send data TO the robot

// all buttons use the "$button_commands" nt key, and post their command names in the same queue-style as the console

public class dsp_button : MonoBehaviour
{
    public string commandName;

    public void Initialize()
    {
        
    }

    // called when the button is pressed
    // just adds the command name to the queue
    public void Post()
    {
        string existingQueue = NetworkManager.Instance.latestData.GetValueAt("$button_commands");
        existingQueue += "{" + commandName + "}";

        NetworkManager.Instance.SetValueAt("$button_commands", existingQueue);
    }
}
