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
        private static RoutedCommand switchSettingsCommand = new RoutedCommand("SPAD_neXt.SwitchSettings", typeof(SPADCommands));

        private static RoutedCommand profileActivateCommand = new RoutedCommand("SPAD_neXt.Profile_Activate", typeof(SPADCommands));
        private static RoutedCommand profileCreateNewCommand = new RoutedCommand("SPAD_neXt.Profile_CreateNew", typeof(SPADCommands));
        private static RoutedCommand profileNewFromThisCommand = new RoutedCommand("SPAD_neXt.Profile_NewFromThis", typeof(SPADCommands));
        private static RoutedCommand profileDeleteCommand = new RoutedCommand("SPAD_neXt.Profile_Delete", typeof(SPADCommands));
        private static RoutedCommand profileEditCommand = new RoutedCommand("SPAD_neXt.Profile_Edit", typeof(SPADCommands));
        private static RoutedCommand profileEditAircraftsCommand = new RoutedCommand("SPAD_neXt.Profile_Edit_Aicrafts", typeof(SPADCommands));
        private static RoutedCommand profileEditDevicesCommand = new RoutedCommand("SPAD_neXt.Profile_Edit_Devcices", typeof(SPADCommands));
        private static RoutedCommand profilePublishCommand = new RoutedCommand("SPAD_neXt.Profile_Publish", typeof(SPADCommands));
        private static RoutedCommand profileReconvertCommand = new RoutedCommand("SPAD_neXt.Profile_Reconvert", typeof(SPADCommands));
        private static RoutedCommand profileRateCommand = new RoutedCommand("SPAD_neXt.Profile_Rate", typeof(SPADCommands));
        private static RoutedCommand profileUnpublishCommand = new RoutedCommand("SPAD_neXt.Profile_Unpublish", typeof(SPADCommands));

        private static RoutedCommand _CopyAllCommand = new RoutedCommand("SPAD_neXt.CopyAll", typeof(SPADCommands));
        private static RoutedCommand _CopyDeviceCommand = new RoutedCommand("SPAD_neXt.CopyDevice", typeof(SPADCommands));
        private static RoutedCommand _PublishProfileCommand = new RoutedCommand("SPAD_neXt.PublishProfile", typeof(SPADCommands));
        private static RoutedCommand _PublishSnippetCommand = new RoutedCommand("SPAD_neXt.PublishSnippet", typeof(SPADCommands));

        public static RoutedCommand CommandDevicePowerON { get; } = new RoutedCommand("SPAD_neXt.Device_PowerON", typeof(SPADCommands));
        public static RoutedCommand CommandDevicePowerOFF { get; } = new RoutedCommand("SPAD_neXt.Device_PowerOFF", typeof(SPADCommands));
        public static RoutedCommand CommandGaugeNext { get; } = new RoutedCommand("SPAD_neXt.Gauge_Next", typeof(SPADCommands));
        public static RoutedCommand CommandGaugePrev { get; } = new RoutedCommand("SPAD_neXt.Gauge_Prev", typeof(SPADCommands));
//        public static RoutedCommand CommandGaugeSet { get; } = new RoutedCommand("SPAD_neXt.Gauge_Set", typeof(SPADCommands));
        public static RoutedCommand CommandGaugeSwitch { get; } = new RoutedCommand("SPAD_neXt.Gauge_Switch", typeof(SPADCommands));
        public static RoutedCommand CommandDeviceStop { get; } = new RoutedCommand("SPAD_neXt.Device_STOP", typeof(SPADCommands));
        public static RoutedCommand CommandDeviceStart { get; } = new RoutedCommand("SPAD_neXt.Device_START", typeof(SPADCommands));
        public static RoutedCommand CommandChangeDigitmark { get; } = new RoutedCommand("DIGITMARK", typeof(SPADCommands));
        public static RoutedCommand CommandPlaySound { get; } = new RoutedCommand("SPAD_neXt.PlaySound", typeof(SPADCommands));
        public static RoutedCommand CommandEmulate { get; } = new RoutedCommand("SPAD_neXt.Emulate", typeof(SPADCommands));
        public static RoutedCommand CommandRemoteEvent { get; } = new RoutedCommand("SPAD_neXt.Remote", typeof(SPADCommands));
        public static RoutedCommand CommandSendMessage { get; } = new RoutedCommand("SPAD_neXt.Message", typeof(SPADCommands));
        public static RoutedCommand CommandRunProgram { get; } = new RoutedCommand("SPAD_neXt.RunProgram", typeof(SPADCommands));
        public static RoutedCommand CommandConfigureSwitch { get; } = new RoutedCommand("SPAD_neXt.Device_ConfigureSwitch", typeof(SPADCommands));

        public static RoutedCommand CommandSendCDUKey { get; } = new RoutedCommand("SPAD_neXt.SendCDUKey", typeof(SPADCommands));

        private static List<RoutedCommand> _ExposedCommands = new List<RoutedCommand>();
        public static IEnumerable<RoutedCommand> ExposedCommands
        {
            get { return _ExposedCommands; }
        }

        public static List<string> DeviceCommands = new List<string>()
        {
            CommandGaugeSwitch.Name,CommandGaugeNext.Name,CommandGaugePrev.Name,CommandDevicePowerOFF.Name,CommandDevicePowerON.Name
        };

        static SPADCommands()
        {
            _ExposedCommands.Add(CommandDevicePowerON);
            _ExposedCommands.Add(CommandDevicePowerOFF);
            //_ExposedCommands.Add(CommandDeviceStart);
            //_ExposedCommands.Add(CommandDeviceStop);
            _ExposedCommands.Add(CommandGaugeNext);
            _ExposedCommands.Add(CommandGaugePrev);
            //_ExposedCommands.Add(CommandGaugeSet);
            _ExposedCommands.Add(CommandGaugeSwitch);
            _ExposedCommands.Add(CommandChangeDigitmark);
            _ExposedCommands.Add(CommandEmulate);
            _ExposedCommands.Add(CommandRemoteEvent);
            _ExposedCommands.Add(CommandSendMessage);
            _ExposedCommands.Add(CommandRunProgram);
            _ExposedCommands.Add(CommandSendCDUKey);
        }

        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static RoutedCommand SaveCommand { get { return saveCommand; } }
        public static RoutedCommand ConfigureSpecialCommand { get { return configureSpecialCommand; } }
        public static RoutedCommand CalibrateAxisCommand { get { return calibrateAxisCommand; } }
        public static RoutedCommand AxisCurveCommand { get; } = new RoutedCommand("SPAD_neXt.AxisCurve", typeof(SPADCommands));

        public static RoutedCommand PowerConfigurationCommand { get { return powerConfigurationCommand; } }
        public static RoutedCommand DeviceConfigurationCommand { get { return deviceConfigurationCommand; } }

        public static RoutedCommand DeviceSwitchPanelCommand { get; } = new RoutedCommand("SPAD_neXt.DeviceSwitchPanel", typeof(SPADCommands));

        public static RoutedCommand DeviceReconnectCommand { get; } = new RoutedCommand("SPAD_neXt.ReconnectDevice", typeof(SPADCommands));
        public static RoutedCommand DeviceDisconnectCommand { get; } = new RoutedCommand("SPAD_neXt.DisconnectDevice", typeof(SPADCommands));
        public static RoutedCommand DeviceCopyCommand { get { return _CopyDeviceCommand; } }
        public static RoutedCommand DeviceActivateCommand { get; } = new RoutedCommand("SPAD_neXt.DeviceActivate", typeof(SPADCommands));
        public static RoutedCommand DeviceDeactivateCommand { get; } = new RoutedCommand("SPAD_neXt.DeviceDeactivate", typeof(SPADCommands));
        public static RoutedCommand DeviceToggleActiveCommand { get; } = new RoutedCommand("SPAD_neXt.DeviceToggleActive", typeof(SPADCommands));
        public static RoutedCommand AddEventCommand { get { return addEventCommand; } }
        public static RoutedCommand EditEventCommand { get { return editEventCommand; } }
        public static RoutedCommand DelEventCommand { get { return delEventCommand; } }
        public static RoutedCommand ChangeEventTriggerCommand { get; } = new RoutedCommand("SPAD_neXt.EventChangeTrigger", typeof(SPADCommands));
        public static RoutedCommand OnlineActionCommand { get { return onlineActionCommand; } }
        public static RoutedCommand ChangeLabelCommand { get { return changeLabelCommand; } }
        public static RoutedCommand DoNothingCommand { get { return doNothingCommand; } }
        public static RoutedCommand SwitchSettingsCommand => switchSettingsCommand;
        public static RoutedCommand CopyAllCommand { get { return _CopyAllCommand; } }
        public static RoutedCommand CopyPageCommand { get; } = new RoutedCommand("SPAD_neXt.CopyPage", typeof(SPADCommands));
        public static RoutedCommand RefreshPageCommand { get; } = new RoutedCommand("SPAD_neXt.RefreshPage", typeof(SPADCommands));

        public static RoutedCommand ConditionAddCommand { get; } = new RoutedCommand("SPAD_next.ConditionAdd", typeof(SPADCommands));
        public static RoutedCommand ConditionEditCommand { get; } = new RoutedCommand("SPAD_next.ConditionEdit", typeof(SPADCommands));
        public static RoutedCommand ConditionDeleteCommand { get; } = new RoutedCommand("SPAD_next.ConditionDelete", typeof(SPADCommands));
        public static RoutedCommand ConditionSaveCommand { get; } = new RoutedCommand("SPAD_next.ConditionSave", typeof(SPADCommands));
        public static RoutedCommand ConditionPasteCommand { get; } = new RoutedCommand("SPAD_next.ConditionPaste", typeof(SPADCommands));

        public static RoutedCommand CategoryAddCommand { get; } = new RoutedCommand("SPAD_next.CategoryAdd", typeof(SPADCommands));
        public static RoutedCommand CategoryEditCommand { get; } = new RoutedCommand("SPAD_next.CategoryEdit", typeof(SPADCommands));
        public static RoutedCommand ImportCommand { get; } = new RoutedCommand("SPAD_neXt.Commands.Import", typeof(SPADCommands));
        public static RoutedCommand InheritCommand { get; } = new RoutedCommand("SPAD_neXt.Commands.Inherit", typeof(SPADCommands));
        public static RoutedCommand PublishProfileCommand { get { return _PublishProfileCommand; } }
        public static RoutedCommand PublishSnippetCommand { get { return _PublishSnippetCommand; } }

        public static RoutedCommand ProfileActivateCommand {  get { return profileActivateCommand; } }
        public static RoutedCommand ProfileCreateNewCommand { get { return profileCreateNewCommand; } }
        public static RoutedCommand ProfileNewFromThisCommand { get { return profileNewFromThisCommand; } }
        public static RoutedCommand ProfileDeleteCommand { get { return profileDeleteCommand; } }
        public static RoutedCommand ProfileEditCommand { get { return profileEditCommand; } }
        public static RoutedCommand ProfileEditAircraftsCommand { get { return profileEditAircraftsCommand; } }
        public static RoutedCommand ProfileEditDevicesCommand { get { return profileEditDevicesCommand; } }
        public static RoutedCommand ProfilePublishCommand { get { return profilePublishCommand; } }
        public static RoutedCommand ProfileReconvertCommand { get { return profileReconvertCommand; } }
        public static RoutedCommand ProfileUnpublishCommand { get { return profileUnpublishCommand; } }
        public static RoutedCommand ProfileRateCommand { get { return profileReconvertCommand; } }
        public static RoutedCommand ProfileUpgradeCommand { get; } = new RoutedCommand("SPAD_neXt.Profile_Upgrade", typeof(SPADCommands));
        public static RoutedCommand ShowNotificationsCommand { get; } = new RoutedCommand("SPAD_neXt._ShowNotificationsCommand", typeof(SPADCommands));
        public static RoutedCommand ShowStatusMessageCommand { get; } = new RoutedCommand("SPAD_neXt._ShowStatusMessageCommand", typeof(SPADCommands));
        public static RoutedCommand DebugModeCommand { get; } = new RoutedCommand("SPAD_neXt._DebugModeCommand", typeof(SPADCommands));
        public static RoutedCommand ProgrammingCommand { get; } = new RoutedCommand("SPAD_neXt.ProgrammingCommand", typeof(SPADCommands));
        public static RoutedCommand NotImplemented { get { return notImplementedCommand; } }

        public static RoutedCommand EventDisableCommand { get; } = new RoutedCommand("SPAD_neXt.EventDisable", typeof(SPADCommands));
        public static RoutedCommand EventEnableCommand { get; } = new RoutedCommand("SPAD_neXt.EventEnable", typeof(SPADCommands));

        public static RoutedCommand EventMoveUpCommand { get; } = new RoutedCommand("SPAD_neXt.EventMoveUp", typeof(SPADCommands));
        public static RoutedCommand EventMoveDownCommand { get; } = new RoutedCommand("SPAD_neXt.EventMoveDown", typeof(SPADCommands));
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
