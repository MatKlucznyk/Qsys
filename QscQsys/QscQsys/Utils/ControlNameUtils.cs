namespace QscQsys.Utils
{
    public static class ControlNameUtils
    {

        private const string MUTE = "mute";
        private const string GAIN = "gain";
        private const string THRESHOLD = "threshold";
        private const string HOLD_TIME = "hold_time";
        private const string INFINITE_HOLD = "infinite_hold";
        private const string SIGNAL_PRESENCE = "signal_presence";

        private const string MATRIX_MIXER_CONTROL_NAME_FORMAT = "input_{0}_output_{1}_{2}";
        private const string METER_NAME_FORMAT = "meter_{0}";
        private const string ROUTER_SELECT_NAME_FORMAT = "select_{0}";
        private const string ROUTER_MUTE_NAME_FORMAT = "mute_{0}";
        private const string SIGNAL_PRESENCE_METER_CONTROL_NAME_FORMAT = SIGNAL_PRESENCE + "_{0}";
        private const string SNAPSHOT_LOAD_CONTROL_NAME_FORMAT = "load_{0}";
        private const string SNAPSHOT_SAVE_CONTROL_NAME_FORMAT = "save_{0}";

        public static string GetMatrixCrosspointMuteName(int input, int output)
        {
            return string.Format(MATRIX_MIXER_CONTROL_NAME_FORMAT, input, output, MUTE);
        }

        public static string GetMatrixCrosspointGainName(int input, int output)
        {
            return string.Format(MATRIX_MIXER_CONTROL_NAME_FORMAT, input, output, GAIN);
        }

        public static string GetMeterName(int index)
        {
            return string.Format(METER_NAME_FORMAT, index);
        }

        public static string GetRouterSelectName(int output)
        {
            return string.Format(ROUTER_SELECT_NAME_FORMAT, output);
        }

        public static string GetRouterMuteName(int output)
        {
            return string.Format(ROUTER_MUTE_NAME_FORMAT, output);
        }

        public static string GetSignalPresenceMeterName(int index, int totalCount)
        {
            // special case handling - if only 1, index isn't used
            if (totalCount == 1 && index == 1)
                return SIGNAL_PRESENCE;
            
            return string.Format(SIGNAL_PRESENCE_METER_CONTROL_NAME_FORMAT, index);
        }

        public static string GetSnapshotLoadControlName(int index)
        {
            return string.Format(SNAPSHOT_LOAD_CONTROL_NAME_FORMAT, index);
        }

        public static string GetSnapshotSaveControlName(int index)
        {
            return string.Format(SNAPSHOT_SAVE_CONTROL_NAME_FORMAT, index);
        }

        public static string GetMuteControlName()
        {
            return MUTE;
        }

        public static string GetGainControlName()
        {
            return GAIN;
        }

        public static string GetThresholdControlName()
        {
            return THRESHOLD;
        }

        public static string GetHoldTimeControlName()
        {
            return HOLD_TIME;
        }

        public static string GetInfiniteHoldControlName()
        {
            return INFINITE_HOLD;
        }
    }
}