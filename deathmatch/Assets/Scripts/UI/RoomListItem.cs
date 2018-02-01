using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
public class RoomListItem : MonoBehaviour
{

    public delegate void JoinRoomDelegate(MatchInfoSnapshot _match);
    private JoinRoomDelegate joinRoomCallBack;

    [SerializeField]
    private Text RoomName;

    private MatchInfoSnapshot match;

    public void Setup(MatchInfoSnapshot _match, JoinRoomDelegate _joinRoomCallBack)
    {
        match = _match;
        RoomName.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
        joinRoomCallBack = _joinRoomCallBack;
    }
    public void JoinRoom()
    {
        joinRoomCallBack.Invoke(match);
    }
}
