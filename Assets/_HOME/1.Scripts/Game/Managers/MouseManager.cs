using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace HOME.Game {

    public class MouseManager : MonoBehaviour {

        public static event EventHandler OnGathererClicked;

        private static MouseManager _instance;
        public static MouseManager Instance { get { return _instance; } }
        public List<Interactive> selectedUnits = new List<Interactive>(); // selected units
        private GameManager _gameManager;
        public GameObject _rightClickIndicPrefab;

        [Header("MouseDrag Fields")]
        public float boxWidth;
        public float boxHeight;
        public float boxTop;
        public float boxLeft;
        [Space]
        public Vector2 boxStart;
        public Vector2 boxFinish;
        public Vector3 mouseDragStartPosition;
        public Vector3 currentMousePoint;
        public Vector3 mouseDownPoint;
        [Space]
        public GUIStyle mouseDragSkin;
        [Space]
        public bool mouseDragging;
        public float minDragDist = 10f;
        [Space]
        public Interactive clickedUnit;

        public enum State {
            clickOrDrag,
            clickSelect,
            clickDeselect
        }

        private State _state;

        public void Init(GameManager gameManager) {
            _gameManager = gameManager;
        }

        private void Awake() {
            _instance = this;
            StartCoroutine(DetectDoubleClick());
        }

        private void Update() {
            switch (_state) {
                case State.clickOrDrag:

                ClickOrDrag();
                break;

                case State.clickSelect:
                SelectSingleUnit();
                break;

                case State.clickDeselect:
                DeselectAll();
                break;
            }
        }

        private void OnGUI() {
            if (mouseDragging)
                GUI.Box(new Rect(boxLeft, boxTop, boxWidth, boxHeight), "", mouseDragSkin);
        }

        public void DeselectAll() {
            if (selectedUnits.Count > 0) {
                for (int i = 0; i < selectedUnits.Count; i++) {
                    selectedUnits[i].Deselect(); //deselect unit
                }
                selectedUnits.Clear();//clear list
                ResourceManager.Instance.selectedGathererAIList.Clear(); // clear gatherer list
            }
            if (selectedUnits.Count == 0) { // if 0 again enter new state
                _state = State.clickOrDrag;
            }
        }

        private void ClickOrDrag() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // cast ray from mouse pos
            RaycastHit hit; // store hit
            EventSystem es = EventSystem.current; // reference to EV for click inputs

            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && !IsPointerOverUI()) { // See if ray hits anything aslong as not over ui
                currentMousePoint = hit.point; //set current mouse point to ray hit point

                if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift)) {                  //Single Click 
                    Interactive isInteractive = hit.collider.gameObject.GetComponent<Interactive>(); // store interactive component here if available
                    mouseDownPoint = hit.point;
                    mouseDragStartPosition = Input.mousePosition;
                    if (isInteractive != null) { // if there was an Interactive hit and its tagged "Unit" and is not already in the selection list
                        clickedUnit = isInteractive;
                        _state = State.clickSelect;
                    }
                    if (hit.collider.gameObject.tag == "Terrain") {
                        _state = State.clickDeselect;
                    }
                }
                if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift)) { //holding shift, click to select units or click selected units to deselect
                    Interactive isInteractive = hit.collider.gameObject.GetComponent<Interactive>(); // store interactive componen here if available
                                                                                                     //Debug.Log("isInteractive" + isInteractive);
                    if (!selectedUnits.Contains(isInteractive)) {
                        selectedUnits.Add(isInteractive);
                        isInteractive.Select();
                    } else if (selectedUnits.Contains(isInteractive)) {
                        selectedUnits.Remove(isInteractive);
                        isInteractive.Deselect();
                    }
                }
                if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftShift)) {
                    if (UserDraggingByPosition(mouseDragStartPosition, Input.mousePosition)) {
                        mouseDragging = true;
                        DrawDragBox();
                        SelectUnitsInDrag();
                    }
                }
                if (Input.GetMouseButtonUp(0) && !Input.GetKey(KeyCode.LeftShift)) {
                    mouseDragging = false;
                }
            }
        }

        private void SelectUnitsInDrag() {
            if (_gameManager == null) {
                Debug.Log("MOUSEMANAGER Select Units in Drag No gamemanager found!");
            }
            foreach (var faction in _gameManager.Factions) {
                if (!faction.isAi) { // if isAi false== player units
                    foreach (var activeUnit in faction.ActiveUnits) {
                        Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(activeUnit.transform.position);
                        if (unitScreenPosition.x < boxFinish.x && unitScreenPosition.y > boxFinish.y && unitScreenPosition.x > boxStart.x && unitScreenPosition.y < boxStart.y && activeUnit.tag == "Unit") {
                            if (activeUnit.GetComponent<Interactive>() != null) { // make sure unit is interactive
                                AddToSelectedUnits(activeUnit.GetComponent<Interactive>());
                            }
                        }
                    }
                } else {
                    return;
                }
            }
        }

        private void AddToSelectedUnits(Interactive unitToAdd) {
            if (!selectedUnits.Contains(unitToAdd)) {
                selectedUnits.Add(unitToAdd);
                unitToAdd.Select(); //select clickedUnit
                isClickedUnitGathererCheck(unitToAdd); // check if gatherer
                return;
            }
        }

        private void SelectSingleUnit() {
            if (selectedUnits.Count > 0) { // if there are already units selected
                for (int i = 0; i < selectedUnits.Count; i++) { // look through selected units
                    if (selectedUnits[i] != null) { // check if unit got destroeyd
                        selectedUnits[i].Deselect(); //deselect unit
                    }
                }
                selectedUnits.Clear(); //clear list list
                                       //Debug.Log("Deselect Unit");
            }

            if (selectedUnits.Count == 0) { // if no selected units currently
                clickedUnit.Select(); //select clickedUnit
                isClickedUnitGathererCheck(clickedUnit); // check if gatherer
                selectedUnits.Add(clickedUnit);
                _state = State.clickOrDrag; //enter new state
                                            //Debug.Log("Select Unit");
            }
        }

        IEnumerator DetectDoubleClick() {
            while (true) {
                float duration = 0;
                bool doubleClicked = false;
                if (Input.GetMouseButtonDown(0)) {
                    while (duration < .2f) {
                        duration += Time.deltaTime;
                        yield return new WaitForSeconds(0.005f);
                        if (Input.GetMouseButtonDown(0)) {
                            doubleClicked = true;
                            duration = .2f;
                            // Double click/tap
                            //print(selectedUnits.Last().name);
                        }
                    }
                    if (!doubleClicked) {
                        // Single click/tap
                    }
                }
                yield return null;
            }
        }

        private void isClickedUnitGathererCheck(Interactive clickedUnit) {// add unit to list of selected gatherers
            if (clickedUnit.GetComponent<GatherAI>() != null) {
                OnGathererClicked?.Invoke(clickedUnit.GetComponent<GatherAI>(), EventArgs.Empty);
            }
            return;
        }

        private void DrawDragBox() {
            boxWidth = Camera.main.WorldToScreenPoint(mouseDownPoint).x - Camera.main.WorldToScreenPoint(currentMousePoint).x;
            boxHeight = Camera.main.WorldToScreenPoint(mouseDownPoint).y - Camera.main.WorldToScreenPoint(currentMousePoint).y;
            boxLeft = Input.mousePosition.x;
            boxTop = (Screen.height - Input.mousePosition.y) - boxHeight; //need to invert y as GUI space has 0,0 at top left, but Screen space has 0,0 at bottom left. x is the same. 

            if (boxWidth > 0 && boxHeight < 0f) {
                boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            } else if (boxWidth > 0 && boxHeight > 0f) {
                boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y + boxHeight);
            } else if (boxWidth < 0 && boxHeight < 0f) {
                boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y);
            } else if (boxWidth < 0 && boxHeight > 0f) {
                boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y + boxHeight);
            }
            boxFinish = new Vector2(boxStart.x + Mathf.Abs(boxWidth), boxStart.y - Mathf.Abs(boxHeight));
        }

        private bool UserDraggingByPosition(Vector2 dragStartPoint, Vector2 newPoint) {
            if ((newPoint.x > dragStartPoint.x || newPoint.x < dragStartPoint.x) || (newPoint.y > dragStartPoint.y || newPoint.y < dragStartPoint.y)) {
                return true;
            } else {
                return false;
            }
        }

        public void MarkTarget(Vector3 target, Color color) {
            GameObject clicker = Instantiate(_rightClickIndicPrefab, target, Quaternion.identity);
            var rend = clicker.GetComponentsInChildren<SpriteRenderer>(); // change colour
            foreach (var sprite in rend) {
                sprite.color = color;
            }
        }

        public static bool IsPointerOverUI() {
            if (EventSystem.current.IsPointerOverGameObject()) {
                //Debug.Log("Over UI");
                return true;
            } else {
                //Debug.Log("Not Over UI");
                PointerEventData pe = new PointerEventData(EventSystem.current);
                pe.position = Input.mousePosition;
                List<RaycastResult> hits = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pe, hits);
                return hits.Count > 0;
            }
        }
    }
}