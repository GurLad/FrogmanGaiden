﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConversationController : MonoBehaviour
{
    public static ConversationController Current;
    public List<TextAsset> Conversations;
    [Header("Editor loader")]
    public string Path = "Conversations";
    private List<ConversationData> options;
    private void Awake()
    {
        Current = this;
        options = new List<ConversationData>();
        foreach (TextAsset conversation in Conversations)
        {
            options.Add(new ConversationData(conversation));
        }
        options = options.FindAll(a => !a.Done);
    }
    public ConversationData SelectConversation()
    {
        List<ConversationData> currentOptions = options.FindAll(a => a.MeetsRequirements());
        currentOptions.Sort();
        Debug.Log("Options: " + string.Join(", ", currentOptions));
        ConversationData chosen = currentOptions[0];
        return chosen;
    }
    public ConversationData SelectConversationByID(string id)
    {
        List<ConversationData> currentOptions = options.FindAll(a => a.ID == id);
        if (currentOptions.Count <= 0)
        {
            return null;
        }
        currentOptions.Sort();
        Debug.Log("Options: " + string.Join(", ", currentOptions));
        ConversationData chosen = currentOptions[0];
        return chosen;
    }
    #if UNITY_EDITOR 
    public void AutoLoad()
    {
        Conversations.Clear();
        string[] fileNames = UnityEditor.AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets/Data/" + Path });
        foreach (string fileName in fileNames)
        {
            TextAsset file = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(UnityEditor.AssetDatabase.GUIDToAssetPath(fileName));
            Conversations.Add(file);
        }
        UnityEditor.EditorUtility.SetDirty(gameObject);
    }
    #endif
}

/*
 * Concept:
 *  Each room (out of the 9 per run, plus maybe the BFOT ones) starts with a conversation, out of a pool of available ones for that specific room.
 *  Some events are global (can appear at any room), local (only appear in the Xth room of a run) or specific (only appear in a specific map).
 *  Those use the requirements of "roomNumber:X" and "map:ID".
 *  Read text files with the conversation information.
 *  Starts with priority and unique flag, then requirements (ex. hasCharacter, foughtCharacter...), then the event itself.
 * Priority & Unique:
 *  Unique events only appear once per save.
 *  If multiple events are possible, choose a random one from among the highest priority available ones.
 *  High priority should be tutorials/story-based events. Unique.
 *  Medium priority should be interactions between specific character/reactions to rare events. Mostly unique.
 *  Low priority should be filler conversation, in case no higher priority events are available/all of them appeared already. Not unique.
 *  Format (temporary?):
 *  "
 *      priority:int (higher number means higher priority)
 *      unique:T/F
 *      ~
 *  "
 * Requirements:
 *  The requirements for the event to appear.
 *  Mainly having participating characters, but could also be about whether the play defeated X, has a certain weapon, etc.
 *  Format (temporary?):
 *  "
 *      requirement:info
 *      requirement:info
 *      ...
 *      requirement:info
 *      ~
 *  "
 * Demands (new!):
 *  Some events only make sense in a specific type of maps. The demands is a list of requirments for the sort of maps this event can appear in.
 *  Most demands will be about the number of characters alive (ex. the third level of each area may allow you to recruit another character if you have less than X).
 *  Format (temporary?):
 *  "
 *      demand:info
 *      demand:info
 *      ...
 *      demand:info
 *      ~
 *  "
 * Event:
 *  Read line-by-line.
 *  When a character name appears ("name: text"), load its image until another name appears.
 *  Show a line until the player presses A, then show the next one (change image if necessary) and so on.
 *  Format (temporary?):
 *  "
 *      Name: Text.
 *      More text.
 *      Name 2: Even more text.
 *      ...
 *      Some text.
 *  "
 * Example:
 * "
 *  priority:3
 *  unique:T
 *  ~
 *  hasCharacter:Frogman
 *  ~
 *  charactersAlive:=0
 *  ~
 *  Frogman: Hello world!
 *  This is text!
 *  Enemy: Die!!!
 * "
 */
