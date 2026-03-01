using TMPro;
using UnityEngine;

public class DataPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stateLabel;
    [SerializeField] private Knight player;

    private void Update()
    {
        stateLabel.text = "State: " + player.GetCurrentStateString();
    }
}
