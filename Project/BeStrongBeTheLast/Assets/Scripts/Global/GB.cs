/*
MIT License
Copyright (c) 2019 Team Lama: Carrarini Andrea, Cerrato Loris, De Cosmo Andrea, Maione Michele
Author: Maione Michele
Contributors: 
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GB
{

    //Mantenere ordine corretto, serve per tornare indietro alla scena precedente con il tasto ESC
    public enum EScenes
    {
        StartMenu,
        TrackSelection,
        KartSelection,
        Game
    }

    public enum EAxis
    {
        X, Y, Z
    }

    public static HashSet<Color> usedColors = new HashSet<Color>();


    public static float GetAxis(string axisName)
    {
        try
        {
            return Input.GetAxis(axisName);
        }
        catch
        {

            return 0;
        }
    }

    public static bool GetButtonDown(string buttonName)
    {
        try
        {
            return Input.GetButtonDown(buttonName);
        }
        catch
        {

            return false;
        }
    }

    public static bool GetButtonUp(string buttonName)
    {
        try
        {
            return Input.GetButtonUp(buttonName);
        }
        catch
        {

            return false;
        }
    }

    public static bool between<A>(A item, A from_, A to_) where A : System.IComparable
    {
        return item.CompareTo(from_) >= 0 && item.CompareTo(to_) <= 0;
    }

    public static float NormalizedRandom(float minValue, float maxValue)
    {
        var mean = (minValue + maxValue) / 2f;
        var sigma = (maxValue - mean) / 3f;

        var r = Random.Range(mean, sigma);

        return r;
    }

    public static bool compareVector3(EAxis exclude, Vector3 a, Vector3 b)
    {
        var x = (a.x == b.x);
        var y = (a.y == b.y);
        var z = (a.z == b.z);

        switch (exclude)
        {
            case EAxis.X:
                return y && z;
            case EAxis.Y:
                return x && z;
            case EAxis.Z:
                return x && y;
            default:
                return false;
        }
    }

    public static bool CompareORNames(string name, params string[] names)
    {
        foreach (var n in names)
            if (name.Equals(n))
                return true;

        return false;
    }

    public static bool CompareORTags(Component component, params string[] tags)
    {
        foreach (var tag in tags)
            if (component.CompareTag(tag))
                return true;

        return false;
    }

    public static bool CompareORTags(GameObject go, params string[] tags)
    {
        foreach (var tag in tags)
            if (go.CompareTag(tag))
                return true;

        return false;
    }

    public static bool CompareANDTags(Component component, params string[] tags)
    {
        var x = 0;

        foreach (var tag in tags)
            if (component.CompareTag(tag))
                x++;

        return x == tags.Length;
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

    public static Transform RootParent(Transform t)
    {
        if (t.parent == null)
            return t;
        else
            return RootParent(t.parent);
    }

    public static Transform RootParentUntilTag(Transform t, string tag)
    {
        if (t == null)
            return null;

        if (t.CompareTag(tag))
            return t;

        return RootParentUntilTag(t.parent, tag);
    }

    public static void AbilitaButton(UnityEngine.UI.Button b, bool abilitato)
    {
        var img = b.GetComponent<UnityEngine.UI.Image>();
        img.color = (abilitato ? Color.white : Color.gray);

        b.enabled = abilitato;
    }

    public static Transform FindTransformInChildWithName(Transform parent, string name)
    {
        foreach (Transform tr in parent)
            if (tr.gameObject.name.Equals(name))
            {
                return tr;
            }
            else
            {
                var R = FindTransformInChildWithName(tr, name);

                if (R != null)
                    return R;
            }

        return null;
    }

    public static List<Transform> FindTransformsInChildWithTag(Transform parent, string tag)
    {
        var R = new List<Transform>();

        foreach (Transform tr in parent)
        {
            if (tr.gameObject.CompareTag(tag))
                R.Add(tr);

            var z = FindTransformsInChildWithTag(tr, tag);

            R.AddRange(z);
        }

        return R;
    }

    public static Transform FindTransformInChildWithTag(Transform parent, string tag)
    {
        foreach (Transform tr in parent)
            if (tr.gameObject.CompareTag(tag))
            {
                return tr;
            }
            else
            {
                var R = FindTransformInChildWithTag(tr, tag);

                if (R != null)
                    return R;
            }

        return null;
    }

    public static T FindComponentInChildWithTag<T>(GameObject parent, string tag) where T : Component
    {
        var t = parent.transform;

        foreach (Transform tr in t)
            if (tr.CompareTag(tag))
                return tr.GetComponent<T>();

        return null;
    }

    public static T FindComponentInDadWithName<T>(Transform me, string name_) where T : Component
    {
        if (me.name.Contains(name_))
            return me.GetComponent<T>();

        return FindComponentInDadWithName<T>(me.transform.parent, name_);
    }

    public static List<GameObject> FindGameObjectsInChildWithTag(Transform parent, string tag)
    {
        var R = new List<GameObject>();

        foreach (Transform tr in parent)
            if (tr.CompareTag(tag))
                R.Add(tr.gameObject);
            else
                R.AddRange(FindGameObjectsInChildWithTag(tr, tag));

        return R;
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
        var p = (name.Contains("Scenes/Menus/") ? name : $"Scenes/Menus/{name}");

        SceneManager.LoadScene(p, LoadSceneMode.Single);
    }

    public static void GotoScene(EScenes scene_name)
    {
        var name = scene_name.ToString();

        GotoSceneName(name);
    }

}