using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores and returns commonly used words and phrases
/// </summary>
public class WordManager : MonoBehaviour {
    [Header("Set Dynamically")]
    public List<string> exclamations = new List<string>();
    public List<string> interjections = new List<string>();

    private static WordManager _S;
    public static WordManager S { get { return _S; } set { _S = value; } }

    void Awake() {
        S = this;
    }

    void Start() {
        exclamations = new List<string>() { "Oh yeah", "Heck yeah", "Hoorah", "Whoopee", "Yahoo", "Wahoo", "Hot diggity dog",
            "Huzzah", "Yippee", "Woo hoo", "Whoop dee doo", "Hooray",  "Gee whiz", "Right on", "Far out",
            "Groovy", "Awesome", "Excellent", "Cool", "Incredible", "Unreal", "Fabulous", "Terrific", "Yay",
            "Fantastic", "Great", "Gnarly", "Sweet", "Nice", "Splendid", "Wicked", "Wow", "Dude", "Cool beans",
            "Booyah", "Cowabunga", "Tubular" };

        interjections = new List<string>() { "Rats", "Yuck", "Dang", "Darn", "Blast", "Oh bother", "Doggone it", "Darnation",
            "Gosh darn it", "Gosh darn it to heck", "Oh fiddlesticks", "Cripes", "Confound it", "Shucks", "Shoot",
            "Blooming heck", "Flipping heck", "Blinking heck", "Dash it", "Strike me pink", "My goodness", "For Pete's sake",
            "For Heaven's sake", "Frick", "Good gosh", "Bloody heck", "Dagnabbit", "Oh poo","Blimey", "Great Scott",
            "Goodness me", "For crying out loud", "Good gracious", "Good golly", "Dang it", "Darn it" };
    }

    public string GetRandomExclamation() {
        // Get random index
        int randomNdx = Random.Range(0, exclamations.Count);

        // Return random exclamation
        return exclamations[randomNdx];
    }

    public string GetRandomInterjection() {
        // Get random index
        int randomNdx = Random.Range(0, interjections.Count);

        // Return random interjection
        return interjections[randomNdx];
    }
}