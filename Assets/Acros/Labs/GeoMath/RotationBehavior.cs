using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationBehavior : MonoBehaviour {

    public enum RotMode
    {
        None,
        Quaternion,
        Euler,
        Matrix
    }

    public enum RotType
    {
        World,
        Local,
    }

    public RotType rotType;
    public RotMode rotMode;

    public Vector3 rotSpeed = new Vector3(0,2.0f,0);

    private Quaternion quat;

	void Update () {

        switch (rotMode)
        {
            case RotMode.None:
                break;
            case RotMode.Quaternion:
                QuaternionRot();
                break;
            case RotMode.Euler:
                break;
            case RotMode.Matrix:
                break;
            default:
                break;
        }

    }

    void QuaternionRot()
    {
        quat = Quaternion.Euler(Time.deltaTime * rotSpeed);

        if (rotType == RotType.World)
        {
            //Rotate by Axis in world space
            this.transform.rotation = quat * this.transform.rotation;
        }
        else if (rotType == RotType.Local)
        {
            // Rotation by Axis in local space- 【quat】
            this.transform.rotation *= quat;
        }
        else 
        {
            Debug.Assert(false);
        }
    }
}
