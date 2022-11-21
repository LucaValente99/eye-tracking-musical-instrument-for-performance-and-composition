﻿using MyInstrument.DMIbox;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MyInstrument.Surface
{
    public class MyInstrumentSurface
    {
        #region Class attributes
        // Used to keep track of lastKeyboardPlayed. It is updated when keyboard is played
        // and it needs to check for playability of a key (see CheckPlayability() on DMIbox)
        private string lastKeyboardPlayed = "";
        public string LastKeyboardPlayed { get => lastKeyboardPlayed; set => lastKeyboardPlayed = value; } 

        // Used to keep track of lastKeyboardSelected. It is updated for every note selection (SelectNote on MyInstrumentButton).
        // It helps to manage the movement of keyboards on screen
        private string lastKeyboardSelected = "";
        public string LastKeyboardSelected { get => lastKeyboardSelected; set => lastKeyboardSelected = value; }

        // Used to keep track of lastKeyboardMoved.
        // It helps to manage the movement of keyboards on screen
        private string lastKeyboardMoved = "";

        private List<StackPanel> musicKeyboards = new List<StackPanel>();

        public List<StackPanel> MusicKeyboards
        {
            get { return musicKeyboards; }
            set { musicKeyboards = value; }
        }

        private Canvas canvas;

        // These vars helps to manage methods to change distances between keyboards and keys
        private double verticalDistance = 0;
        private double horizontalDistance;

        #endregion
        public MyInstrumentSurface(Canvas canvas)
        {
            this.canvas = canvas;
        }

        // Creating a keyboard list
        private List<StackPanel> CreateMusicKeyboards()
        {
            List<StackPanel> musicKeyboards = new List<StackPanel>();

            // Generating 16 keyboards helps to manage the the user's fastest play avoiding to see the movement of keyboards
            // at screen. So the movement seems smooth and infinite.
            for (int i = 0; i < 16; i++)
            {
                MyInstrumentKeyboard instrumentKeyboard = new MyInstrumentKeyboard();
                musicKeyboards.Add(instrumentKeyboard.MusicKeyboard);
            }

            return musicKeyboards;

        }

        // Drawing keyboards on the screen with the scale associated
        public void DrawOnCanvas()
        {
            // Each time this method is called the canvas is cleaned at first, then the new keyboards will be added
            if (musicKeyboards.Count != 0)
            {
                ClearSurface();
                musicKeyboards = CreateMusicKeyboards();
                SetVerticalDistance(Rack.UserSettings.KeyVerticaDistance);
            }
            else
            {
                musicKeyboards = CreateMusicKeyboards();
                SetVerticalDistance(Rack.UserSettings.KeyVerticaDistance);
            }

            horizontalDistance = 0;

            for (int i = 0; i < musicKeyboards.Count; i++)
            {
                canvas.Children.Add(musicKeyboards[i]);
                
                // Drawing the keyboard on screen
                Canvas.SetLeft(musicKeyboards[i], 75 + horizontalDistance); 
                Canvas.SetTop(musicKeyboards[i], (canvas.Height - musicKeyboards[i].Height) / 2);
                horizontalDistance += Rack.UserSettings.KeyHorizontalDistance;
            }

            MyInstrumentKeyboard.UpdateOpacity();
        }

        // Cleaning the canvas and resetting variables
        public void ClearSurface()
        {
            foreach (StackPanel instrumentKeyboard in musicKeyboards)
            {
                canvas.Children.Remove(instrumentKeyboard);
            }

            verticalDistance = Rack.UserSettings.KeyVerticaDistance;
            Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width = Rack.UserSettings.CanvasWidth;
            lastKeyboardMoved = "";
            lastKeyboardPlayed = "";
            lastKeyboardSelected = "";
            afterEighthKeyboard = false;
            firstTime = false;
            Rack.DMIBox.CheckedNote = null;

            // This needs to make checkPlayability works on DMIbox (keyboardID == (lastKeyboardPlayed + 1) % 16)
            MyInstrumentKeyboard.ID = 0;

            musicKeyboards.Clear();
        }
        
        // Setting vertical distance between keys
        public void SetVerticalDistance(double distance)
        {
            double addVerticalDistance;

            // Depending on number of keys within keyboards, the height to add or remove from these last will change
            if (Rack.UserSettings.SharpNotesMode == _SharpNotesModes.On)
            {
                addVerticalDistance = distance * 11;
            }
            else
            {
                addVerticalDistance = distance * 6;
            }

            foreach (StackPanel instrumentKeyboard in musicKeyboards)
            {
                #region Changing Keyboards Height
                // If the last vertical distance set is greater than the new one, my keyboards should decreas in height. 
                // So instrumentKeyboard.Height is added up to a negative number.
                // After updating the height of keyboard, it is placed at center of canvas.

                if (verticalDistance > distance)
                {
                    instrumentKeyboard.Height += addVerticalDistance - verticalDistance;
                    Rack.UserSettings.KeyboardHeight = instrumentKeyboard.Height;
                    Canvas.SetTop(instrumentKeyboard, (canvas.Height - instrumentKeyboard.Height) / 2);

                }
                else if (verticalDistance < distance) // same logic but in reverse (so keyboard height will be increased) 
                {
                    instrumentKeyboard.Height -= verticalDistance - addVerticalDistance;
                    Rack.UserSettings.KeyboardHeight = instrumentKeyboard.Height;
                    Canvas.SetTop(instrumentKeyboard, (canvas.Height - instrumentKeyboard.Height) / 2);
                }
                else 
                {
                    instrumentKeyboard.Height = Rack.UserSettings.KeyboardHeight + addVerticalDistance;
                    Canvas.SetTop(instrumentKeyboard, (canvas.Height - instrumentKeyboard.Height) / 2);
                }
                #endregion

                #region Changing Distance Between Keys
                int i = 0;

                // For each key (except for the last one) in keyboards the bottom margin will change.
                foreach (Button key in instrumentKeyboard.Children)
                {
                    if (i != 6 && addVerticalDistance == distance * 6)
                    {
                        key.Margin = new Thickness(0, 0, 0, Rack.UserSettings.KeyVerticaDistance);
                    }
                    else if (i != 11 && addVerticalDistance == distance * 11)
                    {
                        key.Margin = new Thickness(0, 0, 0, Rack.UserSettings.KeyVerticaDistance);
                    }
                    i++;
                }
                #endregion
            }
            verticalDistance = addVerticalDistance;
        }

        // These two variables helps to control which keyboard needs to move and when (afterEighthKeyboard),
        // then where it needs to stop before moving (firstTime).

        // Start to change collocation of keyboards after the selection of the eighth one
        private bool afterEighthKeyboard = false;

        // Start scrollbar movement the second time the MoveKeybaords method is called,
        // or rather, when you look at the second on-screen keyboard after playing the first.
        private bool firstTime = false;
        public void MoveKeyboards()
        {
            if (!firstTime)
            {
                firstTime = true;
            }
            else {
                Rack.DMIBox.MyInstrumentMainWindow.canvasMyInstrument.Width += Rack.UserSettings.KeyHorizontalDistance;
            }

            double distance = Rack.UserSettings.KeyHorizontalDistance;

            if (lastKeyboardSelected != "")
            {
                if (!afterEighthKeyboard)
                {
                    if (lastKeyboardSelected == "8"){

                        // In this first case, the first keyboard, the "_0" one, will be moved after the last one, so the "_15" one.
                        Canvas.SetLeft(MyInstrumentKeyboard.GetKeyboard("_0"), MyInstrumentKeyboard.GetPosition(musicKeyboards[15].Name).X + distance);
                        lastKeyboardMoved = "0";
                        MyInstrumentKeyboard.ResetColors("_0");
                        afterEighthKeyboard = true;
                    }                                     
                }
                else
                {
                    // Since the second case, each keyboard will be moved after the "lastKeyboardMoved",
                    // variable updated after each iteration
                    int lastKS = Int32.Parse(lastKeyboardSelected) - 8; // Integer version of last lastKeyboardSelected (- 8)
                    if (lastKS < 0)
                    {
                        lastKS = 16 + lastKS; // Es: if keyboard n° 5 is played -> 5 - 8 = -3, 16 - 3 = 13 is the keyboard to move.
                    }                    

                    Canvas.SetLeft(MyInstrumentKeyboard.GetKeyboard("_" + lastKS.ToString()), MyInstrumentKeyboard.GetPosition("_" + lastKeyboardMoved).X + distance);
                    lastKeyboardMoved = lastKS.ToString();
                    MyInstrumentKeyboard.ResetColors("_" + lastKeyboardMoved);
                }               
            }
        }
    }
}