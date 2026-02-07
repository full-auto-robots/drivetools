package frc.utils.dto.m;

public class Vector2 {
    public double x;
    public double y;

    public static Vector2 one = new Vector2(1,1);
    public static Vector2 zero = new Vector2(0,0);

    // TODO: think about changing default vectors to match unity standard,
    // instead of WPILib standard
    public static Vector2 forward = new Vector2(1,0);
    public static Vector2 backward = new Vector2(-1,0);
    public static Vector2 left = new Vector2(0,1);
    public static Vector2 right = new Vector2(0,-1);

    public Vector2() {
        this.x = 0;
        this.y = 0;
    }

    public Vector2(double x, double y) {
        this.x = x;
        this.y = y;
    }



    // *** math functions ***

    public static Vector2 add(Vector2 a, Vector2 b) {
        return new Vector2(a.x + b.x, a.y + b.y);
    }
    public Vector2 addTo(Vector2 b) {
        return new Vector2(x + b.x, y + b.y);
    }

    public static Vector2 normalize(Vector2 a) {
        double length = Math.sqrt(a.x * a.x + a.y * a.y);
        return new Vector2(a.x / length, a.y / length);
    }
    public Vector2 normalize() {
        double length = Math.sqrt(x * x + y * y);
        return new Vector2(x / length, y / length);
    }

    public static double magnitude(Vector2 a) {
        return Math.sqrt(a.x * a.x + a.y * a.y);
    }
    public double magnitude() {
        return Math.sqrt(x * x + y * y);
    }

    // euclidean distance
    public static double distance(Vector2 a, Vector2 b) {
        return Math.sqrt((b.x-a.x)*(b.x-a.x) + (b.y-a.y)*(b.y-a.y));
    }
    public double distanceTo(Vector2 b) {
        return Math.sqrt((b.x-x)*(b.x-x) + (b.y-y)*(b.y-y));
    }

    public static double dot(Vector2 a, Vector2 b) {
        return a.x * b.x + a.y * b.y;
    }
    public double dot(Vector2 b) {
        return x * b.x + y * b.y;
    }

    // the angle between two vectors in radians
    public static double angleBetween(Vector2 a, Vector2 b) {
        return Math.acos(dot(a, b) / Vector2.magnitude(a) / Vector2.magnitude(b));
    }

    // projects a onto b
    public static Vector2 project(Vector2 a, Vector2 b) {
        if (Vector2.magnitude(a) == 0 || Vector2.magnitude(b) == 0) {return new Vector2(0,0);}
        double len = Vector2.magnitude(a) * Math.cos(Vector2.angleBetween(a, b));
        Vector2 result = Vector2.normalize(b);
        result = new Vector2(result.x * len, result.y * len);
        return result;
    }
    public Vector2 projectOnto(Vector2 b) {
        if (Vector2.magnitude(this) == 0 || Vector2.magnitude(b) == 0) {return new Vector2(0,0);}
        double len = Vector2.magnitude(this) * Math.cos(Vector2.angleBetween(this, b));
        Vector2 result = Vector2.normalize(b);
        result = new Vector2(result.x * len, result.y * len);
        return result;
    }

    // c is [0..1]
    public static Vector2 lerp(Vector2 a, Vector2 b, double c) {
        return new Vector2(a.x + (b.x-a.x) * c, a.y + (b.y-a.y)*c);
    }

    public static boolean equals(Vector2 a, Vector2 b) {
        if (a.x == b.x && a.y == b.y) {
            return true;
        } else {return false;}
    }
    public boolean equals(Vector2 b) {
        if (x == b.x && y == b.y) {
            return true;
        } else {return false;}
    }
}