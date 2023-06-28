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

        [SerializeField] private Chopper chopper;
        public Chopper Chopper => m_instance.chopper;

        [SerializeField] private UIManager uiManager;
        public UIManager UIManager => m_instance.uiManager;

        [SerializeField] private CollectibleSpawner treeSpawner;
        public CollectibleSpawner TreeSpawner => m_instance.treeSpawner;

        [SerializeField] private AudioManager audioManager;
        public AudioManager AudioManager => m_instance.audioManager;

        private static GameManager m_instance;

        //score
        public int WoodCount;
        public int StoneCount;
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
                StoneCount = PlayerPrefs.GetInt("StoneCount");
                AmountTreeCut = PlayerPrefs.GetInt("AmountTreeCut");
                Player.ChoppingSpeed = PlayerPrefs.GetInt("ChoppingSpeedMultiplicator");
                Chopper.IsPurchased = (PlayerPrefs.GetInt("IsChopperPurchased") != 0);

                //
                if (Player.ChoppingSpeed <= 0) Player.ChoppingSpeed = 1;

                //update UI
                UIManager.SetTreeAmount(WoodCount);
                UIManager.SetStoneAmount(StoneCount);
                UIManager.ChoppingBoostLevel.text = Player.ChoppingSpeed.ToString();
            }
            else
            {
                Debug.Log("Create save");
                PlayerPrefs.SetInt("WoodCount", 0);
                PlayerPrefs.SetInt("StoneCount", 0);
                PlayerPrefs.SetInt("AmountTreeCut", 0);
                PlayerPrefs.SetFloat("ChoppingSpeedMultiplicator", 1);
                PlayerPrefs.SetInt("IsChopperPurchased", (Chopper.IsPurchased ? 1 : 0));
                PlayerPrefs.Save();

                WoodCount = 0;
                StoneCount = 0;
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

            UIManager.ChangeTreeAmount(5);

            //total amount of tree is a multiple of 10
            if (AmountTreeCut % 10 == 0)
                LevelWon();
        }

        public void NewRockInStorage()
        {
            StoneCount += 5;

            //save change
            PlayerPrefs.SetInt("StoneCount", StoneCount);
            PlayerPrefs.Save();

            UIManager.ChangeStoneAmount(5);
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

                UIManager.SetTreeAmount(WoodCount);
                return true;
            }
        }

        public bool UseStone(int stoneAmount)
        {
            if (StoneCount < stoneAmount) return false;
            else
            {
                StoneCount -= stoneAmount;

                //save change
                PlayerPrefs.SetInt("StoneCount", StoneCount);
                PlayerPrefs.Save();

                UIManager.SetStoneAmount(StoneCount);
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

        private void OnApplicationQuit()
        {
            //save players progration
            PlayerPrefs.SetInt("WoodCount", WoodCount);
            PlayerPrefs.SetInt("StoneCount", StoneCount);
            PlayerPrefs.SetInt("AmountTreeCut", AmountTreeCut);
            PlayerPrefs.SetFloat("ChoppingSpeedMultiplicator", Player.ChoppingSpeed);
            PlayerPrefs.Save();
        }

    }
}
