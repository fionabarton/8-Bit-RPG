using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
- Sub Menu
- Set Music/SFX
- Set Animation

- Dialogue that isn’t deactivated on button press

- Activate Quest 
void ActivateQuest (int questNdx) {
    B.S.questManagerCS.activated [questNdx] = true;
}
- Complete Quest 
void CompleteQuest (int questNdx) {
    B.S.questManagerCS.completed [questNdx] = true;
}
*/

public class CutsceneManager : MonoBehaviour
{
    [Header("Set Dynamically")]
    // Singleton
    private static CutsceneManager _S;
    public static CutsceneManager S { get { return _S; } set { _S = value; } }

    // Index of the current step of the cutscene
    public int              stepNdx;
    // Controls when to move to next step
    public bool             stepDone;
    // Set to TRUE in:
    // - RPGDialogueManager.ThisLoop (calls CamFollow.ChangeTarget)
    // - CamFollow.ChangeTarget
    // - MoveCharacter.NextMove

    // Set in CutsceneTriggerOnCollision.cs & CutsceneTriggerOnButtonPress.cs 
    public List<GameObject> actors;
    int                     sceneNdx = 99;

    // Remembers whether a cutscene has already happened to prevent it from running more than once
    public List<bool>       sceneDone = new List<bool> { false, false, false };

    // Resuable variables
    List<string>            message = new List<string>();
    DialogueTrigger         dt;
    List<EnemyStats>        enemyStats;

    private void Awake() {
        S = this;
    }

    public void StartScene(int _ndx, List<GameObject> _actors) {
        if (!sceneDone[_ndx]) { 
            actors.Clear();

            sceneNdx = _ndx;
            actors = _actors;

            stepDone = true;

            // Add FixedLoop() to UpdateManager
            UpdateManager.fixedUpdateDelegate += FixedLoop;
            // Add Loop() to UpdateManager
            UpdateManager.updateDelegate += Loop;
        }
    }

    private void FixedLoop() {
        if (stepDone) {
            switch (sceneNdx) {
                case 0: // Bullet Head
                    stepDone = false;

                    switch (stepNdx) {
                        case 0: // Change Cam Target
                            CamManager.S.ChangeTarget(actors[0], true);
                            break;
                        case 1: // Display Dialogue 
                            message.Clear();
                            message.Add("<color=yellow><Mac></color> Whoa.");
                            message.Add("Is this working?");
                            message.Add("Well, IT BUTTER BE!!!");
                            DialogueManager.S.DisplayText(message);
                            break;
                        case 2: // Display Dialogue 
                            message.Clear();
                            message.Add("<color=yellow><Blob></color> ...?");
                            DialogueManager.S.DisplayText(message);
                            break;
                        case 3: // Change Cam Target
                            CamManager.S.ChangeTarget(actors[0], true);
                            break;
                        case 4: // Move Character
                            FreezePlayer();

                            List<int> directions = new List<int>() {0,1,2,3};
                            List<int> distances = new List<int>() {2,2,2,2};
                            MoveCharacter(actors[0], directions, distances, 5);
                            break;
                        case 5: // Display Dialogue 
                            message.Clear();
                            message.Add("<color=yellow><Mac></color> I knew it was working!");
                            DialogueManager.S.DisplayText(message);
                            break;
                        case 6: // Enable DialogueTrigger & reset CutsceneManager
                            dt = actors[0].GetComponent<DialogueTrigger>();
                            dt.enabled = true;

                            EndScene();
                            return;
                    }

                    stepNdx += 1;
                    break;
                case 1: // Spike Mini Bosses
                    stepDone = false;

                    switch (stepNdx) {
                        case 0: // Move Character and Move Cam
                            FreezePlayer();

                            List<int> directions1 = new List<int>() { 2, 1 };
                            List<int> distances1 = new List<int>() { 2, 2 };
                            MoveCharacter(actors[0], directions1, distances1);

                            List<int> directions2 = new List<int>() { 0, 1 };
                            List<int> distances2 = new List<int>() { 2, 2 };
                            MoveCharacter(actors[1], directions2, distances2);

                            List<int> directions3 = new List<int>() { 1 };
                            List<int> distances3 = new List<int>() { 2 };
                            MoveCamera(directions3, distances3, 1.25f);
                            break;
                        case 1: // Switch Cam Mode & Change Cam Target  
                            CamManager.S.camMode = eCamMode.followAll;
                            CamManager.S.ChangeTarget(actors[0], true);
                            break;
                        case 2: // Display Dialogue  
                            message.Clear();
                            message.Add("<color=yellow><Bim></color> Here, have a magic potion!");
                            DialogueManager.S.DisplayText(message);
                            break;
                        case 3: // Change Cam Target
                            CamManager.S.ChangeTarget(actors[1], true);
                            break;
                        case 4: // Display Dialogue  
                            message.Clear();
                            message.Add("<color=yellow><Bam></color> You're welcome...");
                            message.Add("...NERD!");
                            DialogueManager.S.DisplayText(message);
                            break;
                        case 5: // Change Cam Target
                            //CamManager.S.ChangeTarget(Player.S.gameObject, true);
                            break;
                        case 6: // Add Item to Inventory & Display Dialogue  
                            Inventory.S.AddItemToInventory(Items.S.GetItem(eItem.mpPotion));
                           
                            message.Clear();
                            message.Add("You were just gifted a magic potion...");
                            message.Add("...COOL!");

                            DialogueManager.S.linesWithMiddleAlignment.Add(1);
                            DialogueManager.S.DisplayText(message, false);
                            break;
                        case 7: // Enable DialogueTrigger & reset CutsceneManager
                            dt = actors[0].GetComponent<DialogueTrigger>();
                            dt.enabled = true;
                            dt = actors[1].GetComponent<DialogueTrigger>();
                            dt.enabled = true;

                            EndScene();
                            return;
                    }

                    stepNdx += 1;
                    break;
                case 2: // Toiletron
                    stepDone = false;

                    switch (stepNdx) {
                        case 0:
                            // Move Character
                            FreezePlayer();

                            // Change Cam Target  
                            CamManager.S.ChangeTarget(actors[0], true);
                            break;
                        case 1: // Display Dialogue  
                            message.Clear();
                            message.Add("Hey, buddy.");
                            message.Add("Yeah, I’m the boss of this here cave, Toiletron.");
                            message.Add("<color=yellow><Toiletron></color> And I’m thinking…");
                            message.Add("...of stinking...");
                            message.Add("...LET US FIGHT!");
                            DialogueManager.S.DisplayText(message);
                            break;
                        case 2: // Time Buffer  
                            FreezePlayer();
                            TimeBuffer(2);
                            break;
                        case 3: // Display Dialogue  
                            message.Clear();
                            message.Add("<color=yellow><Toiletron></color> ...");
                            message.Add("...sorry for the delay...");
                            message.Add("...LET US FIGHT!");
                            DialogueManager.S.DisplayText(message);
                            break;
                        case 4: // Reset CutsceneManager & Start BAttle
                            Enemy enemy = actors[0].GetComponent<Enemy>();
                            enemyStats = enemy.stats;
                            StartBattle();

                            EndScene();
                            return;
                    }

                    stepNdx += 1;
                    break;
            }
        }   
    }

    // Handle multiple lines of dialogue
    private void Loop() {
        if (Input.GetButtonDown("SNES A Button")) {
            if (!GameManager.S.paused) {
                // For Multiple Lines
                if (DialogueManager.S.dialogueFinished && DialogueManager.S.ndx > 0) {
                    // Reset DialogueManager's text and cursor
                    DialogueManager.S.ClearForNextLine();

                    List<string> tMessage = new List<string>();

                    tMessage = message;

                    tMessage.RemoveAt(0);

                    // Call DisplayText() with one less line of "messages"
                    DialogueManager.S.DisplayText(tMessage);
                }
            }
        }
    }

    private void EndScene() {
        sceneDone[sceneNdx] = true;

        sceneNdx = 99;
        stepNdx = 0;
        actors.Clear();

        // Remove Delgate
        UpdateManager.fixedUpdateDelegate -= FixedLoop;
        // Remove Loop() to UpdateManager
        UpdateManager.updateDelegate -= Loop;
        return;
    }

    // Move Character
    void MoveCharacter(GameObject actor, List<int> walkDirections, List<int> distances, int speed = 2) {
        MoveCharacter m = actor.GetComponent<MoveCharacter>();

        m.speed = speed;

        for(int i = 0; i < walkDirections.Count; i++) {
            m.directions.Add(walkDirections[i]);
            m.distances.Add(distances[i]);
        }

        m.StartMovement();
    }

    // Move Camera
    void MoveCamera(List<int> directions, List<int> distances, float speed = 2) {
        MoveCam m = CamManager.S.gameObject.GetComponent<MoveCam>();

        m.speed = speed;

        for (int i = 0; i < directions.Count; i++) {
            m.directions.Add(directions[i]);
            m.distances.Add(distances[i]);
        }
        
        m.StartMovement();
    }

    // Freeze Player
    public void FreezePlayer() {
        //Player.S.canMove = false;
        //Player.S.mode = eRPGMode.idle;
    }

    // Start Battle 
    public void StartBattle() {
        if (enemyStats != null) {
            //GameManager.S.StartBattle(enemyStats);
        } else {
            Debug.LogWarning("EnemyStats not assigned in Inspector!");
        }
    }

    // Time Buffer 
    public void TimeBuffer(float amountToWait) {
        Invoke("GoToNextStep", amountToWait);
    }
    public void GoToNextStep() {
        stepDone = true;
    }

    // If this cutscene has already happened,
    // set up GameObjects as they were at the end of the cutscene
    public void SceneHasAlreadyHappened(int ndx, List<GameObject> actors) {
        switch (ndx) {
            case 0: // Bullet Head
                Utilities.S.SetPosition(actors[0], 24, 13.25f);
                Utilities.S.SetScale(actors[0], -1, 1);

                dt = actors[0].GetComponent<DialogueTrigger>();
                dt.enabled = true;
                break;
            case 1: // Spike Mini Bosses
                Utilities.S.SetPosition(actors[0], 3.5f, 8);
                Utilities.S.SetScale(actors[0], -1, 1);

                Utilities.S.SetPosition(actors[1], 8.5f, 8);

                dt = actors[0].GetComponent<DialogueTrigger>();
                dt.enabled = true;
                dt = actors[1].GetComponent<DialogueTrigger>();
                dt.enabled = true;
                break;
            case 2: // Toiletron
                // If party dies, reset cutscene to be triggered again
                Enemy enemy = actors[0].GetComponent<Enemy>();
                if (!enemy.stats[0].isDead) {
                    sceneDone[ndx] = false;
                }
                break;
        }
    }
}