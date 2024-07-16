using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Widgets
{
    public class NetworkManagerWidget : MonoBehaviour
    {
        [SerializeField] private GameObject connectionInfoObject;
        [SerializeField] private TextMeshProUGUI debugInfoLabel;

        [SerializeField] private Button host1vs1;
        [SerializeField] private Button host2vs2;
        [SerializeField] private Button clientButton;
        [SerializeField] private Button leaveButton;

        [SerializeField] private GameObject mainCam;

        [SerializeField] GameManager gameManager;

        private void Awake()
        {
            host1vs1.onClick.AddListener(() => { gameManager.Start1vs1(); leaveButton.gameObject.SetActive(true); mainCam.SetActive(false); NetworkManager.Singleton.StartHost(); connectionInfoObject.SetActive(false); debugInfoLabel.gameObject.SetActive(true); debugInfoLabel.text = "Host"; });
            host2vs2.onClick.AddListener(() => { gameManager.Start2vs2(); leaveButton.gameObject.SetActive(true); mainCam.SetActive(false); NetworkManager.Singleton.StartHost(); connectionInfoObject.SetActive(false); debugInfoLabel.gameObject.SetActive(true); debugInfoLabel.text = "Host"; });
            clientButton.onClick.AddListener(() => { leaveButton.gameObject.SetActive(true); mainCam.SetActive(false); NetworkManager.Singleton.StartClient(); connectionInfoObject.SetActive(false); debugInfoLabel.gameObject.SetActive(true); debugInfoLabel.text = "Client"; });
            leaveButton.onClick.AddListener(() => { mainCam.SetActive(true); NetworkManager.Singleton.Shutdown(); connectionInfoObject.SetActive(true); debugInfoLabel.gameObject.SetActive(true); debugInfoLabel.text = "hub"; });
        }
    }
}
