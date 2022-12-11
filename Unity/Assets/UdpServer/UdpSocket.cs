/*
Created by Youssef Elashry to allow two-way communication between Python3 and Unity to send and receive strings

Feel free to use this in your individual or commercial projects BUT make sure to reference me as: Two-way communication between Python 3 and Unity (C#) - Y. T. Elashry
It would be appreciated if you send me how you have used this in your projects (e.g. Machine Learning) at youssef.elashry@gmail.com

Use at your own risk
Use under the Apache License 2.0

Modified by: 
Youssef Elashry 12/2020 (replaced obsolete functions and improved further - works with Python as well)
Based on older work by Sandra Fang 2016 - Unity3D to MATLAB UDP communication - [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
*/

using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Random = UnityEngine.Random;

public class UdpSocket : MonoBehaviour
{
    [SerializeField] GameObject effect_carbeep;
    [SerializeField] GameObject effect_scream;
    [SerializeField] GameObject effect_siren;

    [SerializeField] GameObject effect_doorbell;
    [SerializeField] GameObject effect_glassbreaking;
    [SerializeField] GameObject effect_fire;
    [SerializeField] GameObject effect_microwave;
    
    [SerializeField] GameObject effect_heart;

    private float randomSeed = 1f;
    private string recognizedSound = "Others";

    [HideInInspector] public bool isTxStarted = false;

    [SerializeField] string IP = "127.0.0.1"; // local host
    [SerializeField] int rxPort = 8000; // port to receive data from Python on
    [SerializeField] int txPort = 8001; // port to send data to Python on

    // Create necessary UdpClient objects
    UdpClient client;
    IPEndPoint remoteEndPoint;
    Thread receiveThread; // Receiving Thread

    public void SendData(string message) // Use to send data to Python
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
    
    float next_spawn_time;
    void LateUpdate()
    {
        if (Time.time > next_spawn_time)
        {
            UpdateSoundEffect(recognizedSound);
            next_spawn_time += 0.5f;
        }
    }

    void Awake()
    {
        next_spawn_time = Time.time;
        randomSeed = Random.Range(1.0f, 1000.0f);
        
        // Create remote endpoint (to Matlab) 
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), txPort);

        // Create local client
        client = new UdpClient(rxPort);

        // local endpoint define (where messages are received)
        // Create a new thread for reception of incoming messages
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        // Initialize (seen in comments window)
        print("UDP Comms Initialised");

        //StartCoroutine(SendDataCoroutine()); // DELETE THIS: Added to show sending data from Unity to Python via UDP
    }

    // Receive data, update packets received
    private void ReceiveData()
    {
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                Debug.Log(text);
                recognizedSound = text;
                ProcessInput(text);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    private void ProcessInput(string input)
    {
        if (!isTxStarted) // First data arrived so tx started
        {
            isTxStarted = true;
        }
    }

    //Prevent crashes - close clients and threads properly!
    void OnDisable()
    {
        if (receiveThread != null)
            receiveThread.Abort();

        client.Close();
    }

    private int randomIndex = 1;
    Vector3 GetRandomPosition()
    {
        if (randomIndex > 10000)
            randomIndex = 1;
        return new Vector3(0f, y[(int)(Math.Abs((randomSeed + randomIndex) % 13))], z[(int)(Math.Abs((randomSeed - randomIndex) % 13))]);
    }

    float[] y = new float[] {-12f, -6f,  -2f, 0f, 2f, -4f, 10f, 4f, -10f, 6f, 8f, 12f, -8f};
    float[] z = new float[] {24f, -24f, -12f, -20f, -8f, 4f, -4f, 20f, 8f, 12f, -16f, 16f, 0f};

    void UpdateSoundEffect(string effect)
    {
        if (effect.Contains("Car_horn"))
        {
            Instantiate(effect_carbeep, GetRandomPosition(), Quaternion.identity);
            randomIndex += 1;    
        }
        else if(effect.Contains("Scream"))
        {
            Instantiate(effect_scream, GetRandomPosition(), Quaternion.identity);
            randomIndex += 2;    
        }
        else if(effect.Contains("Emergency_vehicle_siren"))
        {
            Instantiate(effect_siren, GetRandomPosition(), Quaternion.identity);
            randomIndex += 3;    
        }
        else if(effect.Contains("Doorbell"))
        {
            Instantiate(effect_doorbell, GetRandomPosition(), Quaternion.identity);
            randomIndex += 4;    
        }
        else if(effect.Contains("Glass_break"))
        {
            Instantiate(effect_glassbreaking, GetRandomPosition(), Quaternion.identity);
            randomIndex += 5;    
        }
        else if(effect.Contains("Fire_smoke_alarm"))
        {
            Instantiate(effect_fire, GetRandomPosition(), Quaternion.identity);
            randomIndex += 6;    
        }
        else if(effect.Contains("Appliance_alarm"))
        {
            Instantiate(effect_microwave, GetRandomPosition(), Quaternion.identity);
            randomIndex += 7;    
        }
        else if (effect.Contains("Clap") || effect.Contains("Applause"))
        {
            Instantiate(effect_heart, GetRandomPosition(), Quaternion.identity);
            randomIndex += 8;    
        }
    }
}