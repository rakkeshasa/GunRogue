# GunRogue
Unity를 사용한 로그라이크 게임

### 커스텀 에디터

![customeditor](https://github.com/user-attachments/assets/43314163-9adc-49cb-87c6-b88ad32d69f0)
</br>
게임의 맵을 다양한 구조로 만들기 위해 맵을 그래프로 나타내는 에디터를 확장했습니다.</br>
에디터에서는 방의 종류와 복도를 노드 형식으로 나타내고 노드를 선으로 연결해 맵 형태를 나타냅니다.</br></br>

```
public class RoomNodeGraphEditor : EditorWindow
{
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
    }
}
```
에디터를 확장하기 위해 EditorWindow를 상속받고 MenuItem을 통해 커스텀한 에디터가 Unity의 메뉴 목록에 뜨도록 했습니다.</br>
GetWindow를 통해 EditorWindow가 없으면 인스턴스를 생성하고 만약 존재한다면 해당 인스턴스를 얻어오도록 해 에디터 창이 1개만 존재하도록 했습니다.</br>
EditorWindow가 활성화되면 노드를 담당할 GUI를 OnEnable에서 세팅했습니다.</BR></BR>

```
[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

    private void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();
        foreach(RoomNodeSO node in roomNodeList)
        {
            roomNodeDictionary[node.id] = node;
        }
    }
}
```
방 노드와 노드들을 연결하는 것을 나타내는 그래프는 인스턴스와는 별도로 대량의 데이터를 저장해야하므로 ScriptableObject 클래스를 상속받게 했습니다.</br>
에디터 세션 동안 커스텀 에디터에서 그린 그래프를 저장해야하며 런타임 동안 맵의 구조를 가져와 구성해야하므로 ScriptableObject가 적합했습니다.</br>
그래프는 현재 그려진 노드들이 담긴 리스트를 순회하며 노드의 id와 노드를 사전형 타입에 저장해 노드간 연결 시 필요한 id를 미리 준비했습니다.</br></br>

```
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    public bool isCorridor;
    public bool isEntrance;
    public bool isBossRoom;
}

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
```
방의 타입을 나타내는 RoomNodeType은 방의 형태가 어떤지 bool타입으로 나타냈으며, 타입 별 이름을 갖도록 했습니다.</br>
만약 방의 타입의 이름을 지정하지 않았다면 OnValidate를 통해 디버그 로그를 출력해 검증할 수 있게 했습니다.</br>
각 타입별로 오브젝트를 생성해 리스트에 담아 게임 내에서 관리할 수 있도록 했습니다.</br></br>

노드 그래프의 에셋을 더블 클릭하면 RoomNodeGraphEditor는 해당 에셋을 Unity에 출력시켜야합니다.</br>
```
[OnOpenAsset(0)]
public static bool OnDoubleClickAssets(int instanceID, int line)
{
    RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

    if (roomNodeGraph != null)
    {
        OpenWindow();
        currentRoomNodeGraph = roomNodeGraph;
        return true;
    }
    return false;
}
```
OnOpenAsset은 Unity.Callback에서 지원하는 기능으로 더블 클릭시 이벤트를 감지하는 콜백입니다.</br>
더블 클릭한 에셋이 노드 그래프인지 에셋 id를 통해 캐스팅을 해 체크하고 맞다면 Unity의 기본 동작을 따르지 않고 커스텀 에디터를 출력하는 함수를 호출합니다.</br></br>

![leftclick](https://github.com/user-attachments/assets/3da4cb67-0a1e-4610-b9e8-c2f713bf2801)
</br>
커스텀한 에디터에서 방 노드를 추가하거나 노드 전체 선택과 같은 옵션을 추가하기 위해 OnGUI에서 에디터가 활성화된 동안 우클릭을 한 상황을 추적해 옵션 메뉴가 출력되도록 했습니다.</BR>

```
private void ShowContextMenu(Vector2 mousePosition)
{
    GenericMenu menu = new GenericMenu();
    menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
    menu.AddSeparator("");
    menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
    menu.AddSeparator("");
    menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
    menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);

    menu.ShowAsContext();
}
```
UnityEditor에서 제공하는 GenericMenu를 이용해 손쉽게 구현할 수 있었으며 옵션에 맞는 함수를 연결해줄 수 있었습니다.</br></br>

```
private void CreateRoomNode(Vector2 mousePosition, RoomNodeTypeSO roomNodeType)
{
    RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
    currentRoomNodeGraph.roomNodeList.Add(roomNode);

    roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)),
      currentRoomNodeGraph, roomNodeType);
    
    // RoomNodeGraphSO 에셋 데이터베이스에 roomNode 더하기
    AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
    AssetDatabase.SaveAssets();
}
```
방 노드를 생성하는 CreateRoomNode는 ScriptableObject타입인 RoomNode를 인스턴스화하고, 그래프가 관리할 수 있도록 노드를 리스트에 담았습니다.</br>
인스턴스화한 방 노드는 위치, 크기, 타입을 세팅해 초기 설정을 해줬으며, 그래프 에셋에 방 노드 객체를 더한 후 변경된 에셋을 저장해 에디터를 껐다 켜도 그래프가 유지되도록 했습니다.</br>
에디터를 재시작한 후에 그래프 에셋을 활성화하면 그래프의 방 노드가 담긴 리스트를 순회해 그래프에 노드를 그려주게 됩니다.</br></br>

![createnode](https://github.com/user-attachments/assets/4b1d83e0-f1ee-4266-a151-87f137cf85d4)
</br>

```
public void Draw(GUIStyle nodeStyle)
{
    GUILayout.BeginArea(rect, nodeStyle);
    EditorGUI.BeginChangeCheck();

    int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
    int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
    roomNodeType = roomNodeTypeList.list[selection];

    if (EditorGUI.EndChangeCheck())
        EditorUtility.SetDirty(this);

    GUILayout.EndArea();
}
```
노드는 정해진 크기와 스타일로 에디터에 그려지게 되고, 노드에서 방의 타입(입구, 작은방, 큰방 등)을 선택할 수 있습니다.</br>
이를 위해 노드에서 변경사항이 있는지 체크해 방 타입이 변경됐는지 확인했으며, 변경됐다면 더티 플래그를 설정해 저장되도록 했습니다.</br>
Unity는 에셋에서 변경이 일어나도 자동저장이 되지 않기 때문에 씬을 이동하거나 에디터를 재시작하면 에셋에 설정한 값들이 모두 초기화됩니다.</br>
이를 방지하기 위해 변경된 사항을 더티 플래그로 설정하고 에셋의 요소가 변경될 때마다 저장되는 것이 비효율적이므로 더티 플래그를 설정하고 나중에 저장할때 플래그가 설정된 것만 한번에 저장하는 기능입니다.</br></br>

![line2](https://github.com/user-attachments/assets/f6439cc8-0976-440f-ad83-7efcbd890a13)
</br>
생성된 방 노드에서 우클릭으로 드래그하면 다른 방 노드와 연결할 수 있는 선이 나옵니다.</br>
그래프에서는 선이 출발한 노드와 현재 마우스의 위치를 저장하고 에디터는 해당 정보를 갖고와 선을 그려줍니다.</br>

```
private void ProcessRightMouseDragEvent(Event currentEvent)
{
    if(currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
    {
        DragConnectingLine(currentEvent.delta);
        GUI.changed = true;
    }
}

private void DragConnectingLine(Vector2 delta)
{
    currentRoomNodeGraph.linePosition += delta;
}

private void DrawDraggedLine()
{
    if(currentRoomNodeGraph.linePosition != Vector2.zero)
    {
        Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
          currentRoomNodeGraph.linePosition, currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
          currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
    }
}
```

선을 드래그하여 노드가 아닌 곳에서 우클릭을 놓는다면 그래프의 마우스 위치와 시작 노드를 Vector2.Zero와 null로 초기화하지만</br>
노드 위에서 우클릭을 놓는다면 두 노드를 연결시키기 위해 노드에 있는 부모 노드 리스트와 자식 노드 리스트에 노드의 id를 넣었습니다.</br>
노드의 id는 그래프에서 사전형 타입으로 노드와 연결 시켜서 관리 중이므로 id를 통해 쉽게 노드에 접근할 수 있습니다.</br>

```
private void ProcessMouseUpEvent(Event currentEvent)
{
      RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);
      if (roomNode != null)
      {
          if(currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
          {
              roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
          }
      }
}

private void DrawRoomConnections()
{
      foreach(string childRoomNodeId in roomNode.childRoomNodeIDList)
      {
          if (currentRoomNodeGraph.roomNodeDictionary[childRoomNodeId])
          {
              DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeId]);
              GUI.changed = true;
          }
      }
}
```
노드에 있는 자식 노드 리스트를 순회하며 연결할 노드를 확인하고 선을 연결합니다.</br>
이때, 노드간 부모-자식 관계를 나타내기위해 방향이 있는 선을 구현했습니다.</br>

```
private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
{
    Vector2 midPosition = (endPosition + startPosition) / 2f;
    Vector2 direction = endPosition - startPosition;

    Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
    Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
    Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

    Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
    Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);
}
```

![math](https://github.com/user-attachments/assets/6fb5b304-6a5e-4cec-89e5-95d72779676e)
</br>
연결된 두 노드에서 선을 끌어온 노드가 부모 노드가 되고 새롭게 선을 놓은 노드를 자식 노드로 두어 벡터를 구했습니다.</br>
화살표를 나타내기 위해 벡터로 부터 수직인 직교 벡터를 구하기 위해 회전 행렬을 이용해 화살표의 좌표를 구해준 뒤 선을 그려 화살표를 구현했습니다.</br></br>

방 노드를 잘못 두어 삭제해야할 경우 해당 노드와 연결된 노드들을 리스트에서 탐색해 삭제하고 안전하게 노드가 삭제될 수 있도록 했습니다.</br>

```
private void DeleteSelectedRoomNodes()
{
    foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
    {
        if(roomNode.isSelected)
        {
            roomNodeDeletionQueue.Enqueue(roomNode);
            foreach(string childRoomNodeID in roomNode.childRoomNodeIDList)
            {
                RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);
                childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
            }
            foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
            {
                RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);
                parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
            }
        }
    }

    while(roomNodeDeletionQueue.Count > 0)
    {
        RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();
        currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);
        currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);

        DestroyImmediate(roomNodeToDelete, true);
    }
}
```
그래프에 있는 모든 방 노드들을 순회하면서 노드가 선택된 상태라면 큐에 담아 리스트에서 삭제할 요소를 따로 두었습니다.</br>
삭제할 노드가 가지고 있는 자식 노드 리스트를 전부 삭제하고 부모 노드의 경우 부모 노드의 자식 노드 리스트에서 자기 자신을 제거해 연결 관계를 끊었습니다.</br>
큐에 담긴 요소를 순회하면서 그래프에 있는 리스트와 딕셔너리에서 자기자신을 제거하고 노드를 그래프상에서 파괴합니다.</br></br>

### 방 노드를 활용한 방 생성

DungeonLevelSO : 그래프와 방을 실제로 연결해주는 클래스</br>

```
public class DungeonLevelSO : ScriptableObject
{
    public string levelName;
    public List<RoomTemplateSO> roomTemplateList;
    public List<RoomNodeGraphSO> roomNodeGraphList;
}
```

커스텀 에디터를 통해 레벨별로 5종의 그래프를 만들었습니다.</br>
게임 매니저를 싱글톤으로 구현해서 매니저가 게임 내 던전의 레벨을 처리하고, 던전의 레벨 별로 구현할 그래프와 그래프를 구현할 방의 템플릿들의 정보를 가지고 있도록 했습니다.</br>
예를 들어 게임을 새롭게 시작하면 레벨 1 던전부터 플레이하게 되고, 레벨 1에 맞는 그래프 5개 중 1개를 선택해 던전의 큰 틀을 설계하고, 그래프에 있는 방 노드들을 다양한 방 형태들을 사용해 구현하게 됩니다.</br></br>

```
private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
{
    RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));
    openRoomNodeQueue.Enqueue(entranceNode);

    bool noRoomOverlaps = true;
    noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

    if(openRoomNodeQueue.Count == 0 && noRoomOverlaps) return true;
    else                                               return false;
}
```
그래프 5개 중에 1개를 선택해 구현할 방 노드들을 큐에 1개씩 담습니다.</br>
모든 그래프의 최상위 노드인 입구부터 큐에 담으며 그 다음 노드부터는 구현할 방 형태가 다른 방과 겹치는지 체크하고 큐에 담도록 했습니다.</br></br>


```
private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
{
    while(openRoomNodeQueue.Count > 0 && noRoomOverlaps)
    {
        RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();
        
        foreach(RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
        {
            openRoomNodeQueue.Enqueue(childRoomNode);
        }

        if(roomNode.roomNodeType.isEntrance)
        {
            RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode); // 방 정보 세팅
            room.isPositioned = true;
            dungeonBuilderRoomDictionary.Add(room.id, room);
        }
        else
        {
            Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];
            noRoomOverlaps = CanPlaceRoomW    ithNoOverlaps(roomNode, parentRoom);
        }
    }

    return noRoomOverlaps;
}
```
큐에서 첫번째 요소인 입구 노드를 추출하고 트리 그래프처럼 입구 노드부터 자식 노드들을 전부 큐에 담아 그래프에 있는 모든 노드들을 큐에 넣었습니다.</br>
입구의 경우에는 부모 노드가 없으며 제일 우선적으로 배치되는 방이므로 다른 방과 겹치는지 확인하지 않고 바로 Room 클래스를 구현합니다.</br>
입구가 아닌 경우에는 부모 노드인 방과 통로를 연결해주기 위해 부모 노드를 추출하고 부모 노드와 자신이 위치가 겹치는지 체크합니다.</br></br>


```
private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
{
    while (roomOverlaps)
    {
        List<Doorway> unconnectedParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorWayList).ToList();

        Doorway doorwayParent = unconnectedParentDoorways[UnityEngine.Random.Range(0, unconnectedParentDoorways.Count)];
        RoomTemplateSO roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

        Room room = CreateRoomFromRoomTemplate(roomtemplate, roomNode); // 방 정보 세팅
        if(PlaceTheRoom(parentRoom, doorwayParent, room))
        {
            roomOverlaps = false;
            dungeonBuilderRoomDictionary.Add(room.id, room);
        }
        else
            roomOverlaps = true;
    }

    return true;
}
```
부모 노드 방과 자신을 연결하기 위해 부모 노드 방에서 아직 연결되지 않은 문을 리스트에 담습니다.</br>
리스트에서 통로 1개를 추출해 해당 통로의 방향에 맞는 내 방의 형태를 갖고와 Room 클래스를 세팅합니다.</br>
예를 들어 부모 노드가 가로축 통로라면 동쪽과 서쪽인 2개의 문이 존재하며 이미 통로는 입구나 다른 방에 의해 한 쪽 문이 연결된 상태입니다.</br>
이 때 남은 통로 1개가 동쪽 문이라면 자식 노드는 서쪽 문을 갖고 있어야 연결이 가능하며 서쪽 문을 가지고 자신의 방 타입과 맞는 방 형태를 갖고와 Room 클래스를 구현하게 됩니다.</br></br>

구현한 Room 클래스의 정보를 토대로 World좌표에 방을 좌표에 그리고 지금까지 구현한 방이 담긴 Dictionary의 모든 방과 겹치는지 체크를 합니다.</br>

```
private bool IsOverlappingRoom(Room room1, Room room2)
{
    bool isOverlappingX = IsOverlappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);
    bool isOverlappingY = IsOverlappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

    if(isOverlappingX && isOverlappingY)
    {
        return true;
    }

    return false;
}

private bool IsOverlappingInterval(int imin1, int imax1, int imin2, int imax2)
{
    if(Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
    {
        return true;
    }
    else
    {
        return false;
    }
}
```
방이 겹치는지 체크하는 방법은 사각형과 사격형끼리 겹치는지 확인하는 방식으로 두 방의 min, max 좌표를 비교했습니다.</br>
모든 방과 겹치지 않는다면 해당 방은 구현할 수 있는 방이므로 추후에 인스턴스화하기 위해 Dictionary에 넣습니다.</br></br>

```
private void InstantiateRoomGameObjects()
{
    foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
    {
        Room room = keyvaluepair.Value;

        Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

        GameObject roomGameObject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);
        InstantiatedRoom instantiatedRoom = roomGameObject.GetComponentInChildren<InstantiatedRoom>();
        instantiatedRoom.room = room;

        instantiatedRoom.Initialise(roomGameObject);
        room.instantiatedRoom = instantiatedRoom;
    }
}
```
그래프에 있는 모든 방 노드가 Dictionary에 담겼다면 방을 이제 인스턴스화하여 World에 배치합니다.</br>

### Cinemachine
역동적인 카메라 움직임을 주기 위해 Cinemachine을 사용했습니다.</br>
Cinemachine은 Unity의 카메라 시스템을 강화하는 강력한 카메라 제어 패키지로 자동 추적, 충돌 감지, 뷰 조정 등을 쉽게 구현할 수 있도록 돕는 시스템입니다.</br>
Cinemachine이 Unity의 카메라를 직접 조작하지 않고, Virtual Camera(VCam) 개념을 도입해 카메라의 움직임을 설정합니다.</br>
Cinemachine은 씬에 여러 개의 VCam을 설정하고 우선순위를 두어 어떤 카메라가 활성화될지 제어할 수 있으며 Unity Camera는 VCam을 따라 움직이는 역할을 합니다.</br>
</br>

![cinemachine target group](https://github.com/user-attachments/assets/31b4e544-9f43-4fce-a47b-f20a470dae46)
</br>
이 프로젝트에서는 카메라가 플레이어를 중심에 두고 움직이는 것이 아니라 플레이어와 마우스의 중점을 중심에 두고 움직이게 하기 위해 Cinemachine을 사용했습니다.</br>
그렇게 하기 위해서는 VCam이 어떤 것을 중심으로 찍어야 할지 Cinemachine Target Group에 객체를 넘겨줘야 합니다.</br>
Target Group에 플레이어와 마우스를 담기 위해 리스트를 새로 생성하고 마우스와 플레이어를 담아 VCam이 찍어야 하는 객체를 넘겨줬습니다.</br>
두 객체의 가중치를 동일하게 1로 두어 VCam이 마우스와 플레이어의 중점을 중심으로 두도록 했습니다.</br>

```
private void SetCinemachineTargetGroup()
{
    CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target {
        weight = 1f, radius = 2.5f, target = GameManager.Instance.GetPlayer().transform };
    CinemachineTargetGroup.Target cinemachineGroupTarget_cursor = new CinemachineTargetGroup.Target {
        weight = 1f, radius = 1f, target = cursorTarget };

    CinemachineTargetGroup.Target[] cinemachingTargetArray = new CinemachineTargetGroup.Target[] {
        cinemachineGroupTarget_player, cinemachineGroupTarget_cursor };

    cinemachingTargetGroup.m_Targets = cinemachingTargetArray;
}
```

### 발행-구독 디자인 패턴
플레이어의 입력에 따른 이동 처리를 Action을 이용해 처리하나 옵저버 패턴이 아닌 발행-구독 디자인 패턴을 사용했습니다.</br>
옵저버 패턴은 한 객체의 상태 변화가 여러 객체에 자동으로 반영되도록 하는 패턴으로 상태 변화를 관찰하는 옵저버(구독자)가 발행자를 직접 구독해야 동작할 수 있습니다.</br>
하지만 발행-구독 패턴은 구독자와 발행자가 분리되어 있어 서로 참조하지 않고, 둘 사이에 이벤트 관리자를 사용해 통신이 가능하도록 해 옵저버 패턴보다는 더 낮은 결합도를 가지고 있습니다.</br>

```
public class MovementByVelocityEvent : MonoBehaviour
{
    public event Action<MovementByVelocityEvent, MovementByVelocityArgs> OnMovementByVelocity;

    public void CallMovementByVelocityEvent(Vector2 moveDirection, float moveSpeed)
    {
        OnMovementByVelocity?.Invoke(this, new MovementByVelocityArgs() { moveDirection = moveDirection, moveSpeed = moveSpeed });
    }
}

public class MovementByVelocityArgs : EventArgs
{
    public Vector2 moveDirection;
    public float moveSpeed;
}
```
구독자와 발행자 사이를 이어주는 이벤트 관리자 클래스로 델리게이트를 선언하고 넘겨줄 인자를 따로 클래스를 생성해 관리했습니다.</br>
델리게이트에 추가적인 정보를 보내야할경우 구독자나 발행자 측 코드를 수정 할 필요없이 EventArgs만 수정하면 되기때문에 낮은 결합도를 체감할 수 있었습니다.</br>

```
private void OnEnable()
{
    movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
}

private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
{
    MoveRigidBody(movementByVelocityArgs.moveDirection, movementByVelocityArgs.moveSpeed);
}

private void MoveRigidBody(Vector2 moveDirection, float moveSpeed)
{
    rigidBody2D.velocity = moveDirection * moveSpeed;
}
```
구독자는 이벤트 관리자의 델리게이트를 구독하고 이벤트가 발생하면 바인딩한 함수를 호출해 플레이어의 RigidBody의 속도를 세팅해 움직이도록 했습니다.</br>
또한 이벤트는 가비지 컬렉터(GC)에 의해 자동 정리되지 않으므로 OnDisable에서 이벤트 구독을 해제해 메모리 누수가 방지했습니다.</br>
구독자는 발행자인 PlayerControl 클래스를 전혀 알고 있지 않으며 발행자인 PlayerControl 클래스 또한 구독자 클래스를 알지 않고 서로 이벤트 관리자를 통해서만 통신을 할 수 있습니다.</br>
이동 담당 클래스뿐만 아니라 애니메이션을 관리하는 클래스도 PlayerControl 클래스를 감시하지 않고 이벤트 관리자를 통해 델리게이트를 구독해 발행-구독 패턴을 구현했습니다.</br>

### 플레이어 회피
구르는 동작을 더 자연스럽게 만들고, 구르는 중에 다른 동작을 방지하기 위해 코루틴을 활용했습니다.</br>
특히 구르는 동작은 RidigdBody의 MovePosition을 사용해서 이동하므로 정확한 물리연산을 위해 WaitForFixedUpdate를 사용해 물리 엔진이 적용된 후 다음 이동을 수행하도록 했습니다.</br>

```
private void PlayerRoll(Vector3 direction)
{
    playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
}

private IEnumerator PlayerRollRoutine(Vector3 direction)
{
    float minDistance = 0.2f;
    Vector3 targetPosition = player.transform.position + (Vector3)direction * movementDetails.rollDistance;

    while(Vector3.Distance(player.transform.position, targetPosition) > minDistance)
    {
        player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, movementDetails.rollSpeed, direction, isPlayerRolling);

        yield return waitForFixedUpdate;
    }

    isPlayerRolling = false;
    playerRollCooldownTimer = movementDetails.rollCooldownTime;
    player.transform.position = targetPosition;
}
```
플레이어가 구르는 키를 입력하면 구르는 코루틴을 실행하며 해당 코루틴을 변수에 저장해 구르는 중에 벽에 부딪힐 경우 코루틴을 중지할 수 있게 했습니다.</br>
코루틴에서는 while문을 돌면서 구르는 거리가 일정치 이하가 될 때까지 순회하도록 했으며, FixedUpdate마다 이벤트 관리자의 이벤트를 발생시켜 캐릭터의 RigidBody의 위치를 강제로 이동시켜 점진적으로 나아가는 것처럼 보이게했습니다.</br>
while문이 종료되면 구르기 쿨타임을 적용하고 플레이어의 위치를 정확하게 하기 위해 보정했습니다.</br></br>

