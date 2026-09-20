using System;
using UnityEngine;

public class timelineManager : MonoBehaviour
{

    public int currentTimeline;
    // Como ainda não tem a mecanica,
    // eu vou imaginar que seriam valores de 0 e 1. Poderia ser uma string também.

    public static event Action<int> onTimelineChanged;

    // Evento para que ele possa comunicar com quem precisar dessa informação

    public int onTimeChange(int mudancaTemp)
    {
        currentTimeline = mudancaTemp;
        onTimelineChanged?.Invoke(currentTimeline);

        return currentTimeline;
    }
}
