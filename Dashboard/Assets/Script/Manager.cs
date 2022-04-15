using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public Text _text;
    public InputField _username;
    public InputField _password;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Display()
    {
        //_text.text = "Hello World";
        _text.text = "Username does not exist";
    }

    public void ChangeScene()
    {
        if (_username.text == "bkiot" && _password.text == "12345678") {
            SceneManager.LoadScene("Scenes 2");
        }
        else _text.text = "Username or Password does not matched";
        
    }
}
