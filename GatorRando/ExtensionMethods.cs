using UnityEngine.Events;

namespace GatorRando;

public static class ExtensionMethods
{
    public static void ObliteratePersistentListenerByIndex(this UnityEvent uevent, int eventIndex)
    {
        var persistentCalls = uevent.m_PersistentCalls;
        persistentCalls.UnregisterPersistentListener(eventIndex);
        persistentCalls.RemoveListener(eventIndex);
    }
}

