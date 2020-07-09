﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationPlayer : MidBattleScreen
{
    public enum CurrentState { Writing, Waiting }
    public new static ConversationPlayer Current;
    public float LettersPerSecond;
    public Text Name;
    public Text Text;
    public GameObject Arrow;
    private CurrentState state;
    private ConversationData origin;
    private int currentLine;
    private int currentChar;
    private float count;
    private void Awake()
    {
        Current = this;
    }
    private void Update()
    {
        if (origin != null)
        {
            switch (state)
            {
                case CurrentState.Writing:
                    if (Control.GetButtonDown(Control.CB.A))
                    {
                        Text.text = origin.Lines[currentLine];
                        Arrow.SetActive(true);
                        state = CurrentState.Waiting;
                    }
                    else
                    {
                        count += Time.deltaTime * LettersPerSecond;
                        if (count >= 1)
                        {
                            if (++currentChar < origin.Lines[currentLine].Length)
                            {
                                count -= 1;
                                Text.text += origin.Lines[currentLine][currentChar];
                            }
                            else
                            {
                                Arrow.SetActive(true);
                                state = CurrentState.Waiting;
                            }
                        }
                    }
                    break;
                case CurrentState.Waiting:
                    if (Control.GetButtonDown(Control.CB.A))
                    {
                        if (++currentLine >= origin.Lines.Count)
                        {
                            origin = null;
                            MidBattleScreen.Current = null;
                            gameObject.SetActive(false);
                            return;
                        }
                        else
                        {
                            StartLine(currentLine);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
    public void Play(ConversationData conversation)
    {
        gameObject.SetActive(true);
        MidBattleScreen.Current = this;
        origin = conversation;
        StartLine(0);
    }
    private void StartLine(int num)
    {
        currentLine = num;
        if (origin.Lines[currentLine].IndexOf(':') != -1)
        {
            string[] parts = origin.Lines[currentLine].Split(':');
            Name.text = parts[0]; // Also change image
            currentChar = origin.Lines[currentLine].IndexOf(':') + 2;
        }
        else
        {
            currentChar = 0;
        }
        count = 0;
        state = CurrentState.Writing;
        Text.text = origin.Lines[currentLine][currentChar].ToString();
        Arrow.SetActive(false);
    }
}
