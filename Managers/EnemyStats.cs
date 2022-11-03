﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Stats")]
public class EnemyStats : ScriptableObject {
	public new string name;
	public Sprite sprite;
	public int HP; // set to maxHP in BattleInitiative.cs
	public int MP; // set to maxMP in BattleInitiative.cs
	public int maxHP;
	public int maxMP;
	public int STR;
	public int DEF;
	public int WIS;
	public int AGI;
	public int EXP;
	public int Gold;
	public int LVL;

	// Drop Item
	public List<eItem> itemsToDrop = new List<eItem>();
	public float chanceToDrop;
	public int maxAmountToSteal; // amount of items that can be stolen from this enemy
	public int amountToSteal; // set to maxAmountToSteal in BattleInitiative.cs

	// Items stolen from party
	public List<Item> stolenItems = new List<Item>();

	// AI
	public eEnemyAI AI;
	public int AI_id = -1; // Default setting simply calls random move

	public int questNdx = -1;

	// Default Action
	public float chanceToCallAction;
	public int defaultAction;

	// Actions/Moves Enemy can perform
	public List<int> actionList;

	public bool isDead; // set to false in BattleInitiative.cs
	public bool isCallingForHelp;
	public int	nextTurnActionNdx;

	// Set dynamically when entering battle
	public int battleID;
}