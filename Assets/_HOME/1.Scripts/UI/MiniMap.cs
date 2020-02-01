using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using HOME.Game;

namespace HOME.UI {

    public class MiniMap : MonoBehaviour, IPointerClickHandler {

        private static MiniMap _instance;
        public static MiniMap Instance { get => _instance; }
        private CameraManager _cameraMgr;
        public RectTransform playerCameraPosition;
        public RectTransform playerclickPosition;
        [SerializeField] private RawImage _mapRaw = default;
        public static Vector3 terrainSize;
        public static Vector3 miniMapHitTargetWorldSpace;
        private Vector3 _minimapHit;

        private void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
            } else {
                _instance = this;
            }
        }

        public void Init(CameraManager camMgr) {
            _cameraMgr = camMgr;
            _mapRaw = GetComponentInChildren<RawImage>();
            StartCoroutine(MinimapCameraIndicatorHandler());
        }
        IEnumerator MinimapCameraIndicatorHandler() {
            while (true) {
                playerCameraPosition.position = WorldPositionToMap(Camera.main.transform.position);
                yield return new WaitForSeconds(0.1f);
            }
        }

        IEnumerator MinimapClickIndicatorHandler(Vector3 hitP) {
            RectTransform clickIndicatorMinimap = Instantiate(playerclickPosition, hitP, Quaternion.identity, playerclickPosition.transform.parent);
            yield return new WaitForSeconds(1f);
            Destroy(clickIndicatorMinimap.gameObject);
        }

        public Vector2 WorldPositionToMap(Vector3 point) { // returns world positon to minimap position
            if (terrainSize != Vector3.zero) {
                float coordX = point.x / terrainSize.x * _mapRaw.texture.width;
                float coordZ = point.z / terrainSize.z * _mapRaw.texture.height;
                Vector2 mapPos = new Vector2(coordX, coordZ);
                return mapPos;
            } else {
                Debug.Log("MiniMapFailure");
                Debug.Break();
                return Vector2.zero;
            }
        }

        public void OnPointerClick(PointerEventData eventData) { // getting eventsystem data from minimap click position
            Vector2 miniMapClickPosition = new Vector2(0, 0); 
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_mapRaw.rectTransform, eventData.pressPosition, eventData.pressEventCamera, out miniMapClickPosition)) {

                Texture rawTexture = _mapRaw.texture;
                Rect rawRectTransform = _mapRaw.rectTransform.rect;

                //Using the size of the texture and the miniMapClickPosition, clamp the X,Y coords between 0 and width - height of texture
                float coordX = Mathf.Clamp(0, (((miniMapClickPosition.x - rawRectTransform.x) * rawTexture.width) / rawRectTransform.width), rawTexture.width);
                float coordY = Mathf.Clamp(0, (((miniMapClickPosition.y - rawRectTransform.y) * rawTexture.height) / rawRectTransform.height), rawTexture.height);
                Debug.Log("miniMapClickPosition.x " + miniMapClickPosition.x + "   " + miniMapClickPosition.y);
                //Convert coordX and coordY to % (0.0-1.0) with respect to texture width and height
                float recalcX = coordX / rawTexture.width;
                float recalcY = coordY / rawTexture.height;

                miniMapClickPosition = new Vector2(recalcX, recalcY);
                CastMinimapToWorld(miniMapClickPosition, eventData);
            }
        }

        private void CastMinimapToWorld(Vector2 miniMapClickPosition, PointerEventData eventData) { // cast to world position
            Ray miniMapRay = _cameraMgr.miniMapCamera.ScreenPointToRay(new Vector2(miniMapClickPosition.x * _cameraMgr.miniMapCamera.pixelWidth, miniMapClickPosition.y * _cameraMgr.miniMapCamera.pixelHeight));

            RaycastHit miniMapHit; // store hit here
            if (Physics.Raycast(miniMapRay, out miniMapHit, Mathf.Infinity)) { // see what we hit
                Debug.Log("miniMapHit.point: " + miniMapHit.point);  // do stuff with hit pos here!

                if (eventData.button == PointerEventData.InputButton.Left) { // left click moves camera
                    Debug.Log("Left click");
                    Vector3 tempP = miniMapHit.point;
                    tempP.y -= 1f;
                    GameObject camClicker = Instantiate(MouseManager.Instance._rightClickIndicPrefab, tempP, Quaternion.identity);
                    Destroy(camClicker, 6);
                    _cameraMgr.SetFollowTarget(camClicker.transform);
                } else if (eventData.button == PointerEventData.InputButton.Right) { // right click moves units if selected
                    Unit._hasMinimapTarget = true;
                    Unit._minimapTarget = miniMapHit.point;
                    StartCoroutine(MinimapClickIndicatorHandler(WorldPositionToMap(miniMapHit.point)));
                }
            }
        }
    }
}



