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


