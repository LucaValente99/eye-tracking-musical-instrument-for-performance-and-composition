﻿using MyInstrument.DMIbox;
using MyInstrument.Surface;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MyInstrument
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Bool variables for checking activation and deactivation of buttons
        private bool myInstrumentStarted = false;
        private bool myInstrumentSettingsOpened = false;
        private bool musicSheetSettingsOpened = false;
        private bool btnKeyboardOn = false;
        private bool btnFaceOn = false;
        private bool btnDisableWritingMode = false;
        private bool btnSlidePlayOn = false;
        private bool btnSharpNotesOn = false;

        //Dictionaries used to select index into combobox for Start and Stop phases
        private Dictionary<string, int> comboScale = new Dictionary<string, int>()
        {
            {"C", 0 }, { "C#", 1 }, { "D", 2 }, { "D#", 3 }, { "E", 4 }, { "F", 5 }, { "F#", 6 }, {"G", 7 }, { "G#", 8 }, { "A", 9 }, { "A#", 10 }, { "B", 11}, { "_", 12}
        };
        private Dictionary<string, int> comboCode = new Dictionary<string, int>()
        {
            {"maj", 0}, {"min", 1}, {"chrom", 2}, { "_", 3}
        };
        private Dictionary<string, int> comboOctave = new Dictionary<string, int>()
        {
            {"2", 0 }, {"3", 1 }, {"4", 2 }, {"5", 3}, {"6", 4}, { "_", 5}
        };

        private readonly SolidColorBrush ActiveBrush = new SolidColorBrush(Colors.LightYellow);
        private readonly SolidColorBrush WarningBrush = new SolidColorBrush(Colors.DarkRed);
        private readonly SolidColorBrush DisableBrush = new SolidColorBrush(Colors.Transparent);

        //Icons and Backgrounds
        BitmapImage startIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\Images\Icons\Start.png"));

        BitmapImage pauseIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\Images\Icons\Pause.png"));

        BitmapImage settingsIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\Images\Icons\Settings.png"));

        BitmapImage closeSettingsIcon = new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\Images\Icons\CloseSettings_1.png"));

        ImageBrush buttonBackground = new ImageBrush(new BitmapImage(
                    new Uri(Environment.CurrentDirectory + @"\..\..\Images\Backgrounds\Buttons.jpeg")));

        private DispatcherTimer updater;
        public MainWindow()
        {
            InitializeComponent();

            updater = new DispatcherTimer();
            updater.Interval = TimeSpan.FromMilliseconds(10);
            updater.Tick += UpdateWindow;
            updater.Start();           
        }

        private void UpdateWindow(object sender, EventArgs e)
        {
            txtPitch.Text = Rack.UserSettings.NotePitch;
            txtNoteName.Text = Rack.UserSettings.NoteName;
            txtVelocityMouth.Text = Rack.UserSettings.NoteVelocity;

        }

        #region TopBar (Row0)

        #region Start, Exit and Setting buttons

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!myInstrumentStarted)
            {

                MyInstrumentSetup myInstrumentSetup = new MyInstrumentSetup(this);
                myInstrumentSetup.Setup();

                // Graphic changes
                btnStartImage.Source = pauseIcon;
                btnStart.Background = ActiveBrush;
                btnStartLabel.Content = "Running...";

                // Enabling ComboBox & slider
                lstScaleChanger.IsEnabled = true;
                lstScaleChanger.SelectedIndex = comboScale[Rack.UserSettings.ScaleName];
                lstCodeChanger.IsEnabled = true;
                lstCodeChanger.SelectedIndex = comboCode[Rack.UserSettings.ScaleCode];
                lstOctaveChanger.IsEnabled = true;
                lstOctaveChanger.SelectedIndex = comboOctave[Rack.UserSettings.Octave];
                sldVerticalDistance.IsEnabled = true;
                sldHorizontalDistance.IsEnabled = true;


                // MIDI
                txtMidiPort.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();
                CheckMidiPort();

                myInstrumentStarted = true;
            }
            else
            {
                // Graphic changes
                myInstrumentStarted = false;
                btnStartImage.Source = startIcon;
                btnStart.Background = DisableBrush;
                btnStartLabel.Content = "Start";
                txtMidiPort.Text = "";

                // Disabling ComboBox & slider
                lstScaleChanger.IsEnabled = false;
                lstScaleChanger.SelectedIndex = comboScale["_"];
                lstCodeChanger.IsEnabled = false;
                lstCodeChanger.SelectedIndex = comboCode["_"];
                lstOctaveChanger.IsEnabled = false;
                lstOctaveChanger.SelectedIndex = comboOctave["_"];
                sldVerticalDistance.IsEnabled = false;
                sldHorizontalDistance.IsEnabled = false;

                // Resetting surface
                Rack.DMIBox.MyInstrumentSurface.ClearSurface();
            }
        }

        private void btnInstrumentSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!myInstrumentSettingsOpened)
            {
                myInstrumentSettingsOpened = true;

                // Graphic changes
                WindowInstrumentSettings.Visibility = Visibility.Visible;
                btnInstrumentSettingImage.Source = closeSettingsIcon;
                btnInstrumentSettings.Background = ActiveBrush;
                btnInstrumentSettingLabel.Content = "Close Settings";
            }
            else
            {
                myInstrumentSettingsOpened = false;

                // Graphic changes
                WindowInstrumentSettings.Visibility = Visibility.Hidden;
                btnInstrumentSettingImage.Source = settingsIcon;
                btnInstrumentSettings.Background = DisableBrush;
                btnInstrumentSettingLabel.Content = "Instrument Settings";
            }
        }

        private void btnMusicSheetSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!musicSheetSettingsOpened)
            {
                musicSheetSettingsOpened = true;

                // Graphic changes
                WindowMusicSheetSettings.Visibility = Visibility.Visible;
                btnMusicSheetSettingsImage.Source = closeSettingsIcon;
                btnMusicSheetSettings.Background = ActiveBrush;
                btnMusicSheetSettingsLabel.Content = "Close Settings";

            }
            else
            {
                musicSheetSettingsOpened = false;

                // Graphic changes
                WindowMusicSheetSettings.Visibility = Visibility.Hidden;
                btnMusicSheetSettingsImage.Source = settingsIcon;
                btnMusicSheetSettings.Background = DisableBrush;
                btnMusicSheetSettingsLabel.Content = "Music Sheet Settings";
            }
        }

        #endregion Start, Exit and Setting buttons

        #endregion TopBar (Row0)

        #region Instrument (Row1)

        #region Instrument Settings

        private void btnCtrlKeyboard_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (!btnKeyboardOn)
                {
                    btnKeyboardOn = true;
                    btnFaceOn = false;
                    btnCtrlKeyboard.IsEnabled = false;
                    btnCtrlFace.IsEnabled = true;

                    Rack.UserSettings.MyInstrumentControlMode = _MyInstrumentControlModes.Keyboard;
                    Rack.DMIBox.ResetModulationAndPressure();
                }
            }           
        }
        private void btnCtrlFace_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (!btnFaceOn)
                {
                    btnFaceOn = true;
                    btnKeyboardOn = false;
                    btnCtrlFace.IsEnabled = false;
                    btnCtrlKeyboard.IsEnabled = true;

                    Rack.UserSettings.MyInstrumentControlMode = _MyInstrumentControlModes.Face;
                    Rack.DMIBox.ResetModulationAndPressure();
                }
            }

        }

        private void btnMidiPortMinus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.MIDIPort--;
                Rack.DMIBox.MidiModule.OutDevice = Rack.UserSettings.MIDIPort;

                // Graphic changes
                txtMidiPort.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();
                CheckMidiPort();
            }
        }

        private void btnMidiPortPlus_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.MIDIPort++;
                Rack.DMIBox.MidiModule.OutDevice = Rack.UserSettings.MIDIPort;

                // graphic changes
                txtMidiPort.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();
                CheckMidiPort();
            }
        }
        private void CheckMidiPort()
        {
            if (Rack.DMIBox.MidiModule.IsMidiOk())
            {
                txtMidiPort.Foreground = ActiveBrush;
            }
            else
            {
                txtMidiPort.Foreground = WarningBrush;
            }
        }

        private void lstScaleChanger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (myInstrumentStarted)
            {
                Rack.UserSettings.ScaleName = (e.AddedItems[0] as ComboBoxItem).Content as string;
                Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
            }
        }

        private void lstCodeChanger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (myInstrumentStarted)
            {
                Rack.UserSettings.ScaleCode = (e.AddedItems[0] as ComboBoxItem).Content as string;
                Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
            }
        }

        private void lstOctaveChanger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (myInstrumentStarted)
            {
                Rack.UserSettings.Octave = (e.AddedItems[0] as ComboBoxItem).Content as string;
                Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
            }
        }

        // Setting vertical distance between keys in keyboards
        private void sldVerticalDistance_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.keyVerticaDistance = sldVerticalDistance.Value;
                Rack.DMIBox.MyInstrumentSurface.SetVerticalDistance(sldVerticalDistance.Value);
            }
        }

        // Setting horizontal distance between keyboards
        private void sldHorizontalDistance_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                Rack.UserSettings.keyHorizontalDistance = sldHorizontalDistance.Value;
                Rack.DMIBox.MyInstrumentSurface.SetHorizontalDistance(sldHorizontalDistance.Value);
            }
        }

        private void btnSlidePlay_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (!btnSlidePlayOn)
                {
                    btnSlidePlayOn = true;

                    btnSlidePlay.Background = ActiveBrush;
                    Rack.UserSettings.SlidePlayMode = _SlidePlayModes.On;
                    MyInstrumentButtons.resetSlidePlay();
                }
                else
                {
                    btnSlidePlayOn = false;

                    btnSlidePlay.Background = buttonBackground;
                    Rack.UserSettings.SlidePlayMode = _SlidePlayModes.Off;

                }
            }
        }

        private void btnSharpNotes_Click(object sender, RoutedEventArgs e)
        {
            if (myInstrumentStarted)
            {
                if (!btnSharpNotesOn)
                {
                    btnSharpNotesOn = true;

                    btnSharpNotes.Background = ActiveBrush;
                    Rack.UserSettings.SharpNotesMode = _SharpNotesModes.On;
                    Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();
                }
                else
                {
                    btnSharpNotesOn = false;

                    btnSharpNotes.Background = buttonBackground;
                    Rack.UserSettings.SharpNotesMode = _SharpNotesModes.Off;
                    Rack.DMIBox.MyInstrumentSurface.DrawOnCanvas();

                }
            }
        }

        #endregion Instrument Settings       

        #region Music Sheet Settings

        #endregion Music Sheet Settings

        #endregion Instrument (Row1)

        #region MusicSheet (Row2)

        private void btnDisable_Click(object sender, RoutedEventArgs e)
        {
            if (!btnDisableWritingMode)
            {
                btnDisableWritingMode = true;
                btnDisable.Background = ActiveBrush;
            }
            else
            {
                btnDisableWritingMode = false;
                btnDisable.Background = buttonBackground;

            }
        }

        #endregion MusicSheet (Row2)

        
    }
        
}