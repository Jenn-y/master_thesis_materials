using UnityEngine;
using VIVE.OpenXR;

public static class XrExtensions
{
    public static Vector3 ToUnityVector(this XrVector3f vec)
    {
        return new Vector3(vec.x, vec.y, vec.z);
    }

    public static Quaternion ToUnityQuaternion(this XrQuaternionf quat)
    {
        return new Quaternion(quat.x, quat.y, quat.z, quat.w);
    }

    public static Vector3 Forward(this XrPosef pose)
    {
        return pose.orientation.ToUnityQuaternion() * Vector3.forward;
    }

    public static Vector3 Position(this XrPosef pose)
    {
        return pose.position.ToUnityVector();
    }
}