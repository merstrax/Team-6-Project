using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class settingsMenuFunctions : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] Slider volumeMaster;
    [SerializeField] Slider volumeEffects;
    [SerializeField] Slider volumeBackground;

    [SerializeField] AudioMixer volumeMixer;

    [Header("Mouse")]
    [SerializeField] Slider mouseNormal;
    [SerializeField] Slider mouseADS;

    void Start()
    {
        
        float _vol = PlayerPrefs.GetFloat("volumeMaster", 0.0f);
        volumeMaster.value = ((_vol + 80) / 80) * 100;
        UpdateSlider(volumeMaster);

        _vol = PlayerPrefs.GetFloat("volumeEffects", 0.0f);
        volumeEffects.value = ((_vol + 80) / 80) * 100;
        UpdateSlider(volumeEffects);

        _vol = PlayerPrefs.GetFloat("volumeBG", 0.0f);
        volumeBackground.value = ((_vol + 80) / 80) * 100;
        UpdateSlider(volumeBackground);

        mouseNormal.value = PlayerPrefs.GetFloat("mouseNormal", 100.0f);
        UpdateSlider(mouseNormal);

        mouseADS.value = PlayerPrefs.GetFloat("mouseADS", 100.0f);
        UpdateSlider(mouseADS);

        LoadSettings();

        gameObject.SetActive(false);
    }

    void LoadSettings()
    {
        ChangeVolume();
        ChangeSensitivity();
    }

    public void Accept()
    {
        ChangeVolume();
        ChangeSensitivity();
        gameManager.instance.ToggleSettings();
    }

    public void AcceptMainMenu(GameObject settings)
    {
        ChangeVolume();
        ChangeSensitivity();
        settings.SetActive(false);
    }

    public void ChangeVolume()
    {
        float _val = (80 * (volumeMaster.value / 100)) - 80;
        volumeMixer.SetFloat("volumeMaster", _val);
        PlayerPrefs.SetFloat("volumeMaster", _val);

        _val = (80 * (volumeEffects.value / 100)) - 80;
        volumeMixer.SetFloat("volumeEffects", _val);
        PlayerPrefs.SetFloat("volumeEffects", _val);

        _val = (80 * (volumeBackground.value / 100)) - 80;
        volumeMixer.SetFloat("volumeBG", _val);
        PlayerPrefs.SetFloat("volumeBG", _val);
    }

    public void ChangeSensitivity()
    {
        PlayerPrefs.SetFloat("mouseNormal", mouseNormal.value);
        PlayerPrefs.SetFloat("mouseADS", mouseADS.value);
        if(SceneManager.GetActiveScene().name != "Main Menu")
            Camera.main.GetComponent<cameraController>().ChangeSensitivity(mouseNormal.value, mouseADS.value);
    }

    public void UpdateSlider(Slider slider)
    {
        slider.GetComponentInChildren<TMP_InputField>().text = slider.value.ToString("N0");
    }

    public void UpdateInput(Slider slider)
    {
        string _value = slider.GetComponentInChildren<TMP_InputField>().text;
        if (_value.Length > 0)
            slider.value = float.Parse(_value);
    }
}
