package frc.utils.dto.m;

import java.util.LinkedList;
import java.util.List;

import edu.wpi.first.math.geometry.Pose2d;
import edu.wpi.first.math.geometry.Pose3d;
import frc.utils.dto.DTUtils;

// on the c# end (in Drivetools) this is referred to as DoubleVector3
// but since doubles are the norm here I'm just calling it a Vector3

public class Vector3 {
    public double x;
    public double y;
    public double z;

    public static Vector3 one = new Vector3(1,1,1);
    public static Vector3 zero = new Vector3(0,0,0);

    // TODO: think about changing default vectors to match unity standard,
    // instead of WPILib standard
    public static Vector3 up = new Vector3(0,0,1);
    public static Vector3 down = new Vector3(0,0,-1);
    public static Vector3 left = new Vector3(0,1,0);
    public static Vector3 right = new Vector3(0,-1,0);
    public static Vector3 forward = new Vector3(1,0,0);
    public static Vector3 backward = new Vector3(-1,0,0);

    public Vector3() {
        this.x = 0;
        this.y = 0;
        this.z = 0;
    }

    public Vector3(double x, double y, double z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    // maybe unecessary, but it could be useful
    public Vector3(Pose2d p) {
        this.x = p.getX();
        this.y = p.getY();
        this.z = 0;
    }
    public Vector3(Pose3d p) {
        this.x = p.getX();
        this.y = p.getY();
        this.z = p.getZ();
    }

    // standard encoding/decoding functions
    public static String EncodeToString(Vector3 input)
    {
        String result = "";

        result += "vector3(";

        result += "{" + input.x + "}" + "{" + input.y + "}" + "{" + input.z + "}";

        result += ")";

        return "$" + result;
    }
    public static Vector3 DecodeFromString(String input)
    {
        List<String> components = new LinkedList<String>();

        for (int i = 1; i < 4; i++)
        {
            int startIndex = DTUtils.FindOccurance(input, "{", i);
            // System.out.println("START" + startIndex);
            int endIndex = DTUtils.FindClosingBracket(input, startIndex);
            // System.out.println("END" + endIndex);

            components.add(input.substring(startIndex + 1, endIndex));
        }

        return new Vector3(Double.parseDouble(components.get(0)), Double.parseDouble(components.get(1)), Double.parseDouble(components.get(2)));
    }

    // ONLY ROTATES X AND Y
    public Vector3 RotateDeg(double theta) {
        double a = theta * Math.PI / 180;
        return new Vector3(x * Math.cos(a) - y * Math.sin(a), y * Math.cos(a) + x * Math.sin(a), 0);
    }
    public Vector3 RotateRad(double theta) {
        double a = theta;
        return new Vector3(x * Math.cos(a) - y * Math.sin(a), y * Math.cos(a) + x * Math.sin(a), 0);
    }



    // *** math functions ***

    public static Vector3 add(Vector3 a, Vector3 b) {
        return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public Vector3 addTo(Vector3 b) {
        return new Vector3(x + b.x, y + b.y, z + b.z);
    }

    public static double magnitude(Vector3 a) {
        return Math.sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
    }
    public double magnitude() {
        return Math.sqrt(x * x + y * y + z * z);
    }

    public static Vector3 normalize(Vector3 a) {
        double length = magnitude(a);
        return new Vector3(a.x / length, a.y / length, a.z / length);
    }
    public Vector3 normalize() {
        double length = magnitude();
        return new Vector3(x / length, y / length, z / length);
    }

    public static double distance(Vector3 a, Vector3 b) {
        return Math.sqrt((b.x-a.x)*(b.x-a.x) + (b.y-a.y)*(b.y-a.y)+ (b.z-a.z)*(b.z-a.z));
    }
    public double distanceTo(Vector3 b) {
        return Math.sqrt((b.x-x)*(b.x-x) + (b.y-y)*(b.y-y)+ (b.z-z)*(b.z-z));
    }

    public static double dot(Vector3 a, Vector3 b) {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }
    public double dot(Vector3 b) {
        return x * b.x + y * b.y + b.z * z;
    }

    // the angle between two vectors in radians
    public static double angleBetween(Vector3 a, Vector3 b) {
        return Math.acos(dot(a, b) / Vector3.magnitude(a) / Vector3.magnitude(b));
    }
}