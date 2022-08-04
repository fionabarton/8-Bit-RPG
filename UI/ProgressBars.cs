using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides access to both the party and enemy's battle progress bars
/// </summary>
public class ProgressBars : MonoBehaviour
{
    [Header("Set in Inspector")]
    // Health Bars
    public List<ProgressBar> playerHealthBarsCS;
    public List<ProgressBar> enemyHealthBarsCS;

    // Magic Bars
    public List<ProgressBar> playerMagicBarsCS;

    [Header("Set Dynamically")]
    private static ProgressBars _S;
    public static ProgressBars S { get { return _S; } set { _S = value; } }

    void Awake() {
        S = this;
    }
}
