using UnityEngine;

public class TEMP : MonoBehaviour
{
    private NetworkTables n;
    void Start()
    {
        n = new NetworkTables();

        n.Initialize();
    }
}
