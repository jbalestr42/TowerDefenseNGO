using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class RoleSelection : MonoBehaviour
{
    [SerializeField]
    ScreenFade _screenFade;

    public UnityEvent OnClientRoleSelected = new UnityEvent();

    [SerializeField]
    string _sceneToLoad = "";

    bool _shouldFade = false;

    void Start()
    {
        if (ConfigManager.instance.GetBoolValue("no_lobby"))
        {
            string role = ConfigManager.instance.GetValue("role");
            switch (role)
            {
                case "player":
                    StartPlayerScene();
                    break;
                case "actor":
                    StartActorScene();
                    break;
                case "observer":
                    StartObserverScene();
                    break;
                default:
                    StartPlayerScene();
                    break;
            }
        }
        else
        {
            _screenFade.FadeOut(1f);
            _shouldFade = true;
        }
    }

    public void StartPlayerScene()
    {
        PlayerPrefs.SetString("role", "client");
        PlayerPrefs.SetString("client_role", ClientRole.Player.ToString());
        OnClientRoleSelected.Invoke();
        LoadScene();
    }

    public void StartObserverScene()
    {
        PlayerPrefs.SetString("role", "client");
        PlayerPrefs.SetString("client_role", ClientRole.Observer.ToString());
        OnClientRoleSelected.Invoke();
        LoadScene();
    }

    public void StartActorScene()
    {
        PlayerPrefs.SetString("role", "client");
        PlayerPrefs.SetString("client_role", ClientRole.Actor.ToString());
        OnClientRoleSelected.Invoke();
        LoadScene();
    }

    public void StartHostScene()
    {
        PlayerPrefs.SetString("role", "host");
        LoadScene();
    }

    public void StartServerScene()
    {
        PlayerPrefs.SetString("role", "server");
        LoadScene();
    }

    private void LoadScene()
    {
        if (_shouldFade)
        {
            _screenFade.FadeIn(1f, () => SceneManager.LoadScene(_sceneToLoad, LoadSceneMode.Single));
        }
        else
        {
            StartCoroutine(LoadSceneCor());
        }
    }

    private IEnumerator LoadSceneCor()
    {
        //SceneManager.LoadScene(_sceneToLoad, LoadSceneMode.Single);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            Debug.Log(asyncLoad.progress);
            yield return null;
        }
        Debug.Log(asyncLoad.progress);
    }
}