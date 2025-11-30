using UnityEngine;

public class SimulatedRobot : MonoBehaviour
{
    public bool isActive;

    public DoubleVector3 position;
    public double zAngle;
    public float moveSpeed;

    private float lastConsolePush;

    private int testValue;

    void Update()
    {
        if (isActive)
        {
            position = position.Add(new DoubleVector3((double)Input.GetAxis("Vertical") * Time.deltaTime, -(double)Input.GetAxis("Horizontal") * Time.deltaTime, 0));
            if (Input.GetKey("q")) zAngle += 180 * Time.deltaTime;
            if (Input.GetKey("e")) zAngle -= 180 * Time.deltaTime;

            net_fakent.WriteString("position", position.EncodeToString());

            nav_field fieldData = new nav_field();
            fieldData.markers = new nav_marker[]
            {
                new nav_marker(8, Vector3.zero, 0.0f)
            };
            // diamond around the robot
            fieldData.paths = new nav_path[] {new nav_path(new DoubleVector3[] {
                position.Add(new DoubleVector3(-1,0,0)),
                position.Add(new DoubleVector3(0,1,0)),
                position.Add(new DoubleVector3(1,0,0)),
                position.Add(new DoubleVector3(0,-1,0)),
            }, (ushort)LineDisplayMode.LoopedLine)};
            fieldData.robots = new nav_robot[] { new nav_robot(2386, position, zAngle, 0.5d, 0.5d, Color.blue) };

            net_fakent.WriteString("x", "x: " + position.x.ToString());
            net_fakent.WriteString("y", "y: " + position.y.ToString());
            net_fakent.WriteString("rot", "rotation: " + zAngle.ToString());

            net_fakent.WriteString("time", Time.time.ToString());
            net_fakent.WriteString("field", fieldData.EncodeToString());
            net_fakent.WriteString("robot", nav_robot.EncodeToString(new nav_robot(2386, position, zAngle, 0.5d, 0.5d)));

            if (Time.time > lastConsolePush + 1) { net_fakent.PostToConsole("main", Mathf.Round(Time.time).ToString() + "*"); lastConsolePush = Time.time; }

            if (Input.GetKeyDown("f"))
            {
                testValue++;
                if (testValue > 4)
                {
                    testValue = 0;
                }
            }
            
            net_fakent.WriteString("int", testValue.ToString());
        }
    }
}
