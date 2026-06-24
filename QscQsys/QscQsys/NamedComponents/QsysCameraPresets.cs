using System;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public sealed class QsysCameraPresets : AbstractQsysComponent
    {
        private const int NUM_CAMERAS = 6;
        private const int NUM_PRESETS = 16;
        private const int NUM_ROUTER_OUTPUTS = 11;

        // Control name formats - these match Q-SYS QRC names exactly
        private const string CAM_SELECT_FORMAT = "Cam Select {0}";
        private const string CAM_PRESET_RECALL_FORMAT = "Cam{0}PresetRecall {1}";
        private const string CAM_PRESET_SAVE_FORMAT = "Cam{0}PresetSave {1}";
        private const string CAMERA_NAME_FORMAT = "Camera {0}";
        private const string CAMERA_PRESETS_FORMAT = "Camera {0} Presets {1}";
        private const string CAMERA_POSITION_FORMAT = "Camera{0}Position";
        private const string PRESET_BUTTONS_FORMAT = "PresetButtons {0}";
        private const string PRESET_RECALL_FORMAT = "PresetRecall {0}";
        private const string PRESET_SAVE_FORMAT = "PresetSave {0}";
        private const string PNH_PROGRESS_FORMAT = "PnHProgress {0}";
        private const string ROUTER_OUTPUT_FORMAT = "RouterOutput {0}";

        private const string TRACKING = "Tracking";
        private const string LOAD_PRESETS = "LoadPresets";
        private const string SAVE_PRESETS = "SavePresets";
        private const string RECALIBRATE = "Recalibrate";
        private const string REFRESH_FILES = "RefreshFiles";
        private const string STATUS_TEXT = "Status Text";
        private const string AUTOSAVE_STATUS = "AutosaveStatus";
        private const string DISABLE = "Disable";
        private const string CREATE_FOLDER = "CreateFolder";

        #region Delegates

        public delegate void CamSelectChange(SimplSharpString cName, ushort camera, ushort value);
        public delegate void PresetButtonChange(SimplSharpString cName, ushort preset, ushort value);
        public delegate void PnHProgressChange(SimplSharpString cName, ushort preset, ushort value);
        public delegate void PresetRecallChange(SimplSharpString cName, ushort preset, ushort value);
        public delegate void RouterOutputChange(SimplSharpString cName, ushort output, ushort value);
        public delegate void BoolChange(SimplSharpString cName, ushort value);
        public delegate void StringChange(SimplSharpString cName, SimplSharpString value);
        public delegate void CameraNameChange(SimplSharpString cName, ushort camera, SimplSharpString value);
        public delegate void PresetNameChange(SimplSharpString cName, ushort camera, ushort preset, SimplSharpString value);
        public delegate void CameraPositionChange(SimplSharpString cName, ushort camera, SimplSharpString value);

        #endregion

        #region Delegate Properties

        public CamSelectChange newCamSelectChange { get; set; }
        public PresetButtonChange newPresetButtonChange { get; set; }
        public PnHProgressChange newPnHProgressChange { get; set; }
        public PresetRecallChange newPresetRecallChange { get; set; }
        public RouterOutputChange newRouterOutputChange { get; set; }
        public BoolChange newTrackingChange { get; set; }
        public BoolChange newDisableChange { get; set; }
        public StringChange newStatusTextChange { get; set; }
        public StringChange newAutosaveStatusChange { get; set; }
        public CameraNameChange newCameraNameChange { get; set; }
        public PresetNameChange newPresetNameChange { get; set; }
        public CameraPositionChange newCameraPositionChange { get; set; }

        #endregion

        #region Controls

        private readonly NamedComponentControl[] _camSelectControls = new NamedComponentControl[NUM_CAMERAS];
        private readonly NamedComponentControl[] _presetButtonControls = new NamedComponentControl[NUM_PRESETS];
        private readonly NamedComponentControl[] _presetRecallControls = new NamedComponentControl[NUM_PRESETS];
        private readonly NamedComponentControl[] _pnhProgressControls = new NamedComponentControl[NUM_PRESETS];
        private readonly NamedComponentControl[] _routerOutputControls = new NamedComponentControl[NUM_ROUTER_OUTPUTS];
        private readonly NamedComponentControl[] _cameraNameControls = new NamedComponentControl[NUM_CAMERAS];
        private readonly NamedComponentControl[,] _presetNameControls = new NamedComponentControl[NUM_CAMERAS, NUM_PRESETS];
        private readonly NamedComponentControl[] _cameraPositionControls = new NamedComponentControl[NUM_CAMERAS];
        private NamedComponentControl _trackingControl;
        private NamedComponentControl _disableControl;
        private NamedComponentControl _statusTextControl;
        private NamedComponentControl _autosaveStatusControl;

        #endregion

        public void Initialize(string coreId, string componentName)
        {
            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            // Unsubscribe all existing controls
            for (int i = 0; i < NUM_CAMERAS; i++)
            {
                UnsubscribeControl(_camSelectControls[i]);
                UnsubscribeControl(_cameraNameControls[i]);
                UnsubscribeControl(_cameraPositionControls[i]);
                for (int j = 0; j < NUM_PRESETS; j++)
                    UnsubscribeControl(_presetNameControls[i, j]);
            }

            for (int i = 0; i < NUM_PRESETS; i++)
            {
                UnsubscribeControl(_presetButtonControls[i]);
                UnsubscribeControl(_presetRecallControls[i]);
                UnsubscribeControl(_pnhProgressControls[i]);
            }

            for (int i = 0; i < NUM_ROUTER_OUTPUTS; i++)
                UnsubscribeControl(_routerOutputControls[i]);

            UnsubscribeControl(_trackingControl);
            UnsubscribeControl(_disableControl);
            UnsubscribeControl(_statusTextControl);
            UnsubscribeControl(_autosaveStatusControl);

            if (component == null)
            {
                ClearAllControls();
                return;
            }

            // Subscribe camera select, names, positions
            for (int i = 0; i < NUM_CAMERAS; i++)
            {
                int cam = i + 1;
                _camSelectControls[i] = component.LazyLoadComponentControl(string.Format(CAM_SELECT_FORMAT, cam));
                _cameraNameControls[i] = component.LazyLoadComponentControl(string.Format(CAMERA_NAME_FORMAT, cam));
                _cameraPositionControls[i] = component.LazyLoadComponentControl(string.Format(CAMERA_POSITION_FORMAT, cam));

                SubscribeControl(_camSelectControls[i], CamSelectControlChanged);
                SubscribeControl(_cameraNameControls[i], CameraNameControlChanged);
                SubscribeControl(_cameraPositionControls[i], CameraPositionControlChanged);

                // Subscribe preset names for each camera
                for (int j = 0; j < NUM_PRESETS; j++)
                {
                    int preset = j + 1;
                    _presetNameControls[i, j] = component.LazyLoadComponentControl(
                        string.Format(CAMERA_PRESETS_FORMAT, cam, preset));
                    SubscribeControl(_presetNameControls[i, j], PresetNameControlChanged);
                }
            }

            // Subscribe preset buttons, recall, PnH progress
            for (int i = 0; i < NUM_PRESETS; i++)
            {
                int preset = i + 1;
                _presetButtonControls[i] = component.LazyLoadComponentControl(string.Format(PRESET_BUTTONS_FORMAT, preset));
                _presetRecallControls[i] = component.LazyLoadComponentControl(string.Format(PRESET_RECALL_FORMAT, preset));
                _pnhProgressControls[i] = component.LazyLoadComponentControl(string.Format(PNH_PROGRESS_FORMAT, preset));

                SubscribeControl(_presetButtonControls[i], PresetButtonControlChanged);
                SubscribeControl(_presetRecallControls[i], PresetRecallControlChanged);
                SubscribeControl(_pnhProgressControls[i], PnHProgressControlChanged);
            }

            // Subscribe router outputs
            for (int i = 0; i < NUM_ROUTER_OUTPUTS; i++)
            {
                int output = i + 1;
                _routerOutputControls[i] = component.LazyLoadComponentControl(string.Format(ROUTER_OUTPUT_FORMAT, output));
                SubscribeControl(_routerOutputControls[i], RouterOutputControlChanged);
            }

            // Subscribe single controls
            _trackingControl = component.LazyLoadComponentControl(TRACKING);
            _disableControl = component.LazyLoadComponentControl(DISABLE);
            _statusTextControl = component.LazyLoadComponentControl(STATUS_TEXT);
            _autosaveStatusControl = component.LazyLoadComponentControl(AUTOSAVE_STATUS);

            SubscribeControl(_trackingControl, TrackingControlChanged);
            SubscribeControl(_disableControl, DisableControlChanged);
            SubscribeControl(_statusTextControl, StatusTextControlChanged);
            SubscribeControl(_autosaveStatusControl, AutosaveStatusControlChanged);
        }

        private void ClearAllControls()
        {
            for (int i = 0; i < NUM_CAMERAS; i++)
            {
                _camSelectControls[i] = null;
                _cameraNameControls[i] = null;
                _cameraPositionControls[i] = null;
                for (int j = 0; j < NUM_PRESETS; j++)
                    _presetNameControls[i, j] = null;
            }

            for (int i = 0; i < NUM_PRESETS; i++)
            {
                _presetButtonControls[i] = null;
                _presetRecallControls[i] = null;
                _pnhProgressControls[i] = null;
            }

            for (int i = 0; i < NUM_ROUTER_OUTPUTS; i++)
                _routerOutputControls[i] = null;

            _trackingControl = null;
            _disableControl = null;
            _statusTextControl = null;
            _autosaveStatusControl = null;
        }

        #region Public Methods

        /// <summary>
        /// Select a camera (1-6)
        /// </summary>
        public void SelectCamera(ushort camera)
        {
            int idx = camera - 1;
            if (idx >= 0 && idx < NUM_CAMERAS && _camSelectControls[idx] != null)
                _camSelectControls[idx].SendChangeBoolValue(true);
        }

        /// <summary>
        /// Recall a preset for a specific camera (cam 1-6, preset 1-16)
        /// </summary>
        public void RecallCameraPreset(ushort camera, ushort preset)
        {
            if (Component == null) return;
            if (camera < 1 || camera > NUM_CAMERAS || preset < 1 || preset > NUM_PRESETS) return;
            Component.SendChangeDoubleValue(string.Format(CAM_PRESET_RECALL_FORMAT, camera, preset), 1);
        }

        /// <summary>
        /// Save a preset for a specific camera (cam 1-6, preset 1-16)
        /// </summary>
        public void SaveCameraPreset(ushort camera, ushort preset)
        {
            if (Component == null) return;
            if (camera < 1 || camera > NUM_CAMERAS || preset < 1 || preset > NUM_PRESETS) return;
            Component.SendChangeDoubleValue(string.Format(CAM_PRESET_SAVE_FORMAT, camera, preset), 1);
        }

        /// <summary>
        /// Recall a preset for the currently selected camera (preset 1-16)
        /// </summary>
        public void RecallPreset(ushort preset)
        {
            int idx = preset - 1;
            if (idx >= 0 && idx < NUM_PRESETS && _presetRecallControls[idx] != null)
                _presetRecallControls[idx].SendChangeBoolValue(true);
        }

        /// <summary>
        /// Save a preset for the currently selected camera (preset 1-16)
        /// </summary>
        public void SavePreset(ushort preset)
        {
            if (Component == null) return;
            if (preset < 1 || preset > NUM_PRESETS) return;
            Component.SendChangeDoubleValue(string.Format(PRESET_SAVE_FORMAT, preset), 1);
        }

        /// <summary>
        /// Set tracking on/off
        /// </summary>
        public void SetTracking(ushort value)
        {
            if (_trackingControl != null)
                _trackingControl.SendChangeBoolValue(value.BoolFromSplus());
        }

        /// <summary>
        /// Load preset file
        /// </summary>
        public void LoadPresetFile()
        {
            if (Component != null)
                Component.SendChangeDoubleValue(LOAD_PRESETS, 1);
        }

        /// <summary>
        /// Save preset file
        /// </summary>
        public void SavePresetFile()
        {
            if (Component != null)
                Component.SendChangeDoubleValue(SAVE_PRESETS, 1);
        }

        /// <summary>
        /// Recalibrate cameras
        /// </summary>
        public void RecalibrateCameras()
        {
            if (Component != null)
                Component.SendChangeDoubleValue(RECALIBRATE, 1);
        }

        /// <summary>
        /// Refresh files
        /// </summary>
        public void RefreshPresetFiles()
        {
            if (Component != null)
                Component.SendChangeDoubleValue(REFRESH_FILES, 1);
        }

        /// <summary>
        /// Set a router output (1-11)
        /// </summary>
        public void SetRouterOutput(ushort output)
        {
            int idx = output - 1;
            if (idx >= 0 && idx < NUM_ROUTER_OUTPUTS && _routerOutputControls[idx] != null)
                _routerOutputControls[idx].SendChangeBoolValue(true);
        }

        #endregion

        #region Subscriptions

        private void SubscribeControl(NamedComponentControl control, EventHandler<QsysInternalEventsArgs> handler)
        {
            if (control == null) return;
            control.OnStateChanged += handler;
        }

        private void UnsubscribeControl(NamedComponentControl control)
        {
            if (control == null) return;
            control.OnStateChanged -= CamSelectControlChanged;
            control.OnStateChanged -= PresetButtonControlChanged;
            control.OnStateChanged -= PresetRecallControlChanged;
            control.OnStateChanged -= PnHProgressControlChanged;
            control.OnStateChanged -= RouterOutputControlChanged;
            control.OnStateChanged -= CameraNameControlChanged;
            control.OnStateChanged -= PresetNameControlChanged;
            control.OnStateChanged -= CameraPositionControlChanged;
            control.OnStateChanged -= TrackingControlChanged;
            control.OnStateChanged -= DisableControlChanged;
            control.OnStateChanged -= StatusTextControlChanged;
            control.OnStateChanged -= AutosaveStatusControlChanged;
        }

        #endregion

        #region Control Changed Handlers

        private void CamSelectControlChanged(object sender, QsysInternalEventsArgs args)
        {
            int idx = GetIndex(sender, _camSelectControls);
            if (idx < 0) return;
            var callback = newCamSelectChange;
            if (callback != null)
                callback(ComponentName, (ushort)(idx + 1), args.BoolValue.BoolToSplus());
        }

        private void PresetButtonControlChanged(object sender, QsysInternalEventsArgs args)
        {
            int idx = GetIndex(sender, _presetButtonControls);
            if (idx < 0) return;
            var callback = newPresetButtonChange;
            if (callback != null)
                callback(ComponentName, (ushort)(idx + 1), args.BoolValue.BoolToSplus());
        }

        private void PnHProgressControlChanged(object sender, QsysInternalEventsArgs args)
        {
            int idx = GetIndex(sender, _pnhProgressControls);
            if (idx < 0) return;
            var callback = newPnHProgressChange;
            if (callback != null)
                callback(ComponentName, (ushort)(idx + 1), args.BoolValue.BoolToSplus());
        }

        private void PresetRecallControlChanged(object sender, QsysInternalEventsArgs args)
        {
            int idx = GetIndex(sender, _presetRecallControls);
            if (idx < 0) return;
            var callback = newPresetRecallChange;
            if (callback != null)
                callback(ComponentName, (ushort)(idx + 1), args.BoolValue.BoolToSplus());
        }

        private void RouterOutputControlChanged(object sender, QsysInternalEventsArgs args)
        {
            int idx = GetIndex(sender, _routerOutputControls);
            if (idx < 0) return;
            var callback = newRouterOutputChange;
            if (callback != null)
                callback(ComponentName, (ushort)(idx + 1), args.BoolValue.BoolToSplus());
        }

        private void TrackingControlChanged(object sender, QsysInternalEventsArgs args)
        {
            var callback = newTrackingChange;
            if (callback != null)
                callback(ComponentName, args.BoolValue.BoolToSplus());
        }

        private void DisableControlChanged(object sender, QsysInternalEventsArgs args)
        {
            var callback = newDisableChange;
            if (callback != null)
                callback(ComponentName, args.BoolValue.BoolToSplus());
        }

        private void StatusTextControlChanged(object sender, QsysInternalEventsArgs args)
        {
            var callback = newStatusTextChange;
            if (callback != null && !string.IsNullOrEmpty(args.StringValue))
                callback(ComponentName, args.StringValue);
        }

        private void AutosaveStatusControlChanged(object sender, QsysInternalEventsArgs args)
        {
            var callback = newAutosaveStatusChange;
            if (callback != null && !string.IsNullOrEmpty(args.StringValue))
                callback(ComponentName, args.StringValue);
        }

        private void CameraNameControlChanged(object sender, QsysInternalEventsArgs args)
        {
            int idx = GetIndex(sender, _cameraNameControls);
            if (idx < 0) return;
            var callback = newCameraNameChange;
            if (callback != null && !string.IsNullOrEmpty(args.StringValue))
                callback(ComponentName, (ushort)(idx + 1), args.StringValue);
        }

        private void PresetNameControlChanged(object sender, QsysInternalEventsArgs args)
        {
            for (int c = 0; c < NUM_CAMERAS; c++)
            {
                for (int p = 0; p < NUM_PRESETS; p++)
                {
                    if (ReferenceEquals(sender, _presetNameControls[c, p]))
                    {
                        var callback = newPresetNameChange;
                        if (callback != null && !string.IsNullOrEmpty(args.StringValue))
                            callback(ComponentName, (ushort)(c + 1), (ushort)(p + 1), args.StringValue);
                        return;
                    }
                }
            }
        }

        private void CameraPositionControlChanged(object sender, QsysInternalEventsArgs args)
        {
            int idx = GetIndex(sender, _cameraPositionControls);
            if (idx < 0) return;
            var callback = newCameraPositionChange;
            if (callback != null && !string.IsNullOrEmpty(args.StringValue))
                callback(ComponentName, (ushort)(idx + 1), args.StringValue);
        }

        #endregion

        #region Helpers

        private int GetIndex(object sender, NamedComponentControl[] controls)
        {
            for (int i = 0; i < controls.Length; i++)
            {
                if (ReferenceEquals(sender, controls[i]))
                    return i;
            }
            return -1;
        }

        #endregion
    }
}
