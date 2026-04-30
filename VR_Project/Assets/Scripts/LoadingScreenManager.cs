using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private float minimumLoadingTime = 3f;

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(minimumLoadingTime);

        AsyncOperation operation = SceneManager.LoadSceneAsync(LoadingData.sceneToLoad);

        while (!operation.isDone)
        {
            yield return null;
        }
    }
}