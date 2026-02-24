package frc.utils.dto;

import edu.wpi.first.wpilibj.util.Color;

public class nav_robot
{
    public int teamNumber;

    public double xPosition;
    public double yPosition;
    public double zPosition;

    public double zAngle;

    public double width; // (m)
    public double length; // (m)

    public Color color;

    public nav_robot() { }

    public nav_robot(int teamNumber, double xPosition, double yPosition, double zPosition, double zAngle, double width, double length)
    {
        this.xPosition = xPosition;
        this.yPosition = yPosition;
        this.zPosition = zPosition;

        this.teamNumber = teamNumber;

        this.zAngle = zAngle;
        this.width = width;
        this.length = length;

        this.color = Color.kWhite;
    }

    // as of 02/24/2026, complies with dto syntax standard
    public static String EncodeToString(nav_robot input)
    {
        String result = "";

        result += "robot(";

        // OTHER than the mandatory entry name, I'm not naming any of the variables
        // this is to save bandwidth, which I know doesn't really matter at this scale but I'm doing it anyway

        // team number
        result += "{" + input.teamNumber + "}";

        // position
        result += "{" + "{" + input.xPosition + "}" + "{" + input.yPosition + "}" + "{" + input.zPosition + "}" + "}"; // extra set of {} for the vector3 class
        // rotation
        result += "{" + input.zAngle + "}";

        // width
        result += "{" + input.width + "}";
        // length
        result += "{" + input.length + "}";

        // color
        result += "{" + "{" + input.color.red + "}" + "{" + input.color.green + "}" + "{" + input.color.blue + "}" + "}"; // extra set of {} for the color
        
        result += ")";
        return result;
    }
}