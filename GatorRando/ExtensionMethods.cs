using UnityEngine.Events;

namespace GatorRando;

public static class ExtensionMethods
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Member Access",
        "Publicizer001:Accessing a member that was not originally public",
        Justification = "There's no public way to remove persistent listeners"
    )]
    public static void ObliteratePersistentListenerByIndex(this UnityEvent uevent, int eventIndex)
    {
        var persistentCalls = uevent.m_PersistentCalls;
        persistentCalls.UnregisterPersistentListener(eventIndex);
        persistentCalls.RemoveListener(eventIndex);
    }
}

