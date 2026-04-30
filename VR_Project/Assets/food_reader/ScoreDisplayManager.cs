using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayManager : MonoBehaviour
{
    [Header("UI")]
    public Text scoreText;

    private OrderSystem connectedOrderSystem;

    public void ConnectToOrderSystem(OrderSystem orderSystem)
    {
        if (connectedOrderSystem != null)
        {
            connectedOrderSystem.OnTotalScoreChanged -= UpdateScoreDisplay;
        }

        connectedOrderSystem = orderSystem;

        if (connectedOrderSystem != null)
        {
            connectedOrderSystem.OnTotalScoreChanged += UpdateScoreDisplay;
            UpdateScoreDisplay(connectedOrderSystem.TotalScore);
        }
    }

    private void OnDestroy()
    {
        if (connectedOrderSystem != null)
        {
            connectedOrderSystem.OnTotalScoreChanged -= UpdateScoreDisplay;
        }
    }

    private void UpdateScoreDisplay(int totalScore)
    {
        if (scoreText == null)
        {
            Debug.LogWarning("ScoreText is not assigned on ScoreDisplayManager.");
            return;
        }

        scoreText.text = totalScore.ToString();

        Debug.Log($"[ScoreDisplayManager] Score display updated: {totalScore}");
    }
}