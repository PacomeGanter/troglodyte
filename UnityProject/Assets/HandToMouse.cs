using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;


public class HandToMouse : MonoBehaviour
{

    Windows.Kinect.Body body;

    private void Start()
    {
        Windows.Kinect.Joint joint = new Windows.Kinect.Joint();
        Debug.Log(joint.Position);
    }




}
