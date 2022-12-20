using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public abstract class AGameType : NetworkBehaviour
{
    public abstract void StartGame();
    public abstract bool IsOver();
}