using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

// data class for stuff every single node has, like position and scale
// keep in mind this is written to disk, so only primitives allowed!
[System.Serializable]
public class dsp_genericdata
{
    // BOTTOM LEFT CORNER OF NODE
    public double posX, posY;
    // SIZE OF NODE FROM BOTTOM LEFT CORNER
    public double sizeX, sizeY;

    public ushort type;
    public string ntKey;

    // custom data, like the image arrays in image display nodes
    public List<string> propertyNames;
    public List<string> propertyValues;

    public dsp_genericdata() {propertyNames = new List<string>();
        propertyValues = new List<string>(); }
    // we want everything to be defined, so the constructor takes in all variables
    public dsp_genericdata(double posX, double posY, double sizeX, double sizeY, ushort type)
    {
        this.posX = posX;
        this.posY = posY;

        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.type = type;

        propertyNames = new List<string>();
        propertyValues = new List<string>();
    }
    public dsp_genericdata(double posX, double posY, double sizeX, double sizeY, ushort type, List<string> names, List<string> values)
    {
        this.posX = posX;
        this.posY = posY;

        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.type = type;

        propertyNames = names;
        propertyValues = values;
    }

    // we store position as floats because this class needs to be written to disk,
    // so the variables CAN ONLY BE PRIMITIVES
    // these functions act as a quick way to get back to the vectors unity knows and loves
    public Vector2 GetPosition() {
        return new Vector2((float)posX, (float)posY);
    }
    public Vector2 GetSize() {
        return new Vector2((float)sizeX, (float)sizeY);
    }

    // why can't vector classes just be primitive? there's probably a good reason
}