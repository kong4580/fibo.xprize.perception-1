using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

using System;

public class mqtest : MonoBehaviour
{
    public GameObject vrcam;
    private MqttClient client;
    float init_roll = 0;
    float init_pitch = 0;
    float init_yaw = 0;
    int state_start = 0;
    // Use this for initialization
    void Start()
    {
        // create client instance 
        client = new MqttClient(IPAddress.Parse("127.0.0.1"), 1883, false, null);

        // register to message received 
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId);

        // subscribe to the topic "/home/temperature" with QoS 2 
        client.Subscribe(new string[] { "hello/world" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        //var init_ang = vrcam.transform.rotation.eulerAngles;
        //init_roll = init_ang[2];
        //init_pitch = init_ang[0];
        //init_yaw = init_ang[1];
    }
    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {

        Debug.Log("Received: " + System.Text.Encoding.UTF8.GetString(e.Message));
    }
    float update_value(float start, float current){
        current = current - start;
        if (current < 0){
            current = 360 + current;
        }
        if (current >= 180){
            current = current - 360;
        }
        return current;
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 40, 80, 20), "Level 1"))
        {
            Debug.Log("sending...");
            var init_ang = vrcam.transform.rotation.eulerAngles;
            var cur_roll = init_ang[2];
            var cur_pitch = init_ang[0];
            var cur_yaw = init_ang[1];
            
            init_roll = cur_roll;
            init_pitch = cur_pitch;
            init_yaw = cur_yaw;
            state_start = 1;
            Debug.Log("sent");
        }
    }
    // Update is called once per frame
    void Update()
    {
        var pos = vrcam.transform.position;
        var ang = vrcam.transform.rotation.eulerAngles;
        var cur_roll = ang[2] ;
        var cur_pitch = ang[0] ;
        var cur_yaw = ang[1] ;
        Debug.Log(cur_yaw);
        cur_roll = update_value(init_roll, cur_roll);
        cur_pitch = update_value(init_pitch, cur_pitch);
        cur_yaw = update_value(init_yaw, cur_yaw);


        
        var roll = cur_roll.ToString();
        var pitch = cur_pitch.ToString();
        var yaw = cur_yaw.ToString();
        var all = roll +","+ pitch +","+ yaw;
        //Debug.Log(vrcam.transform.position);
        var topic_position_roll = "/operator/roll";
        var topic_position_pitch ="/operator/pitch";
        var topic_position_yaw = "/operator/yaw";
        var topic_position_rotation = "/operator/rotation";

        //Debug.Log(roll);
        client.Publish(topic_position_roll, System.Text.Encoding.UTF8.GetBytes(roll), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        client.Publish(topic_position_pitch, System.Text.Encoding.UTF8.GetBytes(pitch), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        client.Publish(topic_position_yaw, System.Text.Encoding.UTF8.GetBytes(yaw), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        if (state_start==1)
        {
            client.Publish(topic_position_rotation, System.Text.Encoding.UTF8.GetBytes(all), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }
        
        Debug.Log("sent");


    }
}
