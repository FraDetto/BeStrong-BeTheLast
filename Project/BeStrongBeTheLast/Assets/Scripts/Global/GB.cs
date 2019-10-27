/*
MIT License
Copyright (c) 2019 Team Lama: Carrarini Andrea, Cerrato Loris, De Cosmo Andrea, Maione Michele
Author: Maione Michele
Contributors: 
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GB
{

    //Mantenere ordine corretto, serve per tornare indietro alla scende precedente con il tasto ESC
    public enum EScenes
    {
        StartTitle,
        PlayerSelection,
        LobbyM,
        Game
    }

    public static void PlayCarEngine(AudioSource carAudioSource, float generalCar_actualSpeed)
    {
        if (carAudioSource != null)
        {
            carAudioSource.volume = Mathf.Min(generalCar_actualSpeed / 40, 1);

            if (!carAudioSource.isPlaying && carAudioSource.volume > 0)
                carAudioSource.Play();
        }
    }

    public static void DistruggiOggetti(Object[] oggetti)
    {
        foreach (var o in oggetti)
            Object.Destroy(o);
    }

    public static void AbilitaButton(UnityEngine.UI.Button b, bool abilitato)
    {
        var img = b.GetComponent<UnityEngine.UI.Image>();
        img.color = (abilitato ? Color.white : Color.gray);

        b.enabled = abilitato;
    }

    public static T FindComponentInChildWithTag<T>(GameObject parent, string tag) where T : Component
    {
        var t = parent.transform;

        foreach (Transform tr in t)
            if (tr.CompareTag(tag))
                return tr.GetComponent<T>();

        return null;
    }

    public static double ms_to_kmh(float meters_per_seconds)
    {
        return meters_per_seconds * 3.6;
    }

    public static double ms_to_mph(float meters_per_seconds)
    {
        return meters_per_seconds * 2.237;
    }

    public static T getRandomEnum<T>()
    {
        var values = System.Enum.GetValues(typeof(T));
        var random = Random.Range(0, values.Length);

        return (T)values.GetValue(random);
    }

    public static void SwitchFullScreen()
    {
        if (Screen.fullScreen)
        {
            Screen.fullScreen = false;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Screen.fullScreen = true;
        }
    }

    public static void GoBackToScene()
    {
        var s = SceneManager.GetActiveScene();
        var i = s.buildIndex - 1;

        if (i < 0)
        {
            Application.Quit();
        }
        else
        {
            var e = (EScenes)i;
            GotoScene(e);
        }
    }

    public static void GotoSceneName(string name)
    {
        var LobbyMName = EScenes.LobbyM.ToString();
        var p = (name.Contains("Scenes/") ? name : $"Scenes/{name}");

        SceneManager.LoadScene(p, LoadSceneMode.Single);
    }

    public static void GotoScene(EScenes scene_name)
    {
        var name = scene_name.ToString();

        GotoSceneName(name);
    }


}