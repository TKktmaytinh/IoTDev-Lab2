using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

//namespace AppManager
//{
//    public class Status_Data
//    {
//        public string temperature { get; set; }
//        public string humidity { get; set; }
//    }

//    public class data_ss
//    {
//        public string ss_name { get; set; }
//        public string ss_unit { get; set; }
//        public string ss_value { get; set; }
//    }

//    public class AppManager : MonoBehaviour
//    {
//        [SerializeField]
//        private Text temperature;
//        [SerializeField]
//        private Text humidity;
//        [SerializeField]
//        private ToggleGroup status_led;
//        [SerializeField]
//        private ToggleGroup status_pump;

//        /// <summary>
//        /// General elements
//        /// </summary>

//        public void Update_Status(Status_Data _status_data)
//        {
//            foreach (data_ss _data in _status_data.data_ss)
//            {
//                switch (_data.ss_name)
//                {
//                    case "temperature":
//                        temperature.text = _data.ss_value;
//                        break;

//                    case "humidity":
//                        humidity.text = _data.ss_value;
//                        break;
//                        //case "device_status":
//                        //    Debug.Log("_data.ss_value " + _data.ss_value);
//                        //    if (_data.ss_value == "1")
//                        //        _btn_config.interactable = true;

//                        //    break;
//                }

//            }
//        }

//        //public void Update_Control(ControlFan_Data _control_data)
//        //{
//        //    if (_control_data.device_status == 1)
//        //    {
//        //        LampControl.interactable = true;
//        //        if (_control_data.fan_status == 1)
//        //            LampControl.isOn = true;
//        //        else
//        //            LampControl.isOn = false;
//        //    }

//        //}

//        //public ControlFan_Data Update_ControlFan_Value(ControlFan_Data _controlFan)
//        //{
//        //    _controlFan.device_status = 0;
//        //    _controlFan.fan_status = (LampControl.isOn ? 1 : 0);
//        //    LampControl.interactable = false;
//        //    return _controlFan;
//        //}
//    }
//}


namespace AppMQTT
{
    public class Status_Data
    {
        public string temperature { get; set; }
        public string humidity { get; set; }
    }

    public class data_ss
    {
        public string ss_name { get; set; }
        public string ss_unit { get; set; }
        public string ss_value { get; set; }
    }


    public class Control_Led
    {
        public int led_status { get; set; }
        public int device_status { get; set; }

    }

    public class Control_Pump
    {
        public int pump_status { get; set; }
        public int device_status { get; set; }

    }

    public class AppMQTT : M2MqttUnityClient
    {
        public List<string> topics = new List<string>();


        public string msg_received_from_topic_status = "";
        public string msg_received_from_topic_control = "";


        private List<string> eventMessages = new List<string>();
        [SerializeField]
        public Status_Data _status_data;
        [SerializeField]
        public Control_Led _control_led;
        [SerializeField]
        public Control_Pump _control_pump;



        //public void PublishConfig()
        //{
        //    _config_data = new Config_Data();
        //    //GetComponent<AppManager>().Update_Config_Value(_config_data);
        //    string msg_config = JsonConvert.SerializeObject(_config_data);
        //    client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        //    Debug.Log("publish config");
        //}

        //public void PublishFan()
        //{
        //    //_controlFan_data = GetComponent<AppManager>().Update_ControlFan_Value(_controlFan_data);
        //    string msg_config = JsonConvert.SerializeObject(_controlFan_data);
        //    client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        //    Debug.Log("publish fan");


        //}

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }


        protected override void OnConnecting()
        {
            base.OnConnecting();
            //SetUiMessage("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }

        protected override void OnConnected()
        {
            base.OnConnected();

            SubscribeTopics();

        }

        protected override void SubscribeTopics()
        {

            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    Debug.Log("SUB");
                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                    Debug.Log("UNSUB");
                }
            }

        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
        }



        protected override void Start()
        {

            base.Start();
            Debug.Log("Start");
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            Debug.Log("Receive Message");
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            //StoreMessage(msg);
            if (topic == topics[0])
                ProcessMessageStatus(msg);

            if (topic == topics[1])
                ProcessMessageControl(msg);

        }

        private void ProcessMessageStatus(string msg)
        {
            _status_data = JsonConvert.DeserializeObject<Status_Data>(msg);
            msg_received_from_topic_status = msg;
            //GetComponent<AppManager>().Update_Status(_status_data);

        }

        private void ProcessMessageControl(string msg)
        {
            _control_led = JsonConvert.DeserializeObject<Control_Led>(msg);
            msg_received_from_topic_control = msg;
            //GetComponent<AppManager>().Update_Control(_controlFan_data);

        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            //if (autoTest)
            //{
            //    autoConnect = true;
            //}
        }

        public void UpdateConfig()
        {

        }

        public void UpdateControl()
        {

        }
    }
}




