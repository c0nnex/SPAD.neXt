using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SPAD.neXt.Interfaces
{
    public static class SPADCommands
    {
        private static RoutedCommand saveCommand = new RoutedCommand("SPAD_neXt.Save", typeof(SPADCommands));
        private static RoutedCommand configureSpecialCommand = new RoutedCommand("SPAD_neXt.Configure_Special", typeof(SPADCommands));
        private static RoutedCommand calibrateAxisCommand = new RoutedCommand("SPAD_neXt.calibrateAxis", typeof(SPADCommands));
        private static RoutedCommand powerConfigurationCommand = new RoutedCommand("SPAD_neXt.PowerConfiguration", typeof(SPADCommands));
        private static RoutedCommand deviceConfigurationCommand = new RoutedCommand("SPAD_neXt.DeviceConfiguration", typeof(SPADCommands));

        private static RoutedCommand addEventCommand = new RoutedCommand("SPAD_neXt.AddEvent", typeof(SPADCommands));
        private static RoutedCommand editEventCommand = new RoutedCommand("SPAD_neXt.EditEvent", typeof(SPADCommands));
        private static RoutedCommand delEventCommand = new RoutedCommand("SPAD_neXt.DelEvent", typeof(SPADCommands));
        private static RoutedCommand onlineActionCommand = new RoutedCommand("SPAD_neXt.AddAction", typeof(SPADCommands));
        private static RoutedCommand changeLabelCommand = new RoutedCommand("SPAD_neXt.ChangeLabel", typeof(SPADCommands));
        private static RoutedCommand doNothingCommand = new RoutedCommand("SPAD_neXt.DoNothing", typeof(SPADCommands));
        private static RoutedCommand notImplementedCommand = new RoutedCommand("SPAD_neXt.NotImplmented", typeof(SPADCommands));

        private static RoutedCommand profileActivateCommand = new RoutedCommand("SPAD_neXt.Profile_Activate", typeof(SPADCommands));
        private static RoutedCommand profileCreateNewCommand = new RoutedCommand("SPAD_neXt.Profile_CreateNew", typeof(SPADCommands));
        private static RoutedCommand profileNewFromThisCommand = new RoutedCommand("SPAD_neXt.Profile_NewFromThis", typeof(SPADCommands));
        private static RoutedCommand profileDeleteCommand = new RoutedCommand("SPAD_neXt.Profile_Delete", typeof(SPADCommands));
        private static RoutedCommand profileEditCommand = new RoutedCommand("SPAD_neXt.Profile_Edit", typeof(SPADCommands));
        private static RoutedCommand profilePublishCommand = new RoutedCommand("SPAD_neXt.Profile_Publish", typeof(SPADCommands));
        private static RoutedCommand profileReconvertCommand = new RoutedCommand("SPAD_neXt.Profile_Reconvert", typeof(SPADCommands));
        private static RoutedCommand profileRateCommand = new RoutedCommand("SPAD_neXt.Profile_Rate", typeof(SPADCommands));

        private static RoutedCommand _CopyThisCommand = new RoutedCommand("SPAD_neXt.CopyThis", typeof(SPADCommands));
        private static RoutedCommand _CopyAllCommand = new RoutedCommand("SPAD_neXt.CopyAll", typeof(SPADCommands));
        private static RoutedCommand _CopyDeviceCommand = new RoutedCommand("SPAD_neXt.CopyDevice", typeof(SPADCommands));
        private static RoutedCommand _PasteCommand = new RoutedCommand("SPAD_neXt.Paste", typeof(SPADCommands));
        private static RoutedCommand _PublishProfileCommand = new RoutedCommand("SPAD_neXt.PublishProfile", typeof(SPADCommands));
        private static RoutedCommand _PublishSnippetCommand = new RoutedCommand("SPAD_neXt.PublishSnippet", typeof(SPADCommands));

        private static RoutedCommand _ShowNotificationsCommand = new RoutedCommand("SPAD_neXt._ShowNotificationsCommand", typeof(SPADCommands));

        

        /*  private static RoutedCommand editActionCommand = new RoutedCommand("SPAD_neXt.EditAction", typeof(SPADCommands));
          private static RoutedCommand delActionCommand = new RoutedCommand("SPAD_neXt.DelAction", typeof(SPADCommands));
          private static RoutedCommand moveActionUpCommand = new RoutedCommand("SPAD_neXt.MoveActionUP", typeof(SPADCommands));
          private static RoutedCommand moveActionDownCommand = new RoutedCommand("SPAD_neXt.MoveActionDOWN", typeof(SPADCommands));
          */

        // Exposed Commands
        private static RoutedCommand devicePowerONCommand = new RoutedCommand("SPAD_neXt.Device_PowerON", typeof(SPADCommands));
        private static RoutedCommand devicePowerOFFCommand = new RoutedCommand("SPAD_neXt.Device_PowerOFF", typeof(SPADCommands));
        private static RoutedCommand gaugeNextCommand = new RoutedCommand("SPAD_neXt.Gauge_Next", typeof(SPADCommands));
        private static RoutedCommand gaugePrevCommand = new RoutedCommand("SPAD_neXt.Gauge_Prev", typeof(SPADCommands));
        private static RoutedCommand _CommandChangeDigitmark = new RoutedCommand("DIGITMARK", typeof(SPADCommands));
        private static RoutedCommand _CommandPlaySound = new RoutedCommand("SPAD_neXt.PlaySound", typeof(SPADCommands));
        private static RoutedCommand _EmulateCommand = new RoutedCommand("SPAD_neXt.Emulate", typeof(SPADCommands));
        private static RoutedCommand _RemoteEventCommand = new RoutedCommand("SPAD_neXt.Remote", typeof(SPADCommands));

        public static RoutedCommand CommandDevicePowerON { get { return devicePowerONCommand; } }
        public static RoutedCommand CommandDevicePowerOFF { get { return devicePowerOFFCommand; } }
        public static RoutedCommand CommandGaugeNext { get { return gaugeNextCommand; } }
        public static RoutedCommand CommandGaugePrev { get { return gaugePrevCommand; } }
        public static RoutedCommand CommandChangeDigitmark { get { return _CommandChangeDigitmark; } }
        public static RoutedCommand CommandPlaySound { get { return _CommandPlaySound; } }
        public static RoutedCommand CommandEmulate { get { return _EmulateCommand; } }
        public static RoutedCommand CommandRemoteEvent { get { return _RemoteEventCommand; } }

        private static List<RoutedCommand> _ExposedCommands = new List<RoutedCommand>();
        public static IEnumerable<RoutedCommand> ExposedCommands
        {
            get { return _ExposedCommands; }
        }

        static SPADCommands()
        {
            _ExposedCommands.Add(CommandDevicePowerON);
            _ExposedCommands.Add(CommandDevicePowerOFF);
            _ExposedCommands.Add(CommandGaugeNext);
            _ExposedCommands.Add(CommandGaugePrev);
            _ExposedCommands.Add(CommandChangeDigitmark);
            _ExposedCommands.Add(CommandEmulate);
            _ExposedCommands.Add(CommandRemoteEvent);
        }

        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static RoutedCommand SaveCommand { get { return saveCommand; } }
        public static RoutedCommand ConfigureSpecialCommand { get { return configureSpecialCommand; } }
        public static RoutedCommand CalibrateAxisCommand { get { return calibrateAxisCommand; } }
        public static RoutedCommand PowerConfigurationCommand { get { return powerConfigurationCommand; } }
        public static RoutedCommand DeviceConfigurationCommand { get { return powerConfigurationCommand; } }

        public static RoutedCommand AddEventCommand { get { return addEventCommand; } }
        public static RoutedCommand EditEventCommand { get { return editEventCommand; } }
        public static RoutedCommand DelEventCommand { get { return delEventCommand; } }
        public static RoutedCommand OnlineActionCommand { get { return onlineActionCommand; } }
        public static RoutedCommand ChangeLabelCommand { get { return changeLabelCommand; } }
        public static RoutedCommand DoNothingCommand { get { return doNothingCommand; } }

        public static RoutedCommand CopyThisCommand { get { return _CopyThisCommand; } }
        public static RoutedCommand CopyAllCommand { get { return _CopyAllCommand; } }
        public static RoutedCommand CopyDeviceCommand { get { return _CopyDeviceCommand; } }
        public static RoutedCommand PasteCommand { get { return _PasteCommand; } }
        public static RoutedCommand PublishProfileCommand { get { return _PublishProfileCommand; } }
        public static RoutedCommand PublishSnippetCommand { get { return _PublishSnippetCommand; } }

        public static RoutedCommand ProfileActivateCommand {  get { return profileActivateCommand; } }
        public static RoutedCommand ProfileCreateNewCommand { get { return profileCreateNewCommand; } }
        public static RoutedCommand ProfileNewFromThisCommand { get { return profileNewFromThisCommand; } }
        public static RoutedCommand ProfileDeleteCommand { get { return profileDeleteCommand; } }
        public static RoutedCommand ProfileEditCommand { get { return profileEditCommand; } }
        public static RoutedCommand ProfilePublishCommand { get { return profilePublishCommand; } }
        public static RoutedCommand ProfileReconvertCommand { get { return profileReconvertCommand; } }
        public static RoutedCommand ProfileRateCommand { get { return profileReconvertCommand; } }

        public static RoutedCommand ShowNotificationsCommand { get { return _ShowNotificationsCommand; } }

        public static RoutedCommand NotImplemented { get { return notImplementedCommand; } }

        /*
        public static RoutedCommand EditActionCommand { get { return editActionCommand; } }
        public static RoutedCommand DelActionCommand { get { return delActionCommand; } }
        public static RoutedCommand MoveActionUpCommand { get { return moveActionUpCommand; } }
        public static RoutedCommand MoveActionDownCommand { get { return moveActionDownCommand; } }
        */

        public static void OnCanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public static void OnCanNeverExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        public static void DoNothingExecute(object sender, ExecutedRoutedEventArgs e)
        {
           
        }

        public static void NotImplementedExecute(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Not Implemented");
        }
    }
}
