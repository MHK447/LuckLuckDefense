using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BanpoFri;
using UnityEngine.EventSystems;

public class TileSelector : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private Camera mainCamera;
    private bool isUnitSelected;
    private GameObject selectedUnit;

    private UnitTileComponent FirstSelectComponent;

    private UnitTileComponent SecondSelectComponent;

    private InGameUnitSelect InGameUnitSelectUI;

    private InGameUnitBase SelectAttackArangeUnit;

    void Start()
    {
        mainCamera = Camera.main;

        // LineRenderer 설정
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
        lineRenderer.sortingLayerName = "Foreground";
        lineRenderer.sortingOrder = 5;

        if (InGameUnitSelectUI == null)
        {
            GameRoot.Instance.UISystem.LoadFloatingUI<InGameUnitSelect>((unitselect) => {
                ProjectUtility.SetActiveCheck(unitselect.gameObject, false);
                InGameUnitSelectUI = unitselect;
            });
        }
    }

    void Update()
    {
        HandleInput();
    }
    void HandleInput()
    {
        if (IsPointerOverUIObject())
        {
            return;
        }

        // 입력 상태 감지
        bool isTouch = Input.touchCount > 0;
        bool isMouseDown = Input.GetMouseButtonDown(0);
        bool isMouseUp = Input.GetMouseButtonUp(0);
        bool isMouse = Input.GetMouseButton(0);

        Vector2 inputPosition2D = Vector2.zero;

        if (isTouch)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10f));
            inputPosition2D = new Vector2(touchPosition.x, touchPosition.y);

            if (touch.phase == TouchPhase.Began)
            {
                HandleBeginInput(inputPosition2D);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                HandleMoveInput(inputPosition2D);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                HandleEndInput(inputPosition2D);
            }
        }
        else
        {
            if (isMouseDown)
            {
                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                inputPosition2D = new Vector2(mousePosition.x, mousePosition.y);
                HandleBeginInput(inputPosition2D);
            }
            else if (isMouse)
            {
                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                inputPosition2D = new Vector2(mousePosition.x, mousePosition.y);
                HandleMoveInput(inputPosition2D);
            }
            else if (isMouseUp)
            {
                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                inputPosition2D = new Vector2(mousePosition.x, mousePosition.y);
                HandleEndInput(inputPosition2D);
            }
        }
    }

    void HandleBeginInput(Vector2 position)
    {
        if (SelectAttackArangeUnit != null)
            SelectAttackArangeUnit.TileAttackRangeActive(false);

        var getunitinfo = GameRoot.Instance.UISystem.GetUI<PopupInGameUnitInfo>();
        if (getunitinfo != null && getunitinfo.gameObject.activeSelf)
            getunitinfo.Hide();

        if (InGameUnitSelectUI != null)
            ProjectUtility.SetActiveCheck(InGameUnitSelectUI.gameObject, false);

        Collider2D hitCollider = Physics2D.OverlapPoint(position);
        if (hitCollider != null && hitCollider.CompareTag("Tile"))
        {
            FirstSelectComponent = hitCollider.GetComponent<UnitTileComponent>();
            if (FirstSelectComponent.UnitList.Count > 0)
            {
                selectedUnit = hitCollider.gameObject;
                isUnitSelected = true;
                FirstSelectComponent.EnableTile();
            }
            else
            {
                isUnitSelected = false;
            }
        }
    }

    void HandleMoveInput(Vector2 position)
    {
        if (isUnitSelected)
        {
            Collider2D hitCollider = Physics2D.OverlapPoint(position);
            if (hitCollider != null && hitCollider.CompareTag("Tile"))
            {
                UnitTileComponent newTileComponent = hitCollider.GetComponent<UnitTileComponent>();

                // 이미 선택된 타일과 같으면 무시
                if (SecondSelectComponent == newTileComponent)
                    return;

                // 이전 타일 비활성화
                if (SecondSelectComponent != null && SecondSelectComponent != FirstSelectComponent)
                    SecondSelectComponent.DisableTile();

                // 새로운 타일 활성화
                SecondSelectComponent = newTileComponent;
                DrawPath(selectedUnit.transform.position, hitCollider.transform.position);
                SecondSelectComponent.EnableTile();
            }
        }
    }

    void HandleEndInput(Vector2 position)
    {
        Collider2D hitCollider = Physics2D.OverlapPoint(position);

        if (FirstSelectComponent != null && SecondSelectComponent == null)
        {
            // 같은 타일을 선택한 경우
            var finddata = FirstSelectComponent.UnitList.FirstOrDefault();
            if (finddata != null)
            {
                finddata.TileAttackRangeActive(true);
                GameRoot.Instance.UISystem.OpenUI<PopupInGameUnitInfo>(popup => popup.Set(finddata.GetUnitIdx));
                ProjectUtility.SetActiveCheck(InGameUnitSelectUI.gameObject, true);
                InGameUnitSelectUI.Init(FirstSelectComponent.transform);
                InGameUnitSelectUI.Set(finddata.GetUnitIdx, FirstSelectComponent);
            }
        }
        else if (FirstSelectComponent != null && SecondSelectComponent != null)
        {
            if (FirstSelectComponent.GetTileSpawnOrder == SecondSelectComponent.GetTileSpawnOrder)
            {
                var finddata = FirstSelectComponent.UnitList.FirstOrDefault();
                if (finddata != null)
                {
                    SelectAttackArangeUnit = finddata;
                    finddata.TileAttackRangeActive(true);
                    GameRoot.Instance.UISystem.OpenUI<PopupInGameUnitInfo>(popup => popup.Set(finddata.GetUnitIdx));
                    ProjectUtility.SetActiveCheck(InGameUnitSelectUI.gameObject, true);
                    InGameUnitSelectUI.Init(FirstSelectComponent.transform);
                    InGameUnitSelectUI.Set(finddata.GetUnitIdx, FirstSelectComponent);
                }
            }
            else
            {
                // 다른 타일로 이동
                FirstSelectComponent.MoveChangeTileUnit(SecondSelectComponent);
                SecondSelectComponent.MoveChangeTileUnit(FirstSelectComponent);

                var tempUnitList = FirstSelectComponent.UnitList.ToList();
                FirstSelectComponent.UnitList = SecondSelectComponent.UnitList;
                SecondSelectComponent.UnitList = tempUnitList;

                if (FirstSelectComponent.UnitList.Count > 0)
                    FirstSelectComponent.SetTileUnitIdx(FirstSelectComponent.UnitList.First().GetUnitIdx);

                if (SecondSelectComponent.UnitList.Count > 0)
                {
                    SelectAttackArangeUnit = SecondSelectComponent.UnitList[0];
                    SecondSelectComponent.UnitList[0].TileAttackRangeActive(true);
                    SecondSelectComponent.SetTileUnitIdx(SecondSelectComponent.UnitList[0].GetUnitIdx);
                }
            }
        }

        ResetSelection();
        lineRenderer.positionCount = 0;
    }

    void ResetSelection()
    {
        if (FirstSelectComponent != null)
        {
            FirstSelectComponent.DisableTile();
            FirstSelectComponent = null;
        }
        if (SecondSelectComponent != null)
        {
            SecondSelectComponent.DisableTile();
            SecondSelectComponent = null;
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        // 마우스 포인터 위치
        if (Input.touchCount > 0)
        {
            eventDataCurrentPosition.position = Input.GetTouch(0).position;
        }
        else
        {
            eventDataCurrentPosition.position = Input.mousePosition;
        }

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }



    void DrawPath(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    System.Collections.IEnumerator MoveUnit(Vector3 targetPosition)
    {
        float duration = 1.0f; 
        Vector3 startPosition = selectedUnit.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            selectedUnit.transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        selectedUnit.transform.position = targetPosition;
        isUnitSelected = false;
        lineRenderer.positionCount = 0;
    }
}