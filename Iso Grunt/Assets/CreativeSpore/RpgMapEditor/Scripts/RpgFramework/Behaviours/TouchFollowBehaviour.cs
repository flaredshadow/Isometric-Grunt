using UnityEngine;
using System.Collections;
using CreativeSpore.RpgMapEditor;

namespace CreativeSpore.RpgMapEditor
{

    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(MovingBehaviour))]
    [RequireComponent(typeof(MapPathFindingBehaviour))]
    [RequireComponent(typeof(DirectionalAnimation))]
    [AddComponentMenu("RpgMapEditor/Behaviours/TouchFollowBehaviour", 10)]
    public class TouchFollowBehaviour : MonoBehaviour
    {
        public float MinDistToReachTarget = 0.16f;

        MovingBehaviour m_moving;
        MapPathFindingBehaviour m_pathFindingBehaviour;
        DirectionalAnimation m_animCtrl;


        void Start()
        {
            m_moving = GetComponent<MovingBehaviour>();
            m_pathFindingBehaviour = GetComponent<MapPathFindingBehaviour>();
            m_animCtrl = GetComponent<DirectionalAnimation>();
            m_pathFindingBehaviour.TargetPos = transform.position;
        }

        void UpdateAnimDir()
        {
            m_animCtrl.IsPlaying = m_moving.Veloc.magnitude > Vector3.kEpsilon;
            m_animCtrl.SetAnimDirection(m_moving.Veloc);
        }

        void Update()
        {
            // Get pressed world position
            bool mouseDown = Input.GetMouseButtonDown(0);
            bool touchUp = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;
            if (mouseDown || touchUp)
            {
                Ray ray = Camera.main.ScreenPointToRay(mouseDown ? Input.mousePosition : new Vector3( Input.GetTouch(0).position.x, Input.GetTouch(0).position.y) );
                Plane hPlane = new Plane(Vector3.forward, Vector3.zero);
                float distance = 0;
                if (hPlane.Raycast(ray, out distance))
                {
                    // get the hit point:
                    Vector2 vHitPoint = ray.GetPoint(distance);
                    if (AutoTileMap.Instance.GetAutotileCollisionAtPosition(vHitPoint) == eTileCollisionType.PASSABLE)
                    {
                        m_pathFindingBehaviour.TargetPos = ray.GetPoint(distance);
                    }
                }
            }
            Vector3 vTarget = m_pathFindingBehaviour.TargetPos;
            //vTarget = RpgMapHelper.GetTileCenterPosition(vTarget);
            vTarget.z = transform.position.z;

            // stop when target position has been reached
            Vector3 vDist = (vTarget - transform.position);
            Debug.DrawLine(vTarget, transform.position); //TODO: the target is the touch position, not the target tile center. Fix this to go to target position once in the target tile
            m_pathFindingBehaviour.enabled = vDist.magnitude > MinDistToReachTarget;
            if (!m_pathFindingBehaviour.enabled || m_pathFindingBehaviour.Path.Count == 0)
            {
                m_moving.Veloc = Vector3.zero;
            }
        }

        void FixedUpdate()
        {
            UpdateAnimDir();
        }
    }
}