using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace CreativeSpore.RpgMapEditor
{

    [RequireComponent(typeof(BoxCollider))]
    [AddComponentMenu("RpgMapEditor/Behaviours/TeleporterBehaviour", 10)]
    public class TeleporterBehaviour : MonoBehaviour
    {
        [Tooltip("Set the name of the target scene")]
        public string TargetSceneName;
        [Tooltip("Set the name of the target teleporter")]
        public string TargetTeleporterName;
        [Tooltip("If true, the target teleported will be linked with this teleporter when the teleportation is done")]
        public bool LinkWithTarget = true;

        public KeyCode ActivationKey = KeyCode.Return;
        public bool TeleportOnEnter = false;
        public void SetTeleporterName(string name)
        {
            m_teleporterName = name;
        }

        BoxCollider m_boxCollider;

        private bool m_isActivationKeyPressed = false;
        private string m_savedSceneName;
        private string m_teleporterName;

        void Start()
        {
            Reset();
            if (string.IsNullOrEmpty(m_teleporterName))
            {
                m_teleporterName = this.name;
            }
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
            m_savedSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
#else
            m_savedSceneName = Application.loadedLevelName;
#endif
        }

        void Reset()
        {
            m_boxCollider = GetComponent<BoxCollider>();
            m_boxCollider.isTrigger = true;
        }

		/* deprecated and reimplemented by Max - See OnEnable()
        void OnLevelWasLoaded()
        {
            PlayerController player = GetComponent<PlayerController>();
            if (player != null)
            {
                TeleportTo(player.gameObject, TargetTeleporterName);
                player.PhyCtrl.TeleportTo(player.transform.position);
                Destroy(this);
            }
        }
        */

        void Update()
        {            
            m_isActivationKeyPressed = m_isActivationKeyPressed && Input.GetKey(ActivationKey) || Input.GetKeyDown(ActivationKey) && Time.timeSinceLevelLoad > 0.5f;
        }

		void OnEnable()
		{
			//Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
			SceneManager.sceneLoaded += OnLevelFinishedLoading;
		}

		void OnDisable()
		{
			//Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
			SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		}

		void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
		{
			PlayerController player = GetComponent<PlayerController>();
			if (player != null)
			{
				TeleportTo(player.gameObject, TargetTeleporterName);
				player.PhyCtrl.TeleportTo(player.transform.position);
				Destroy(this);
			}
		}

        void TeleportTo(GameObject srcObj, string dstObjName)
        {
            GameObject targetTeleport = GameObject.Find(dstObjName);
            if (targetTeleport == null)
            {
                Debug.LogWarning(" Teleport destination not found: " + dstObjName);
            }
            else
            {
                if(LinkWithTarget)
                {
                    TeleporterBehaviour teleporterBhv = targetTeleport.GetComponent<TeleporterBehaviour>();
                    if(teleporterBhv)
                    {
                        teleporterBhv.TargetSceneName = m_savedSceneName;
                        teleporterBhv.TargetTeleporterName = m_teleporterName;
                    }
                }
                Vector3 targetPos = targetTeleport.transform.position;
                targetPos.z = transform.position.z;
                srcObj.transform.position = targetPos;
            }
        }


        private static int s_nextAllowedOnEnterFrame;
        void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
			if (player != null && TeleportOnEnter && Time.frameCount > s_nextAllowedOnEnterFrame && MainEngine.self.playState == MainEngine.playSME.Roaming)
            {
				s_nextAllowedOnEnterFrame = Time.frameCount + 10;
				_prepTransition(player.gameObject);
            }
        }

        void OnTriggerStay(Collider other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
			if (player != null && m_isActivationKeyPressed && MainEngine.self.playState == MainEngine.playSME.Roaming)
            {
				_prepTransition(player.gameObject);
            }
        }

		void _prepTransition(GameObject playerGO)
		{
			MainEngine.self._setState(MainEngine.playSME.Transitioning);
			GameObject transitionGO = Instantiate(MainEngine.self.transitionPrefab);
			transitionGO.GetComponent<Transition>().initFadeToBlack(this, playerGO);
			OverEnemyBehaviour enemyScript = GetComponentInParent<OverEnemyBehaviour>();
			if(enemyScript != null)
			{
				MainEngine.self.primaryEncounter = enemyScript.firstSPECIES;
			}
		}

        public void DoTeleport(GameObject obj)
        {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
            if (string.IsNullOrEmpty(TargetSceneName) || TargetSceneName == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
#else
            if (string.IsNullOrEmpty(TargetSceneName) || TargetSceneName == Application.loadedLevelName)
#endif
            {
                TeleportTo(obj, TargetTeleporterName);
            }
            else
            {
                TeleporterBehaviour teleportComp = obj.AddComponent<TeleporterBehaviour>();
                teleportComp.TargetTeleporterName = TargetTeleporterName;
                teleportComp.SetTeleporterName(this.name);
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
                    UnityEngine.SceneManagement.SceneManager.LoadScene(TargetSceneName);
#else
                Application.LoadLevel(TargetSceneName);
#endif
            }
        }
    }
}