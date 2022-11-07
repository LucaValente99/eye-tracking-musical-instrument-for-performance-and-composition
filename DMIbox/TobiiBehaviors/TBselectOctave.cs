﻿using MyInstrument.DMIbox;
using MyInstrument;
using NeeqDMIs.Eyetracking.Tobii;
using NeeqDMIs.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInstrument.DMIbox.TobiiBehaviors
{
    public class TBselectOctave : ATobiiBlinkBehavior
    {
        public TBselectOctave()
        {
            LCThresh = 2;
            RCThresh = 2;
        }

        public override void Event_doubleClose() { }

        public override void Event_doubleOpen() { }

        public override void Event_leftClose()
        {
            if (Rack.UserSettings.BlinkModes == _BlinkModes.Octave)
            {
                if (Rack.DMIBox.MyInstrumentMainWindow.OctaveIndex > 0)
                {
                    Rack.DMIBox.MyInstrumentMainWindow.OctaveIndex--;
                    Rack.DMIBox.MyInstrumentMainWindow.txtOctave.Text = Rack.DMIBox.MyInstrumentMainWindow.ComboOctave[Rack.DMIBox.MyInstrumentMainWindow.OctaveIndex];
                    Rack.UserSettings.Octave = Rack.DMIBox.MyInstrumentMainWindow.txtOctave.Text;
                    Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
            }
        }

        public override void Event_leftOpen() { }

        public override void Event_rightClose()
        {
            if (Rack.UserSettings.BlinkModes == _BlinkModes.Octave)
            {
                if (Rack.DMIBox.MyInstrumentMainWindow.CodeIndex < 4)
                {
                    Rack.DMIBox.MyInstrumentMainWindow.OctaveIndex++;
                    Rack.DMIBox.MyInstrumentMainWindow.txtOctave.Text = Rack.DMIBox.MyInstrumentMainWindow.ComboOctave[Rack.DMIBox.MyInstrumentMainWindow.OctaveIndex];
                    Rack.UserSettings.Octave = Rack.DMIBox.MyInstrumentMainWindow.txtOctave.Text; 
                    Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
            }
        }
        public override void Event_rightOpen() { }
    }
}

