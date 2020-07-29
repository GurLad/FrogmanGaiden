﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationController : MonoBehaviour
{
    public static ConversationController Current;
    public List<TextAsset> Conversations;
    private List<ConversationData> options;
    private void Awake()
    {
        Current = this;
        options = new List<ConversationData>();
        foreach (TextAsset conversation in Conversations)
        {
            options.Add(new ConversationData(conversation.text));
        }
    }
    public void PlayRandomConversation()
    {
        options = options.FindAll(a => a.MeetsRequirements());
        options.Sort();
        ConversationPlayer.Current.Play(options[0]);
    }
}

public class ConversationData : IComparable<ConversationData>
{
    public int Priority { get; private set; }
    public bool Unique { get; private set; } // Technically, it's probably a better idea to give them ID's and just destry/ignore ones if unique and ID matches saved data.
    public List<string> Requirements { get; } = new List<string>(); // Can add a class for that as well, but seems a bit of an overkill.
    public List<string> Lines { get; private set; } // See above.
    public ConversationData(string source)
    {
        // I know that using JSON is techincally better, but I want to be able to create events using a simple text editor, so splits are simple.
        string[] parts = source.Split('~');
        string[] lines = parts[0].Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] lineParts = lines[i].Split(':');
            switch (lineParts[0])
            {
                case "priority":
                    Priority = int.Parse(lineParts[1]);
                    break;
                case "unique":
                    Unique = lineParts[1] == "T";
                    break;
                default:
                    break;
            }
        }
        // Current approach, Requirments is a list of strings
        Requirements = new List<string>(parts[1].Split('\n'));
        Requirements.RemoveAt(0);
        Lines = new List<string>(parts[2].Split('\n'));
        Lines.RemoveAt(0);
        // Alternate approach with custom classes. Currently theoretical.
        //lines = parts[1].Split('\n');
        //for (int i = 0; i < lines.Length; i++)
        //{
        //    string[] lineParts = lines[i].Split(':');
        //    Requirements.Add(new RequirementClass(lineParts[0], lineParts[1]));
        //}
    }
    public bool MeetsRequirements()
    {
        foreach (var requirement in Requirements)
        {
            string[] parts = requirement.Split(':');
            switch (parts[0])
            {
                case "hasCharacter":
                    // Check, return false if false.
                    break;
                case "roomNumber":
                    // Will also have a X-Y format, for specific areas/specific part of the game (1-3,2-7 etc.)
                    if (int.Parse(parts[1]) != GameController.Current.LevelNumber)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }
        }
        return true;
    }

    public int CompareTo(ConversationData other)
    {
        if (Priority.CompareTo(other.Priority) != 0)
        {
            return Priority.CompareTo(other.Priority);
        }
        else
        {
            return UnityEngine.Random.Range(-1, 2);
        }
    }
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
 *  Frogman: Hello world!
 *  This is text!
 *  Enemy: Die!!!
 * "
 */