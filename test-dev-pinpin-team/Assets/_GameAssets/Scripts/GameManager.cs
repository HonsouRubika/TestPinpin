using UnityEngine;

namespace Pinpin
{
    public class GameManager : MonoBehaviour
    {
        [Header("References")]

        [SerializeField] private GameConfig gameConfig;
        public GameConfig GameConfig => m_instance.gameConfig;

        [SerializeField] private ParticleSystem[] m_psArray;
        public LayerMask groundLayerMask;

        public static GameManager Instance => m_instance;
        [SerializeField] private Camera m_mainCamera;
        public static Camera mainCamera => Instance.m_mainCamera;

        [SerializeField] private PlayerCharacter playerCharacter;
        public PlayerCharacter Player => m_instance.playerCharacter;

        [SerializeField] private UIManager uiManager;
        public UIManager UIManager => m_instance.uiManager;

        [SerializeField] private TreeSpawner treeSpawner;
        public TreeSpawner TreeSpawner => m_instance.treeSpawner;

        [SerializeField] private AudioManager audioManager;
        public AudioManager AudioManager => m_instance.audioManager;

        private static GameManager m_instance;

        //score
        public int WoodCount;
        public int AmountTreeCut;


        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            //load save
            if (PlayerPrefs.HasKey("WoodCount"))
            {
                Debug.Log("Load previous save");
                WoodCount = PlayerPrefs.GetInt("WoodCount");
                AmountTreeCut = PlayerPrefs.GetInt("AmountTreeCut");
                Player.AddChoppingSpeedBoost(PlayerPrefs.GetInt("ChoppingSpeedMultiplicator"));

                UIManager.ChangeTreeAmount(WoodCount);
            }
            else
            {
                Debug.Log("Create save");
                PlayerPrefs.SetInt("WoodCount", 0);
                PlayerPrefs.SetInt("AmountTreeCut", 0);
                PlayerPrefs.SetFloat("ChoppingSpeedMultiplicator", 0);
                PlayerPrefs.Save();

                WoodCount = 0;
                AmountTreeCut = 0;
            }
        }

        public void NewTreeInStorage()
        {
            AmountTreeCut++;
            WoodCount += 5;

            //save change
            PlayerPrefs.SetInt("WoodCount", WoodCount);
            PlayerPrefs.SetInt("AmountTreeCut", AmountTreeCut);
            PlayerPrefs.Save();

            UIManager.ChangeTreeAmount(WoodCount);

            //total amount of tree is a multiple of 10
            if (AmountTreeCut % 10 == 0)
                LevelWon();
        }

        public bool UseWood(int woodAmount)
        {
            if (WoodCount < woodAmount) return false;
            else
            {
                WoodCount -= woodAmount;

                //save change
                PlayerPrefs.SetInt("WoodCount", WoodCount);
                PlayerPrefs.SetInt("AmountTreeCut", AmountTreeCut);
                PlayerPrefs.Save();

                UIManager.ChangeTreeAmount(WoodCount);
                return true;
            }
        }

        [ContextMenu("LevelWon")]
        public void LevelWon()
        {
            foreach (ParticleSystem ps in m_psArray)
                ps.Play();

            //play victory sfx
            AudioManager.VictorySFX();
            Debug.Log("LEVEL WON!");
        }

    }
}
